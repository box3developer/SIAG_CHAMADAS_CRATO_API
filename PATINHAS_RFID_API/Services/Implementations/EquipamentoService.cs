using System.Data;
using System.Text.Json;
using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Services.Implementations
{
    public class EquipamentoService : IEquipamentoService
    {
        private readonly IEquipamentoRepository _equipamentoRepository;
        private readonly IAtividadeRejeicaoRepository _atividadeRepository;
        private readonly IAreaArmazenagemRepository _areaArmazenagemRepository;
        private readonly ISetorRepository _setorRepository;
        private readonly ICheckListRepository _checkListRepository;
        private readonly ICheckListOperadorRepository _checkListOperadorRepository;

        public EquipamentoService(
            IEquipamentoRepository equipamentoRepository,
            IAtividadeRejeicaoRepository atividadeRepository,
            IAreaArmazenagemRepository areaArmazenagemRepository,
            ISetorRepository setorRepository,
            ICheckListRepository checkListRepository,
            ICheckListOperadorRepository checkListOperadorRepository)
        {
            _equipamentoRepository = equipamentoRepository;
            _atividadeRepository = atividadeRepository;
            _areaArmazenagemRepository = areaArmazenagemRepository;
            _setorRepository = setorRepository;
            _checkListRepository = checkListRepository;
            _checkListOperadorRepository = checkListOperadorRepository;
        }

        public async Task<ConfiguracaoModel> ConsultarConfiguracoes(long cracha, string identificadorEquipamento)
        {
            OperadorModel operador = new OperadorModel();
            operador.IdOperador = cracha;
            ChamadaModel chamada = new ChamadaModel();
            chamada.Operador = operador;

            EquipamentoModel equipamento = await _equipamentoRepository.Consultar(identificadorEquipamento, 0);

            List<AtividadeRejeicaoModel> motivos = await SiagAPI.GetListaAtividadeRejeicaoAsync();
            ConfiguracaoModel config = new ConfiguracaoModel();

            config.MotivosRejeicao = motivos;
            config.ConfirmarAceiteTarefa = false;   //fixo por enquanto
            config.InformarMotivoRejeicao = true;   //será sempre obrigatório
            config.OrdemLeitura = 0;                //fixo por enquanto | 1 - Armazenagem, pallet | 0 - Pallet, armazenagem (usado no caso de haver armazenagem e pallet com o mesmo tamanho no codigo de barras) 
            config.TamanhoCodigoArmazenagem = (int)Configuracoes.QtdeCaracteresCodBarraEnderecamento;
            config.TamanhoCodigoPallet = (int)Configuracoes.QtdeCaracteresCodBarraPallet;
            config.TempoAquisicaoTarefas = 5000;    //Fixo por enquanto
            config.TempoEstabilizacaoLeitura = 1000;//Fixo por enquanto
            config.ModeloEquipamento = equipamento.EquipamentoModelo;

            return config;
        }

        public async Task<int> ConsultarPerformance(long cracha)
        {
            if (string.IsNullOrWhiteSpace(cracha.ToString()))
            {
                throw new Exception("Operador não informado");
            }

            var filtros = new Dictionary<string, object>();
            filtros.Add("@idOperador", cracha);

            var parametros = new DynamicParameters(filtros);
            parametros.Add("@performance", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var query = "EXEC sp_siag_performanceonline @idOperador, @performance output";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = await conexao.ExecuteAsync(query, parametros);
            }

            Int32? parPerformance = parametros.Get<Int32?>("@performance");

            if (parPerformance.Value == null)
            {
                return (int)HumorEficiencia.Feliz;
            }
            else
            {
                return (int)parPerformance.Value;
            }
        }

        public async Task<bool> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento)
        {
            var equipamento = await _equipamentoRepository.Consultar(macEquipamento, 0);
            equipamento.SetorTrabalho = await _setorRepository.Consultar(equipamento.SetorTrabalho);

            var mAreaArmazenagem = string.Concat(equipamento.SetorTrabalho.Codigo.ToString(), retornoEquipamento.AsSpan(0, 5), "01", retornoEquipamento.AsSpan(5, 1));
            var areaArmazenagem = new AreaArmazenagemModel
            {
                IdAreaArmazenagem = Convert.ToInt64(mAreaArmazenagem)
            };

            areaArmazenagem = await SiagAPI.GetAreaArmazenagemByIdAsync(areaArmazenagem.IdAreaArmazenagem);

            if (areaArmazenagem != null)
            {
                await _equipamentoRepository.AtualizaMovimentacao(equipamento, areaArmazenagem.Endereco);
            }

            return true;
        }

        public async Task<List<EquipamentoChecklistModel>> GetCheckList(string identificadorEquipamento)
        {
            EquipamentoModel equipamento = new EquipamentoModel();
            equipamento.NmIdentificador = identificadorEquipamento;

            List<EquipamentoChecklistModel> lstChecklist = await _checkListRepository.ConsultarListaPorEquipamento(equipamento);

            return lstChecklist;
        }

        public async Task<bool> SetCheckList(SetCheckListDTO setCheckListDTO)
        {
            EquipamentoModel equipamento = new EquipamentoModel();
            equipamento.NmIdentificador = setCheckListDTO.IdentificadorEquipamento;

            //Busca equipamento pelo identificador
            List<EquipamentoModel> lstEquipamento = await _equipamentoRepository.ConsultarLista(equipamento);
            //Utiliza somente a primeira posição pois identificador é único na tabela Equipamento
            equipamento = lstEquipamento[0];

            OperadorModel operador = new OperadorModel();
            operador.IdOperador = setCheckListDTO.CodOperador;
            //Deserializa JSon como uma lista de dicionários, cada dicionário com código do checklist e valor da resposta (0 ou 1)
            List<Dictionary<string, string>> lstChecklistGenerico = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(setCheckListDTO.ChecklistResponse);

            foreach (Dictionary<string, string> dicionario in lstChecklistGenerico)
            {
                foreach (var item in dicionario)
                {
                    EquipamentoChecklistModel checklist = new EquipamentoChecklistModel();
                    checklist.IdEquipamentoChecklist = Convert.ToInt32(item.Key);

                    //Monta objeto ChecklistOperador
                    EquipamentoChecklistOperadorModel checklistOperador = new EquipamentoChecklistOperadorModel();
                    checklistOperador.Equipamento = equipamento;
                    checklistOperador.Operador = operador;
                    checklistOperador.Checklist = checklist;
                    checklistOperador.Resposta = Convert.ToBoolean(int.Parse(item.Value));
                    checklistOperador.Data = DateTime.Now;

                    //Insere cada resposta na tabela EquipamentoChecklistOperador
                    _checkListOperadorRepository.Inserir(checklistOperador);
                }
            }

            return true;
        }

    }
}
