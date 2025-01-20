using System.Text.Json;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Services.Implementations;

public class EquipamentoService : IEquipamentoService
{
    private readonly IEquipamentoRepository _equipamentoRepository;
    private readonly ICheckListOperadorRepository _checkListOperadorRepository;

    public EquipamentoService(IEquipamentoRepository equipamentoRepository, ICheckListOperadorRepository checkListOperadorRepository)
    {
        _equipamentoRepository = equipamentoRepository;
        _checkListOperadorRepository = checkListOperadorRepository;
    }

    public async Task<ConfiguracaoModel> ConsultarConfiguracoes(long cracha, string identificadorEquipamento)
    {
        var equipamento = await _equipamentoRepository.GetByIdentificador(identificadorEquipamento);

        var motivos = await SiagAPI.GetListaAtividadeRejeicaoAsync();

        ConfiguracaoModel config = new()
        {
            MotivosRejeicao = motivos,
            ConfirmarAceiteTarefa = false,   //fixo por enquanto
            InformarMotivoRejeicao = true,   //será sempre obrigatório
            OrdemLeitura = 0,                //fixo por enquanto | 1 - Armazenagem, pallet | 0 - Pallet, armazenagem (usado no caso de haver armazenagem e pallet com o mesmo tamanho no codigo de barras) 
            TamanhoCodigoArmazenagem = (int)Configuracoes.QtdeCaracteresCodBarraEnderecamento,
            TamanhoCodigoPallet = (int)Configuracoes.QtdeCaracteresCodBarraPallet,
            TempoAquisicaoTarefas = 5000,    //Fixo por enquanto
            TempoEstabilizacaoLeitura = 1000,//Fixo por enquanto
            ModeloEquipamento = equipamento?.EquipamentoModelo
        };

        return config;
    }

    public async Task<int> ConsultarPerformance(long cracha)
    {
        if (string.IsNullOrWhiteSpace(cracha.ToString()))
        {
            throw new Exception("Operador não informado");
        }

        return await SiagAPI.GetOperadorPerformance(cracha);
    }

    public async Task<bool> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento)
    {
        var equipamento = await _equipamentoRepository.GetByIdentificador(macEquipamento);

        if (equipamento == null)
        {
            return false;
        }

        equipamento.SetorTrabalho = await SiagAPI.GetSetorByIdAsync(equipamento.IdSetorTrabalho ?? 0);

        if (equipamento.SetorTrabalho == null)
        {
            return false;
        }

        var mAreaArmazenagem = string.Concat(equipamento.SetorTrabalho.IdSetorTrabalho.ToString(), retornoEquipamento.AsSpan(0, 5), "01", retornoEquipamento.AsSpan(5, 1));

        var areaArmazenagem = new AreaArmazenagemModel
        {
            IdAreaArmazenagem = Convert.ToInt64(mAreaArmazenagem)
        };

        areaArmazenagem = await SiagAPI.GetAreaArmazenagemByIdAsync(areaArmazenagem.IdAreaArmazenagem);

        if (areaArmazenagem != null)
        {
            await SiagAPI.AtualizarEnderecoEquipamentoAsync(equipamento.IdEquipamento, areaArmazenagem.IdEndereco);
        }

        return true;
    }

    public async Task<List<EquipamentoChecklistModel>> GetCheckList(string identificadorEquipamento)
    {
        var checklists = await SiagAPI.GetEquipamentosChecklistAsync(identificadorEquipamento);

        return checklists;
    }

    public async Task<bool> SetCheckList(SetCheckListDTO setCheckListDTO)
    {
        var equipamento = await _equipamentoRepository.GetByIdentificador(setCheckListDTO.IdentificadorEquipamento);

        OperadorModel operador = new()
        {
            IdOperador = setCheckListDTO.CodOperador
        };

        if (string.IsNullOrEmpty(setCheckListDTO.ChecklistResponse))
        {
            return false;
        }

        var listChecklistGenerico = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(setCheckListDTO.ChecklistResponse) ?? new();

        foreach (var dicionario in listChecklistGenerico)
        {
            foreach (var item in dicionario)
            {
                EquipamentoChecklistModel checklist = new()
                {
                    IdEquipamentoChecklist = Convert.ToInt32(item.Key)
                };

                EquipamentoChecklistOperadorModel checklistOperador = new()
                {
                    Equipamento = equipamento,
                    IdEquipamento = equipamento?.IdEquipamento ?? 0,
                    Operador = operador,
                    IdOperador = operador?.IdOperador ?? 0,
                    Checklist = checklist,
                    IdEquipamentoChecklist = checklist.IdEquipamentoChecklist,
                    FgResposta = Convert.ToBoolean(int.Parse(item.Value)),
                    DtChecklist = DateTime.Now
                };

                await _checkListOperadorRepository.Inserir(checklistOperador);
            }
        }

        return true;
    }
}
