using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Services.Implementations
{
    public class OperadorService : IOperadorService
    {
        private readonly IEquipamentoRepository _equipamentoRepository;
        private readonly IOperadorRepository _operadorRepository;
        private readonly IChamadaRepository _chamadaRepository;

        public OperadorService(IEquipamentoRepository equipamentoRepository, IOperadorRepository operadorRepository, IChamadaRepository chamadaRepository)
        {
            _equipamentoRepository = equipamentoRepository;
            _operadorRepository = operadorRepository;
            _chamadaRepository = chamadaRepository;
        }

        public async Task<OperadorModel?> ConsultarOperador(ConsultarOperadorDTO consultarOperadorDTO)
        {
            ChamadaModel chamada = new ChamadaModel();
            OperadorModel? operador = new OperadorModel();

            long.TryParse(consultarOperadorDTO.Cracha, out long codigo);

            if (codigo == 0)
            {
                operador.NmNfcOperador = consultarOperadorDTO.Cracha;
            }
            else
            {
                operador.IdOperador = codigo;
            }

            //Consulta o operador através do código (crachá)
            operador = await SiagAPI.GetOperadorAsync(operador.IdOperador, operador.NmNfcOperador);

            if (operador != null)
            {
                chamada.Operador = operador;

                //Monta lista de status que devem entrar na consulta de chamadas
                List<StatusChamada> listaStatus = new List<StatusChamada>();
                listaStatus.Add(StatusChamada.Recebido);
                listaStatus.Add(StatusChamada.Andamento);

                //Verifica se o operador possui alguma chamada ativa atribuida
                List<ChamadaModel> lstChamada = await _chamadaRepository.ConsultarListaNew(chamada, listaStatus);

                //Altera todas chamadas ativas do operador
                foreach (ChamadaModel item in lstChamada)
                {
                    await SiagAPI.ReiniciarChamadaAsync(item.IdChamada);
                }
            }

            // Faz logoff de todos os equipamentos que o operador está conectado
            // equipamentoBO.logoffOperador("", cracha);
            //Altera o equipamento relacionando o operador consultado
            // equipamentoBO.editar(identificadorEquipamento, operador);

            // método de login, já executa o logoff de outros operadores/equipamentos automaticamente
            EquipamentoModel equipamento = await SiagAPI.GetEquipamentoByIdentificadorAsync(consultarOperadorDTO.IdentificadorEquipamento);

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
                EquipamentoModel equipamento = await SiagAPI.GetEquipamentoByIdentificadorAsync(identificadorEquipamento);
                OperadorModel operador = new OperadorModel();
                Int64 codigo = 0;
                Int64.TryParse(cracha, out codigo);
                operador.IdOperador = codigo;

                await SiagAPI.LogoffOperadorAsync(operador.IdOperador, equipamento.IdEquipamento);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
