using dotnet_api.Utils;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;
using PATINHAS_RFID_API.Utils;

namespace PATINHAS_RFID_API.Services.Implementations
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IEquipamentoRepository _equipamentoRepository;
        private readonly IAtividadeRepository _atividadeRepository;
        private readonly IAtividadeTarefaRepository _atividadeTarefaRepository;
        private readonly IChamadaRepository _chamadaRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ILogRepository _logRepository;
        private readonly IAtividadeRotinaRepository _atividadeRotinaRepository;
        private readonly IPalletRepository _palletRepository;
        private readonly IChamadaTarefaRepository _chamadaTarefaRepository;

        static Dictionary<string, Queue<int>> chamadaFIFO = new Dictionary<string, Queue<int>>();

        public AtividadeService(
            IEquipamentoRepository equipamentoRepository,
            IAtividadeRepository atividadeRepository,
            IAtividadeTarefaRepository atividadeTarefaRepository,
            IChamadaRepository chamadaRepository,
            IAreaRepository areaRepository,
            ILogRepository logRepository,
            IAtividadeRotinaRepository atividadeRotinaRepository,
            IPalletRepository palletRepository,
            IChamadaTarefaRepository chamadaTarefaRepository
            )
        {
            _equipamentoRepository = equipamentoRepository;
            _atividadeRepository = atividadeRepository;
            _atividadeTarefaRepository = atividadeTarefaRepository;
            _chamadaRepository = chamadaRepository;
            _areaRepository = areaRepository;
            _logRepository = logRepository;
            _atividadeRotinaRepository = atividadeRotinaRepository;
            _palletRepository = palletRepository;
            _chamadaTarefaRepository = chamadaTarefaRepository;
        }

        public async Task<bool> IniciarTarefa(string idChamada, int idTarefa)
        {
            ChamadaModel chamada = new ChamadaModel();
            chamada.Codigo = new Guid(idChamada);
            chamada = await _chamadaRepository.Consultar(chamada);

            List<StatusChamada> statusAux = new List<StatusChamada>() { StatusChamada.Dependente, StatusChamada.Aguardando, StatusChamada.Recebido };
            if (statusAux.Contains(chamada.Status))
            {
                chamada.Status = StatusChamada.Andamento;
                await _chamadaRepository.EditarStatus(chamada);
            }

            //Alteração da tarefa da chamada atualizando campo data inicio                
            ChamadaTarefaModel chamadaTarefa = new ChamadaTarefaModel();
            chamadaTarefa.Chamada = chamada;
            AtividadeTarefaModel atividadeTarefa = new AtividadeTarefaModel();
            atividadeTarefa.Codigo = idTarefa;
            chamadaTarefa.Tarefa = atividadeTarefa;
            chamadaTarefa.DataInicio = DateTime.Now;
            await SiagAPI.UpdateChamadaTarefa(chamadaTarefa);

            return true;
        }

        public async Task<MensagemWebservice> EfetivaLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa)
        {
            try
            {
                ChamadaModel chamada = new ChamadaModel();
                chamada.Codigo = new Guid(idChamada);
                //Consulta a chamada conforme parametro
                chamada = await _chamadaRepository.Consultar(chamada);

                //if (chamada.PalletOrigem != null) identificadorPallet = chamada.PalletOrigem.Identificacao;
                //else identificadorPallet = chamada.PalletDestino.Identificacao;

                //if (chamada.AreaArmazenagemOrigem != null) identificadorAreaArmazenagem = chamada.AreaArmazenagemOrigem.Identificacao;
                //else identificadorAreaArmazenagem = chamada.AreaArmazenagemDestino.Identificacao;

                bool leituraAlterada = false;

                if (chamada != null && chamada.Status != StatusChamada.Rejeitado && chamada.Status != StatusChamada.Finalizado)
                {
                    AreaArmazenagemModel areaArmazenagemLeitura = chamada.AreaArmazenagemLeitura;
                    PalletModel palletLeitura = chamada.PalletLeitura;

                    //Armazena codigos lidos no objeto da chamada
                    if ((!String.IsNullOrEmpty(identificadorAreaArmazenagem)) && (identificadorAreaArmazenagem != "0"))
                    {
                        //AreaArmazenagemBO areaBO = new AreaArmazenagemBO(paramWeb, chamadaBO); //transacao
                        chamada.AreaArmazenagemLeitura = await SiagAPI.GetAreaArmazenagemByIdentificadorAsync(identificadorAreaArmazenagem);

                        if (chamada.AreaArmazenagemLeitura == null)
                        {
                            AreaArmazenagemModel areaArmazenagem = new AreaArmazenagemModel();
                            long codigoArea = 0;
                            long.TryParse(identificadorAreaArmazenagem, out codigoArea);

                            if (codigoArea > 0)
                            {
                                areaArmazenagem.Codigo = codigoArea;
                                chamada.AreaArmazenagemLeitura = await SiagAPI.GetAreaArmazenagemByIdAsync(areaArmazenagem.Codigo);
                            }
                        }
                        // verifica se houve mudança na leitura da area
                        if (((areaArmazenagemLeitura == null) && (chamada.AreaArmazenagemLeitura != null)) ||
                            ((areaArmazenagemLeitura != null) && (chamada.AreaArmazenagemLeitura == null)) ||
                            ((areaArmazenagemLeitura != null) && (!areaArmazenagemLeitura.Codigo.Equals(chamada?.AreaArmazenagemLeitura?.Codigo ?? 0))))
                        {
                            leituraAlterada = true;
                        }
                    }
                    if ((!String.IsNullOrEmpty(identificadorPallet)) && (identificadorPallet != "0"))
                    {
                        chamada.PalletLeitura = await _palletRepository.Consultar(identificadorPallet);
                        if (chamada.PalletLeitura == null)
                        {
                            PalletModel pallet = new PalletModel();
                            try
                            {
                                pallet.Codigo = Convert.ToInt32(identificadorPallet.Substring(3));
                            }
                            catch (Exception)
                            {
                                return new MensagemWebservice
                                {
                                    Retorno = false,
                                    Mensagem = "Erro ao cadastrar o pallet " + identificadorPallet,
                                };
                            }
                            pallet.Status = StatusPallet.Livre;
                            pallet.Identificacao = identificadorPallet;
                            pallet.QtUtilizacao = 0;
                            await _palletRepository.Inserir(pallet);

                            chamada.PalletLeitura = pallet;
                        }
                        // verifica se houve mudança na leitura da area
                        if (((palletLeitura == null) && (chamada.PalletLeitura != null)) ||
                            ((palletLeitura != null) && (chamada.PalletLeitura == null)) ||
                            ((palletLeitura != null) && (!palletLeitura.Codigo.Equals(chamada.PalletLeitura.Codigo))))
                        {
                            leituraAlterada = true;
                        }
                    }

                    //Procura tarefa da chamada pelo idTarefa
                    ChamadaTarefaModel? chamadaTarefa = null;
                    List<ChamadaTarefaModel> tarefas = await _chamadaRepository.ConsultarTarefas(chamada);
                    if (idTarefa > 0)
                    {
                        foreach (ChamadaTarefaModel tarefa in tarefas)
                        {
                            tarefa.Tarefa = await SiagAPI.GetAtividadeTarefaById(tarefa.Tarefa.Codigo);

                            if (tarefa.Tarefa.Codigo == Convert.ToInt32(idTarefa))
                            {
                                chamadaTarefa = tarefa;
                            }
                        }
                    }

                    if (chamadaTarefa != null)
                    {
                        //Consulta atividade rotina a partir da atividade tarefa
                        //AtividadeRotina atividadeRotina = new AtividadeRotina();
                        chamadaTarefa.Tarefa.AtividadeRotina = await SiagAPI.GetAtividadeRotinaById(chamadaTarefa.Tarefa.AtividadeRotina.Codigo);

                        string mensagem = "";

                        //Salva leituras recebidas
                        await _chamadaRepository.AtualizaLeitura(chamada);

                        //A partir da atividade rotina consulta a stored procedure para que valide as leituras recebidas
                        // Ignora execução caso a data de fim da tarefa esteja preenchida, e não houve alteração na leitura
                        var leituraValidada = _chamadaRepository.ValidaLeitura(chamada, chamadaTarefa.Tarefa.AtividadeRotina, out mensagem);

                        if (((chamadaTarefa.DataFim != null) && (!leituraAlterada)) || leituraValidada)
                        {
                            //PROCESSO PARA APAGAR LUZ DO CARACOL USANDO API DO NODERED -- O GURGEL QUE MANDOU
                            try
                            {
                                if (chamada.Atividade != null && chamada.Atividade.Codigo == 1)
                                {
                                    if (chamada.AreaArmazenagemOrigem != null && chamada.AreaArmazenagemOrigem.Codigo > 0)
                                    {
                                        string area = chamada.AreaArmazenagemOrigem.Codigo.ToString();

                                        if (area.Length == 9)
                                        {
                                            string caracol = int.Parse(area.Substring(3, 3)).ToString();
                                            string gaiola = int.Parse(area.Substring(6, 2)).ToString();

                                            string url = "http://gra-lxsobcaracol.sob.ad-grendene.com:1880/apagaluzvm/" + caracol + "/" + gaiola + "/";

                                            try
                                            {
                                                await WebRequestUtil.GetRequest(url);
                                            }
                                            catch (Exception ex)
                                            {
                                                string mensagemLog = "APAGARLUZ=ERROAPI;" + identificadorPallet + ";" + identificadorAreaArmazenagem + ";" + idChamada + ";" + idTarefa + ";" + caracol + ";" + gaiola + ";" + ex.Message;
                                                await _logRepository.Insere(mensagemLog);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string mensagemLog = "APAGARLUZ=ERRO;" + identificadorPallet + ";" + identificadorAreaArmazenagem + ";" + idChamada + ";" + idTarefa + ";" + ex.Message;
                                await _logRepository.Insere(mensagemLog);
                            }

                            //Edita chamadaTarefa colocando a data final da tarefa
                            chamadaTarefa.DataFim = DateTime.Now;
                            _chamadaRepository.EditarTarefa(chamadaTarefa);

                            // atualiza data de movimentação do equipamento
                            await _equipamentoRepository.AtualizaMovimentacao(chamada.Equipamento);
                            //_equipamentoRepository.Dispose();

                            //Só finalizar se todas as tarefas estiverem concluidas
                            Boolean finalizarChamada = true;
                            foreach (ChamadaTarefaModel item in tarefas)
                            {
                                //se alguma tarefa não estiver finalizada retorna false
                                if (item.DataFim == null)
                                {
                                    finalizarChamada = false;
                                    break;
                                }
                            }

                            //se todas as taredas estiverem finalizadas edita chamada alterando status para finalizada (Isto poreria ser feito dentro do agendamento da proxima tarefa)
                            if (finalizarChamada)
                            {
                                //altera status da chamada para finalizada e inicia as proximas chamadas que dependem desta
                                await _chamadaRepository.FinalizarChamada(chamada);
                            }
                            // Executa próxima atividade automática (se houver)
                            else
                            {
                                bool executar = false;
                                foreach (ChamadaTarefaModel item in tarefas)
                                {
                                    if (item.Tarefa.Codigo == chamadaTarefa.Tarefa.Codigo)
                                    {
                                        executar = true;
                                    }
                                    else if (executar)
                                    {
                                        if (item.Tarefa.Recursos == Recursos.Automatico)
                                        {
                                            await IniciarTarefa(chamada.Codigo.ToString(), item.Tarefa.Codigo);
                                            MensagemWebservice msg = await EfetivaLeitura("", "", chamada.Codigo.ToString(), item.Tarefa.Codigo);
                                            if (!msg.Retorno)
                                            {
                                                throw new Exception(msg.Mensagem);
                                                //chamadaTarefa.DataFim = null;
                                                //chamadaBO.editarTarefa(chamadaTarefa);
                                                //return msg;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            // Retorna mensagem para o cliente
                            return new MensagemWebservice()
                            {
                                Retorno = true,
                                Mensagem = mensagem
                            };
                        }
                        else
                        {
                            //Se leitura estiver inválida
                            return new MensagemWebservice()
                            {
                                Retorno = false,
                                Mensagem = mensagem
                            };
                        }
                    }
                    else
                    {
                        //se não achar id da tarefa
                        return new MensagemWebservice()
                        {
                            Retorno = false,
                            Mensagem = "Tarefa não encontrada"
                        };
                    }
                }
                else
                {
                    //Retorna falso se tarefa já estiver rejeitada ou finalizada
                    return new MensagemWebservice()
                    {
                        Retorno = true,
                        Mensagem = ""
                    };
                }
            }
            catch (Exception ex)
            {
                return new MensagemWebservice()
                {
                    Retorno = false,
                    Mensagem = ex.Message
                };
            }
        }

        public async Task<List<ChamadaCompletaModel>> SelecionaTarefa(SelecionarTarefaDTO selecionarTarefaDTO)
        {
            //Atribui o operador e o equipamento na chamada
            OperadorModel operador = new OperadorModel();
            operador.Codigo = selecionarTarefaDTO.Cracha;
            ChamadaModel chamada = new ChamadaModel();
            chamada.Operador = operador;
            EquipamentoModel equipamento = new EquipamentoModel();
            equipamento.Identificador = selecionarTarefaDTO.IdentificadorEquipamento;
            equipamento = await _equipamentoRepository.Consultar(selecionarTarefaDTO.IdentificadorEquipamento);

            if ((string.IsNullOrEmpty(equipamento.IP)) || (!selecionarTarefaDTO.IpEquipamento.Equals(equipamento.IP)))
            {
                equipamento.IP = selecionarTarefaDTO.IpEquipamento; // HttpContext.Current.Request.UserHostAddress;
                _equipamentoRepository.Editar(equipamento);
            }


            chamada.Equipamento = equipamento;

            // Monta código responsável por bloquear chamadas concorrentes/simultaneas para um mesmo modelo e setor.
            string codigoBloqueio;
            try
            {
                codigoBloqueio = equipamento.Modelo.Codigo.ToString() + "_" + equipamento.Setor.Codigo.ToString();
            }
            catch
            {
                codigoBloqueio = "";
            }

            lock (chamadaFIFO)
            {
                if (!chamadaFIFO.ContainsKey(codigoBloqueio))
                {
                    chamadaFIFO.Add(codigoBloqueio, new Queue<int>());
                }
            }

            ChamadaModel chamadaAtual = null;

            //Lista para retornar
            List<ChamadaCompletaModel> lstChamadaCompleta = new List<ChamadaCompletaModel>();

            lock (chamadaFIFO[codigoBloqueio])
            {
                chamadaAtual = _chamadaRepository.SelecionaChamadaEquipamento(chamada).Result;


                if (chamadaAtual != null)
                {
                    //Cria objeto para envio das tarefas ao tablet por JSON
                    ChamadaCompletaModel completa = new ChamadaCompletaModel();
                    completa.Tarefas = new List<AtividadeTarefaModel>();
                    List<AtividadeTarefaModel> atividades;

                    //Consulta detalhes da atividade relacionada com o Chamado
                    if (chamadaAtual.Atividade != null && chamadaAtual.Atividade.Codigo != 0)
                    {
                        chamadaAtual.Atividade = SiagAPI.GetAtividadeById(chamadaAtual.Atividade.Codigo).Result;
                        AtividadeTarefaModel atividadeTarefa = new AtividadeTarefaModel();
                        atividadeTarefa.Atividade = chamadaAtual.Atividade;

                        //Consulta lista de atividadades da tarefa relacionada com o Chamado
                        atividades = _atividadeTarefaRepository.ConsultarLista(atividadeTarefa, chamadaAtual).Result;
                    }
                    else
                    {
                        atividades = new List<AtividadeTarefaModel>();
                    }

                    lstChamadaCompleta.Add(completa);

                    //Altera a data de recebimento e o status da Chamada para Recebida
                    chamadaAtual.Equipamento = equipamento;
                    chamadaAtual.Operador = operador;

                    if (chamadaAtual.Status == StatusChamada.Aguardando)
                    {
                        ChamadaModel chamadaConf = new ChamadaModel();
                        try
                        {
                            chamadaConf = _chamadaRepository.Consultar(chamadaAtual).Result;
                            if (chamadaConf.Status == StatusChamada.Aguardando)
                            {
                                chamadaAtual.Status = StatusChamada.Recebido;
                                _chamadaRepository.AtribuirChamada(chamadaAtual);

                                // Altera a localização atual do equipamento que recebeu a chamada
                                if ((chamadaAtual.AreaArmazenagemOrigem != null) && (chamadaAtual.AreaArmazenagemOrigem.Codigo > 0))
                                {
                                    AreaArmazenagemModel areaarmazenagemAux = new AreaArmazenagemModel();
                                    areaarmazenagemAux = SiagAPI.GetAreaArmazenagemByIdAsync(chamadaAtual.AreaArmazenagemOrigem.Codigo).Result;
                                    if (areaarmazenagemAux != null)
                                    {
                                        EnderecoModel endereco = new EnderecoModel();
                                        endereco.Codigo = areaarmazenagemAux.Endereco.Codigo;
                                        _equipamentoRepository.AtualizaMovimentacao(equipamento, endereco);
                                    }
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        finally
                        {
                            chamadaConf = null;
                        }
                    }

                    // Remove da lista todas as tarefas automáticas (que não dependem de interação do equipamento)
                    // Obs: executa as tarefas automáticas antes da primeira interação
                    bool executar = true;
                    bool refazerConsulta = false;
                    foreach (AtividadeTarefaModel tarefa in atividades)
                    {
                        if (tarefa.Recursos == Recursos.Automatico)
                        {
                            if (executar)
                            {
                                executar = false; // a efetivaLeitura() executa recursivamente as tarefas automáticas seguintes
                                IniciarTarefa(chamadaAtual.Codigo.ToString(), tarefa.Codigo);
                                EfetivaLeitura("", "", chamadaAtual.Codigo.ToString(), tarefa.Codigo);
                                refazerConsulta = true;
                            }
                        }
                        else
                        {
                            executar = false;
                            completa.Tarefas.Add(tarefa);
                        }
                    }

                    // refaz a consulta, para o caso de ter modificado algum parametro durante a execução automatica das tarefas
                    if (refazerConsulta)
                    {
                        chamadaAtual = _chamadaRepository.Consultar(chamadaAtual).Result;
                        _atividadeTarefaRepository.AjustaMensagens(chamadaAtual, completa.Tarefas);
                    }

                    completa.Codigo = chamadaAtual.Codigo;
                    completa.PalletOrigem = chamadaAtual.PalletOrigem;
                    completa.PalletDestino = chamadaAtual.PalletDestino;
                    completa.PalletLeitura = chamadaAtual.PalletLeitura;
                    completa.AreaArmazenagemOrigem = chamadaAtual.AreaArmazenagemOrigem;
                    completa.AreaArmazenagemDestino = chamadaAtual.AreaArmazenagemDestino;
                    completa.AreaArmazenagemLeitura = chamadaAtual.AreaArmazenagemLeitura;
                    completa.Operador = chamadaAtual.Operador;
                    completa.Equipamento = chamadaAtual.Equipamento;
                    completa.AtividadeRejeicao = chamadaAtual.AtividadeRejeicao;
                    completa.Atividade = chamadaAtual.Atividade;
                    completa.Status = chamadaAtual.Status;
                    completa.DataChamada = chamadaAtual.DataChamada;
                    completa.DataRecebida = chamadaAtual.DataRecebida;
                    completa.DataAtendida = chamadaAtual.DataAtendida;
                    completa.DataFinalizada = chamadaAtual.DataFinalizada;
                    completa.DataRejeitada = chamadaAtual.DataRejeitada;

                    // testa se existe alguma tarefa pendente na chamada
                    if (completa.Tarefas.Count == 0)
                    {
                        // executou tarefas automaticas e não sobrou nenhuma tarefa, refaz a consulta
                        // faz isso apenas na primeira vez, para não travar o WS se houver sequencia de chamadas nessa situação
                        if (refazerConsulta && selecionarTarefaDTO.Reconsultar)
                        {
                            var selecionarTarefa = new SelecionarTarefaDTO
                            {
                                Cracha = selecionarTarefaDTO.Cracha,
                                IdentificadorEquipamento = selecionarTarefaDTO.IdentificadorEquipamento,
                                Reconsultar = false,
                                IpEquipamento = selecionarTarefaDTO.IpEquipamento,
                            };

                            return SelecionaTarefa(selecionarTarefa).Result;
                        }
                        else
                        {
                            lstChamadaCompleta.Clear();
                        }
                    }
                }
            }

            return lstChamadaCompleta;
        }

        public async Task<bool> RejeitarTarefa(long cracha, int idMotivo, string? idChamada = null)
        {
            ChamadaModel chamada = new ChamadaModel();

            if (idChamada != null && idMotivo > 0)
            {
                //Se código da chamada for nulo rejeita todas chamdas do operador
                OperadorModel operador = new OperadorModel();
                operador.Codigo = cracha;
                chamada.Operador = operador;

                chamada.Codigo = new Guid(idChamada);
                chamada = await _chamadaRepository.Consultar(chamada);

                chamada.Atividade = await SiagAPI.GetAtividadeById(chamada.Atividade.Codigo);
                if (chamada.Atividade.PermiteRejeitar == RejeicaoTarefa.Permite)
                {
                    chamada.AtividadeRejeicao = new AtividadeRejeicaoModel();
                    chamada.AtividadeRejeicao.Codigo = idMotivo;

                    return await _chamadaRepository.RejeitarChamada(chamada);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
