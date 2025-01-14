using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Services.Implementations;

public class OperadorService : IOperadorService
{
    private readonly IChamadaRepository _chamadaRepository;

    public OperadorService(IChamadaRepository chamadaRepository)
    {
        _chamadaRepository = chamadaRepository;
    }

    public async Task<OperadorModel?> ConsultarOperador(ConsultarOperadorDTO consultarOperadorDTO)
    {
        OperadorModel? operador;

        var _ = long.TryParse(consultarOperadorDTO.Cracha, out long codigo);

        if (codigo == 0)
        {
            operador = await SiagAPI.GetOperadorByNFCAsync(consultarOperadorDTO.Cracha);
        }
        else
        {
            operador = await SiagAPI.GetOperadorByCrachaAsync(codigo);
        }

        if (operador == null)
        {
            throw new Exception("Operador não encontrado");
        }

        ChamadaModel chamada = new()
        {
            Operador = operador
        };

        List<StatusChamada> listaStatus = new()
        {
            StatusChamada.Recebido,
            StatusChamada.Andamento
        };

        //Verifica se o operador possui alguma chamada ativa atribuida
        var listaChamadas = await _chamadaRepository.ConsultarListaNew(chamada, listaStatus);

        //Altera todas chamadas ativas do operador
        foreach (ChamadaModel item in listaChamadas)
        {
            await SiagAPI.ReiniciarChamadaAsync(item.IdChamada);
        }

        // Faz logoff de todos os equipamentos que o operador está conectado
        // equipamentoBO.logoffOperador("", cracha);
        //Altera o equipamento relacionando o operador consultado
        // equipamentoBO.editar(identificadorEquipamento, operador);

        // método de login, já executa o logoff de outros operadores/equipamentos automaticamente
        var equipamento = await SiagAPI.GetEquipamentoByIdentificadorAsync(consultarOperadorDTO.IdentificadorEquipamento);

        if (equipamento == null)
        {
            throw new Exception("Equipamento não encontrado");
        }

        await SiagAPI.LoginOperadorAsync(operador.IdOperador, equipamento.IdEquipamento);

        return operador;
    }

    public async Task<bool> Logoff(string cracha, string identificadorEquipamento)
    {
        try
        {
            var equipamento = await SiagAPI.GetEquipamentoByIdentificadorAsync(identificadorEquipamento);

            if (equipamento == null)
            {
                return false;
            }

            var _ = long.TryParse(cracha, out long codigo);

            await SiagAPI.LogoffOperadorAsync(codigo, equipamento.IdEquipamento);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
