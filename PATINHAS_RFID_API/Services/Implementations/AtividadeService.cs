using dotnet_api.Utils;
using Microsoft.IdentityModel.Tokens;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Interfaces;
using PATINHAS_RFID_API.Utils;

namespace PATINHAS_RFID_API.Services.Implementations;

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

    private static readonly Dictionary<string, Queue<int>> chamadaFIFO = new();
    private static readonly string nodeRedURL = "";
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

    public async Task<List<ChamadaCompletaModel>?> SelecionaTarefa(SelecionarTarefaDTO selecionarTarefaDTO)
    {
        OperadorModel operador = new()
        {
            IdOperador = selecionarTarefaDTO.Cracha
        };

        ChamadaModel chamada = new()
        {
            Operador = operador
        };

        var equipamento = await _equipamentoRepository.GetByIdentificador(selecionarTarefaDTO.IdentificadorEquipamento);

        if (equipamento == null)
        {
            throw new Exception("Equipamento não encontrado!");
        }

        if (string.IsNullOrEmpty(equipamento.NmIP) || (!selecionarTarefaDTO.IpEquipamento.Equals(equipamento.NmIP)))
        {
            equipamento.NmIP = selecionarTarefaDTO.IpEquipamento;
            _equipamentoRepository.Editar(equipamento);
        }

        chamada.Equipamento = equipamento;

        // Código responsável por bloquear chamadas concorrentes/simultaneas para um mesmo modelo e setor.
        string codigoBloqueio = equipamento.IdEquipamentoModelo.ToString() + "_" + equipamento.IdEquipamentoModelo.ToString();

        lock (chamadaFIFO)
        {
            if (!chamadaFIFO.ContainsKey(codigoBloqueio))
            {
                chamadaFIFO.Add(codigoBloqueio, new Queue<int>());
            }
        }

        List<ChamadaCompletaModel> listaChamadaCompleta = new();

        lock (chamadaFIFO[codigoBloqueio])
        {
            var chamadaAtual = _chamadaRepository.SelecionaChamadaEquipamento(chamada).Result;

            if (chamadaAtual == null)
            {
                return listaChamadaCompleta;
            }

            //Cria objeto para envio das tarefas ao tablet por JSON
            List<AtividadeTarefaModel> atividades;
            ChamadaCompletaModel completa = new()
            {
                Tarefas = new List<AtividadeTarefaModel>()
            };

            //Consulta detalhes da atividade relacionada com o Chamado
            if (chamadaAtual.Atividade != null && chamadaAtual.Atividade.IdAtividade != 0)
            {
                chamadaAtual.Atividade = SiagAPI.GetAtividadeByIdAsync(chamadaAtual.Atividade.IdAtividade).Result;

                AtividadeTarefaModel atividadeTarefa = new()
                {
                    Atividade = chamadaAtual.Atividade
                };

                //Consulta lista de atividadades da tarefa relacionada com o Chamado
                atividades = _atividadeTarefaRepository.ConsultarLista(atividadeTarefa, chamadaAtual).Result;
            }
            else
            {
                atividades = new List<AtividadeTarefaModel>();
            }

            listaChamadaCompleta.Add(completa);

            //Altera a data de recebimento e o status da Chamada para Recebida
            chamadaAtual.Equipamento = equipamento;
            chamadaAtual.Operador = operador;

            if (chamadaAtual.FgStatus == StatusChamada.Aguardando)
            {
                ChamadaModel? chamadaConf = new();

                try
                {
                    chamadaConf = _chamadaRepository.Consultar(chamadaAtual).Result;

                    if (chamadaConf != null && chamadaConf.FgStatus == StatusChamada.Aguardando)
                    {
                        chamadaAtual.FgStatus = StatusChamada.Recebido;
                        _chamadaRepository.AtribuirChamada(chamadaAtual);

                        // Altera a localização atual do equipamento que recebeu a chamada
                        if ((chamadaAtual.AreaArmazenagemOrigem != null) && (chamadaAtual.AreaArmazenagemOrigem.IdAreaArmazenagem > 0))
                        {
                            var areaArmazenagemAux = SiagAPI.GetAreaArmazenagemByIdAsync(chamadaAtual.AreaArmazenagemOrigem.IdAreaArmazenagem).Result;

                            if (areaArmazenagemAux != null)
                            {
                                EnderecoModel endereco = new()
                                {
                                    IdEndereco = areaArmazenagemAux.IdEndereco
                                };

                                _ = SiagAPI.AtualizarEnderecoEquipamentoAsync(equipamento.IdEquipamento, endereco.IdEndereco).Result;
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
                if (tarefa.FgRecurso == Recursos.Automatico)
                {
                    if (executar)
                    {
                        executar = false; // a efetivaLeitura() executa recursivamente as tarefas automáticas seguintes
                        _ = IniciarTarefa(chamadaAtual.IdChamada.ToString(), tarefa.IdTarefa);
                        _ = EfetivaLeitura("", "", chamadaAtual.IdChamada.ToString(), tarefa.IdTarefa);
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
                _atividadeTarefaRepository.AjustaMensagens(chamadaAtual ?? new(), completa.Tarefas);
            }

            if (chamadaAtual == null)
            {
                return null;
            }

            completa.IdChamada = chamadaAtual.IdChamada;
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
            completa.FgStatus = chamadaAtual.FgStatus;
            completa.DtChamada = chamadaAtual.DtChamada;
            completa.DtRecebida = chamadaAtual.DtRecebida;
            completa.DtAtendida = chamadaAtual.DtAtendida;
            completa.DtFinalizada = chamadaAtual.DtFinalizada;
            completa.DtRejeitada = chamadaAtual.DtRejeitada;

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
                    listaChamadaCompleta.Clear();
                }
            }
        }

        return listaChamadaCompleta;
    }

    public async Task<bool> IniciarTarefa(string idChamada, int idTarefa)
    {
        ChamadaModel? chamada = new()
        {
            IdChamada = new Guid(idChamada)
        };

        chamada = await _chamadaRepository.Consultar(chamada);

        List<StatusChamada> statusAux = new() {
            StatusChamada.Dependente,
            StatusChamada.Aguardando,
            StatusChamada.Recebido
        };

        if (chamada == null)
        {
            return false;
        }

        if (statusAux.Contains(chamada.FgStatus))
        {
            chamada.FgStatus = StatusChamada.Andamento;
            await SiagAPI.AlterarStatusChamadaAsync(chamada.IdChamada, chamada.FgStatus);
        }

        //Alteração da tarefa da chamada atualizando campo data inicio                
        AtividadeTarefaModel atividadeTarefa = new()
        {
            IdTarefa = idTarefa
        };

        ChamadaTarefaModel chamadaTarefa = new()
        {
            Chamada = chamada,
            Tarefa = atividadeTarefa,
            DataInicio = DateTime.Now
        };

        await SiagAPI.UpdateChamadaTarefaAsync(chamadaTarefa);

        return true;
    }

    public async Task<bool> RejeitarTarefa(long cracha, int idMotivo, string? idChamada = null)
    {

        if (idChamada != null && idMotivo > 0)
        {
            //Se código da chamada for nulo rejeita todas chamdas do operador
            OperadorModel operador = new()
            {
                IdOperador = cracha
            };

            ChamadaModel? chamada = new()
            {
                IdChamada = new Guid(idChamada),
                Operador = operador,
            };

            chamada = await _chamadaRepository.Consultar(chamada);

            if (chamada == null)
            {
                return false;
            }

            chamada.Atividade = await SiagAPI.GetAtividadeByIdAsync(chamada.IdAtividade);

            if (chamada.Atividade == null)
            {
                return false;
            }

            if (chamada.Atividade.FgPermiteRejeitar == RejeicaoTarefa.Permite)
            {
                chamada.AtividadeRejeicao = new()
                {
                    IdAtividadeRejeicao = idMotivo
                };

                return await SiagAPI.RejeitarChamadaAsync(chamada.IdChamada);
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

    public async Task<MensagemWebservice> EfetivaLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa)
    {
        try
        {
            ChamadaModel? chamada = new()
            {
                IdChamada = new Guid(idChamada)
            };

            chamada = await _chamadaRepository.Consultar(chamada);

            bool leituraAlterada = false;

            if (chamada == null)
            {
                return new MensagemWebservice()
                {
                    Retorno = true,
                    Mensagem = ""
                };
            }

            if (chamada.FgStatus != StatusChamada.Rejeitado && chamada.FgStatus != StatusChamada.Finalizado)
            {
                AreaArmazenagemModel? areaArmazenagemLeitura = chamada.AreaArmazenagemLeitura;
                PalletModel? palletLeitura = chamada.PalletLeitura;

                if (!identificadorPallet.IsNullOrEmpty() && identificadorPallet != "0")
                {
                    chamada.PalletLeitura = await SiagAPI.GetPalletByIdenfificadorAsync(identificadorPallet ?? "");

                    if (chamada.PalletLeitura == null)
                    {
                        try
                        {

                            PalletModel pallet = new()
                            {
                                IdPallet = Convert.ToInt32(identificadorPallet?[3..]),
                                FgStatus = StatusPallet.Livre,
                                CdIdentificacao = identificadorPallet ?? "",
                                QtUtilizacao = 0
                            };

                            await _palletRepository.Inserir(pallet);

                            chamada.PalletLeitura = pallet;
                        }
                        catch (Exception)
                        {
                            return FormatarMensagem(false, "Erro ao cadastrar o pallet " + identificadorPallet);
                        }

                    }

                    // verifica se houve mudança na leitura da area
                    if (((palletLeitura == null) && (chamada.PalletLeitura != null)) ||
                        ((palletLeitura != null) && (chamada.PalletLeitura == null)) ||
                        ((palletLeitura != null) && (!palletLeitura.IdPallet.Equals(chamada?.PalletLeitura?.IdPallet))))
                    {
                        leituraAlterada = true;
                    }
                }

                //Armazena codigos lidos no objeto da chamada
                if (!identificadorAreaArmazenagem.IsNullOrEmpty() && identificadorAreaArmazenagem != "0")
                {
                    chamada.AreaArmazenagemLeitura = await SiagAPI.GetAreaArmazenagemByIdentificadorAsync(identificadorAreaArmazenagem ?? "");

                    if (chamada.AreaArmazenagemLeitura == null)
                    {
                        AreaArmazenagemModel areaArmazenagem = new();
                        _ = long.TryParse(identificadorAreaArmazenagem, out long codigoArea);

                        if (codigoArea > 0)
                        {
                            areaArmazenagem.IdAreaArmazenagem = codigoArea;
                            chamada.AreaArmazenagemLeitura = await SiagAPI.GetAreaArmazenagemByIdAsync(areaArmazenagem.IdAreaArmazenagem);
                        }
                    }

                    // verifica se houve mudança na leitura da area
                    if (((areaArmazenagemLeitura == null) && (chamada.AreaArmazenagemLeitura != null)) ||
                        ((areaArmazenagemLeitura != null) && (chamada.AreaArmazenagemLeitura == null)) ||
                        ((areaArmazenagemLeitura != null) && (!areaArmazenagemLeitura.IdAreaArmazenagem.Equals(chamada?.AreaArmazenagemLeitura?.IdAreaArmazenagem ?? 0))))
                    {
                        leituraAlterada = true;
                    }
                }

                //Procura tarefa da chamada pelo idTarefa
                ChamadaTarefaModel? chamadaTarefa = null;
                var tarefas = await _chamadaRepository.ConsultarTarefas(chamada);

                if (idTarefa > 0)
                {
                    foreach (var tarefa in tarefas)
                    {
                        tarefa.Tarefa = await SiagAPI.GetAtividadeTarefaByIdAsync(tarefa.IdTarefa);

                        if (tarefa.Tarefa.IdTarefa == Convert.ToInt32(idTarefa))
                        {
                            chamadaTarefa = tarefa;
                        }
                    }
                }

                if (chamadaTarefa != null && chamadaTarefa.Tarefa != null)
                {
                    //Consulta atividade rotina a partir da atividade tarefa
                    chamadaTarefa.Tarefa.AtividadeRotina = await SiagAPI.GetAtividadeRotinaByIdAsync(chamadaTarefa.Tarefa?.AtividadeRotina?.IdAtividadeRotina ?? 0);

                    //Salva leituras recebidas
                    await SiagAPI.AtualizarLeituraChamadaAsync(new()
                    {
                        IdChamada = chamada?.IdChamada ?? Guid.Empty,
                        IdAreaArmazenagem = chamada?.IdAreaArmazenagemLeitura,
                        IdPallet = chamada?.IdPalletLeitura
                    });

                    //A partir da atividade rotina consulta a stored procedure para que valide as leituras recebidas
                    // Ignora execução caso a data de fim da tarefa esteja preenchida, e não houve alteração na leitura
                    var leituraValidada = _chamadaRepository.ValidaLeitura(chamada, chamadaTarefa.Tarefa.AtividadeRotina, out string mensagem);

                    if (((chamadaTarefa.DataFim != null) && (!leituraAlterada)) || leituraValidada)
                    {
                        //PROCESSO PARA APAGAR LUZ DO CARACOL USANDO API DO NODERED -- O GURGEL QUE MANDOU
                        try
                        {
                            if (chamada.Atividade != null && chamada.Atividade.IdAtividade == 1)
                            {
                                if (chamada.AreaArmazenagemOrigem != null && chamada.AreaArmazenagemOrigem.IdAreaArmazenagem > 0)
                                {
                                    string area = chamada.AreaArmazenagemOrigem.IdAreaArmazenagem.ToString();

                                    if (area.Length == 9)
                                    {
                                        string caracol = int.Parse(area.Substring(3, 3)).ToString();
                                        string gaiola = int.Parse(area.Substring(6, 2)).ToString();

                                        string url = $"{nodeRedURL}/apagaluzvm/{caracol}/{gaiola}";

                                        try
                                        {
                                            await WebRequestUtil.GetRequest(url);
                                        }
                                        catch (Exception ex)
                                        {
                                            string mensagemLog = "APAGARLUZ=ERROAPI;" + identificadorPallet + ";" + identificadorAreaArmazenagem + ";" + idChamada + ";" + idTarefa + ";" + caracol + ";" + gaiola + ";" + ex.Message;
                                            await SiagAPI.InsertLogAsync(mensagemLog);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string mensagemLog = "APAGARLUZ=ERRO;" + identificadorPallet + ";" + identificadorAreaArmazenagem + ";" + idChamada + ";" + idTarefa + ";" + ex.Message;
                            await SiagAPI.InsertLogAsync(mensagemLog);
                        }

                        chamadaTarefa.DataFim = DateTime.Now;
                        _chamadaRepository.EditarTarefa(chamadaTarefa);

                        await SiagAPI.AtualizarEnderecoEquipamentoAsync(chamada.IdEquipamento);

                        //Só finalizar se todas as tarefas estiverem concluidas
                        bool finalizarChamada = true;

                        foreach (var tarefa in tarefas)
                        {
                            //se alguma tarefa não estiver finalizada retorna false
                            if (tarefa.DataFim == null)
                            {
                                finalizarChamada = false;
                                break;
                            }
                        }

                        //se todas as taredas estiverem finalizadas edita chamada alterando status para finalizada (Isto poreria ser feito dentro do agendamento da proxima tarefa)
                        if (finalizarChamada)
                        {
                            await SiagAPI.FinalizarChamadaAsync(chamada.IdChamada);
                        }
                        else
                        {
                            bool executar = false;

                            foreach (var tarefa in tarefas)
                            {
                                if (tarefa?.Tarefa?.IdTarefa == chamadaTarefa.Tarefa.IdTarefa)
                                {
                                    executar = true;
                                }
                                else if (executar)
                                {
                                    if (tarefa?.Tarefa?.FgRecurso == Recursos.Automatico)
                                    {
                                        await IniciarTarefa(chamada.IdChamada.ToString(), tarefa.Tarefa.IdTarefa);

                                        MensagemWebservice msg = await EfetivaLeitura("", "", chamada.IdChamada.ToString(), tarefa.Tarefa.IdTarefa);

                                        if (!msg.Retorno)
                                        {
                                            throw new Exception(msg.Mensagem);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        // Retorna mensagem para o cliente
                        return FormatarMensagem(true, mensagem);
                    }
                    else
                    {
                        //Se leitura estiver inválida
                        return FormatarMensagem(false, mensagem);
                    }
                }
                else
                {
                    //se não achar id da tarefa
                    return FormatarMensagem(false, "Tarefa não encontrada");
                }
            }
            else
            {
                //Retorna falso se tarefa já estiver rejeitada ou finalizada
                return FormatarMensagem(true, "");
            }
        }
        catch (Exception ex)
        {
            return FormatarMensagem(false, ex.Message);
        }
    }

    private static MensagemWebservice FormatarMensagem(bool retorno, string mensagem)
    {
        return new MensagemWebservice()
        {
            Retorno = retorno,
            Mensagem = mensagem
        };
    }
}
