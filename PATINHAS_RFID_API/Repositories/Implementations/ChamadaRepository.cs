using System.Data;
using System.Reflection;
using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Utils;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class ChamadaRepository : IChamadaRepository
{

    const string sqlSelect = "SELECT id_chamada, id_palletorigem, id_areaarmazenagemorigem, id_palletdestino, id_areaarmazenagemdestino, " +
                             " id_palletleitura, id_areaarmazenagemleitura, id_operador, id_equipamento, id_atividaderejeicao, " +
                             " id_atividade, fg_status, dt_chamada, dt_recebida, dt_atendida, dt_finalizada, dt_rejeitada, id_chamadasuspensa " +
                             " FROM chamada WHERE 1 = 1 ";

    const string sqlInsert = "INSERT INTO chamada (id_chamada, id_palletorigem, id_areaarmazenagemorigem, id_palletdestino, id_areaarmazenagemdestino, " +
                             " id_palletleitura, id_areaarmazenagemleitura, id_operador, id_equipamento, id_atividaderejeicao, " +
                             " id_atividade, fg_status, dt_chamada, dt_recebida, dt_atendida, dt_finalizada, dt_rejeitada) " +
                             " VALUES (@Codigo, @Palletorigem, @AreaArmazenagemOrigem, @PalletDestino, @AreaArmazenagemDestino, " +
                             " @PalletLeitura, @AreaArmazenagemLeitura, @Operador, @Equipamento, @AtividadeRejeicao, " +
                             " @Atividade, @Status, @DataChamada, @DataRecebida, @DataAtendida, @DataFinalizada, @DataRejeitada)";

    const string sqlFinalizarChamada = "sp_siag_finalizachamada @idChamada";

    const string sqlRejeitarChamada = "sp_siag_rejeitachamada @idChamada, @idRejeicao";

    private readonly IChamadaTarefaRepository _chamadaTarefaRepository;

    public ChamadaRepository(IChamadaTarefaRepository chamadaTarefaRepository)
    {
        _chamadaTarefaRepository = chamadaTarefaRepository;
    }

    public async Task<ChamadaModel> Consultar(ChamadaModel chamada)
    {
        var chamadaEncontrada = await SiagAPI.GetChamadaByIdAsync(chamada.IdChamada);

        if (chamadaEncontrada == null)
        {
            return null;
        }

        var chamadaFormatada = FormatarOutputChamada(chamadaEncontrada);

        if (chamadaFormatada.PalletOrigem != null)
        {
            var palletO = await SiagAPI.GetPalletByIdAsync(chamadaFormatada.PalletDestino.IdPallet);
            if (palletO != null)
            {
                chamadaFormatada.PalletOrigem = new PalletModel
                {
                    IdPallet = palletO.IdPallet,
                    CdIdentificacao = palletO.CdIdentificacao
                };

            }
            else
            {
                chamadaFormatada.PalletOrigem = null;
            }
        }
        else
        {
            chamadaFormatada.PalletOrigem = null;
        }

        if (chamadaFormatada.PalletDestino != null)
        {
            var palletD = await SiagAPI.GetPalletByIdAsync(chamadaFormatada.PalletDestino.IdPallet);

            if (palletD != null)
            {
                chamadaFormatada.PalletDestino = new PalletModel
                {
                    IdPallet = palletD.IdPallet,
                    CdIdentificacao = palletD.CdIdentificacao,
                };
            }
            else
            {
                chamadaFormatada.PalletDestino = null;
            }
        }
        else
        {
            chamadaFormatada.PalletDestino = null;
        }

        if (chamadaFormatada.AreaArmazenagemOrigem != null)
        {
            var areaO = await SiagAPI.GetAreaArmazenagemByIdAsync(chamadaFormatada.AreaArmazenagemOrigem.IdAreaArmazenagem);

            if (areaO != null)
            {
                chamadaFormatada.AreaArmazenagemOrigem = new AreaArmazenagemModel
                {
                    IdAreaArmazenagem = areaO.IdAreaArmazenagem,
                    CdIdentificacao = areaO.CdIdentificacao
                };
            }
            else
            {
                chamadaFormatada.AreaArmazenagemOrigem = null;
            }
        }
        else
        {
            chamadaFormatada.AreaArmazenagemOrigem = null;
        }

        if (chamadaFormatada.AreaArmazenagemDestino != null)
        {
            var areaD = await SiagAPI.GetAreaArmazenagemByIdAsync(chamadaFormatada.AreaArmazenagemDestino.IdAreaArmazenagem);

            if (areaD != null)
            {
                chamadaFormatada.AreaArmazenagemDestino = new AreaArmazenagemModel
                {
                    IdAreaArmazenagem = areaD.IdAreaArmazenagem,
                    CdIdentificacao = areaD.CdIdentificacao
                };
            }
            else
            {
                chamadaFormatada.AreaArmazenagemDestino = null;
            }
        }
        else
        {
            chamadaFormatada.AreaArmazenagemDestino = null;
        }

        return chamadaFormatada;
    }

    public async Task<ChamadaModel?> SelecionaChamadaEquipamento(ChamadaModel chamada)
    {
        chamada = FormatarInputChamada(chamada);

        //var filtros = new Dictionary<string, object>
        //{
        //    { "@id_operador", chamada.Operador.IdOperador },
        //    { "@id_equipamento", chamada.Equipamento.IdEquipamento }
        //};

        //var parametros = new DynamicParameters(filtros);
        //parametros.Add("@id_chamada", dbType: DbType.Guid, direction: ParameterDirection.Output);

        //var query = "EXEC sp_siag_selecionachamada @id_operador, @id_equipamento, @id_chamada output";

        //using (var conexao = new SqlConnection(Global.Conexao))
        //{
        //    var linhas = await conexao.ExecuteAsync(query, parametros);
        //}

        //Guid? outputValido = parametros.Get<Guid?>("@id_chamada");

        Guid? outputValido = await SiagAPI.SelecionarChamadaAsync(chamada);

        if (Guid.Empty == outputValido || outputValido == null)
        {
            return null;
        }
        else
        {
            chamada.IdChamada = outputValido.Value;
            return await Consultar(chamada);
        }
    }

    public async void AtribuirChamada(ChamadaModel chamada)
    {
        chamada = FormatarInputChamada(chamada);

        await SiagAPI.AtribuirChamadaAsync(chamada);

        //string sql = "UPDATE chamada SET " +
        //    " fg_status = @Status, id_operador = @Operador, id_equipamento = @Equipamento, dt_recebida = GETDATE() " +
        //    " WHERE id_chamada = @Codigo AND fg_status < @StatusRejeitada";

        //using (var conexao = new SqlConnection(Global.Conexao))
        //{
        //    var equipamentoAtualizado = await conexao.ExecuteAsync(sql, new
        //    {
        //        Codigo = chamada.IdChamada,
        //        Operador = chamada.Operador.IdOperador,
        //        Equipamento = chamada.Equipamento.IdEquipamento,
        //        Status = chamada.FgStatus,
        //        StatusRejeitada = StatusChamada.Rejeitado
        //    });
        //}
    }

    public async Task EditarStatus(ChamadaModel chamada)
    {
        string sql = "UPDATE chamada SET fg_status = @Status ";
        switch (chamada.FgStatus)
        {
            case StatusChamada.Recebido:
                sql += ", dt_recebida = GETDATE() ";
                break;
            case StatusChamada.Andamento:
                sql += ", dt_atendida = GETDATE() ";
                break;
        }

        sql += " WHERE id_chamada = @Codigo";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var equipamentoAtualizado = await conexao.ExecuteAsync(sql, new
            {
                Codigo = chamada.IdChamada,
                Status = chamada.FgStatus,
            });
        }
    }

    public async Task<List<ChamadaTarefaModel>> ConsultarTarefas(ChamadaModel? chamada = null)
    {
        ChamadaTarefaModel chamadaTarefa = new ChamadaTarefaModel();
        chamadaTarefa.Chamada = chamada;

        return await _chamadaTarefaRepository.ConsultarLista(chamadaTarefa);
    }

    private static bool ValidaObjeto(object obj)
    {
        if (obj != null)
        {
            PropertyInfo Propriedade = obj.GetType().GetProperty("Codigo");
            return (!Propriedade.GetValue(obj).ToString().Equals("0"));
        }
        return false;
    }

    public async Task<bool> AtualizaLeitura(ChamadaModel chamada)
    {
        var filtros = new Dictionary<string, object>();

        filtros.Add("@Codigo", chamada.IdChamada);

        var update = new List<string>();

        if (chamada.PalletLeitura != null)
        {
            filtros.Add("@PalletLeitura", chamada.PalletLeitura.IdPallet);
            update.Add("id_palletleitura = @PalletLeitura");
        }
        if (chamada.AreaArmazenagemLeitura != null)
        {
            filtros.Add("@AreaArmazenagemLeitura", chamada.AreaArmazenagemLeitura.IdAreaArmazenagem);
            update.Add("id_areaarmazenagemleitura = @AreaArmazenagemLeitura");

        }
        if (update.Count == 0)
        {
            throw new Exception("Nenhum parametro informado");
        }

        var sql = $"UPDATE chamada set {string.Join(',', update)} WHERE id_chamada = @Codigo";

        var parametros = new DynamicParameters(filtros);

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var chamadaEncontrada = await conexao.ExecuteAsync(sql, parametros);
            return true;
        }
    }

    public bool ValidaLeitura(ChamadaModel chamada, AtividadeRotinaModel atividadeRotina, out string mensagem)
    {
        if (atividadeRotina.FgTipo == TipoRotina.MetodoClasse)
        {
            MensagemWebservice? msg = (MensagemWebservice)typeof(IChamadaAtivaRepository).GetMethod(atividadeRotina.NmProcedure).Invoke(_chamadaTarefaRepository, new[] { chamada });
            mensagem = msg.Mensagem;
            return msg.Retorno;
        }
        else
        {
            if (chamada == null)
            {
                throw new Exception("Deve ser informado a chamada referente.");
            }

            var filtros = new Dictionary<string, object>();
            filtros.Add("@id_chamada", chamada.IdChamada);

            var parametros = new DynamicParameters(filtros);
            parametros.Add("@mensagem", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            parametros.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            var query = $"{atividadeRotina.NmProcedure}";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = conexao.Execute(query, parametros, commandType: CommandType.StoredProcedure);
            }

            String? outputMsg = parametros.Get<String?>("@mensagem");
            Int32? outputIdParam = parametros.Get<Int32?>("@RetVal");

            mensagem = (string)outputMsg;
            return ((int)outputIdParam.Value > 0);
        }
    }

    public async void EditarTarefa(ChamadaTarefaModel chamadaTarefa)
    {
        if (chamadaTarefa == null)
        {
            throw new Exception("É obrigatório informar uma Chamada Tarefa!");
        }

        await SiagAPI.UpdateChamadaTarefaAsync(chamadaTarefa);
    }

    public async Task<bool> FinalizarChamada(ChamadaModel chamada)
    {
        if (chamada == null)
        {
            throw new Exception("Deve ser informado a chamada referente.");
        }

        string sql = $"exec {sqlFinalizarChamada}";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var resultado = await conexao.ExecuteAsync(sql, new
            {
                idChamada = chamada.IdChamada,
            });

            return true;
        }
    }

    public async Task<bool> RejeitarChamada(ChamadaModel chamada)
    {
        if (chamada == null)
        {
            throw new Exception("Deve ser informado a chamada referente.");
        }

        int? idRejeicao = null;

        if (chamada.AtividadeRejeicao != null)
        {
            idRejeicao = chamada.AtividadeRejeicao.IdAtividadeRejeicao;
        }

        string sql = $"exec {sqlRejeitarChamada}";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var resultado = await conexao.ExecuteAsync(sql, new
            {
                idChamada = chamada.IdChamada,
                idRejeicao = idRejeicao,
            });

            return resultado > 0;
        }
    }

    public async void ReiniciarChamada(ChamadaModel chamada)
    {
        var query = "EXEC sp_siag_reiniciachamada @id_chamada";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var linhas = await conexao.ExecuteAsync(query, new
            {
                id_chamada = chamada.IdChamada,
            });
        }
    }

    public async Task<List<ChamadaModel>> ConsultarLista(ChamadaModel? chamada = null, List<StatusChamada>? listaStatus = null)
    {
        string sql = sqlSelect;

        var filtros = new Dictionary<string, object>();

        if (chamada != null)
        {
            if ((chamada.IdChamada != null) && (chamada.IdChamada != Guid.Empty))
            {
                sql += " AND id_chamada = @Codigo ";
                filtros.Add("@Codigo", chamada.IdChamada);
            }
            if ((chamada.IdChamadaSuspensa != null) && (chamada.IdChamadaSuspensa != Guid.Empty))
            {
                sql += " AND id_chamadasuspensa = @CodigoSuspensa ";
                filtros.Add("@CodigoSuspensa", chamada.IdChamadaSuspensa);
            }
            if ((chamada.PalletOrigem != null) && (chamada.PalletOrigem.IdPallet != 0))
            {
                sql += " AND id_palletorigem = @PalletOrigem ";
                filtros.Add("@PalletOrigem", chamada.PalletOrigem.IdPallet);
            }
            if ((chamada.PalletDestino != null) && (chamada.PalletDestino.IdPallet != 0))
            {
                sql += " AND id_palletdestino = @PalletDestino ";
                filtros.Add("@PalletDestino", chamada.PalletDestino.IdPallet);
            }
            if ((chamada.PalletLeitura != null) && (chamada.PalletLeitura.IdPallet != 0))
            {
                sql += " AND id_palletleitura = @PalletLeitura ";
                filtros.Add("@PalletLeitura", chamada.PalletLeitura.IdPallet);
            }
            if ((chamada.AreaArmazenagemOrigem != null) && (chamada.AreaArmazenagemOrigem.IdAreaArmazenagem != 0))
            {
                sql += " AND id_areaarmazenagemorigem = @AreaArmazenagemOrigem ";
                filtros.Add("@AreaArmazenagemOrigem", chamada.AreaArmazenagemOrigem.IdAreaArmazenagem);
            }
            if ((chamada.AreaArmazenagemDestino != null) && (chamada.AreaArmazenagemDestino.IdAreaArmazenagem != 0))
            {
                sql += " AND id_areaarmazenagemdestino = @AreaArmazenagemDestino ";
                filtros.Add("@AreaArmazenagemDestino", chamada.AreaArmazenagemDestino.IdAreaArmazenagem);
            }
            if ((chamada.AreaArmazenagemLeitura != null) && (chamada.AreaArmazenagemLeitura.IdAreaArmazenagem != 0))
            {
                sql += " AND id_areaarmazenagemleitura = @AreaArmazenagemLeitura ";
                filtros.Add("@AreaArmazenagemLeitura", chamada.AreaArmazenagemLeitura.IdAreaArmazenagem);
            }
            if ((chamada.Operador != null) && (chamada.Operador.IdOperador != 0))
            {
                sql += " AND id_operador = @Operador ";
                filtros.Add("@Operador", chamada.Operador.IdOperador);
            }
            if ((chamada.Equipamento != null) && (chamada.Equipamento.IdEquipamento != 0))
            {
                sql += " AND id_equipamento = @Equipamento ";
                filtros.Add("@Equipamento", chamada.Equipamento.IdEquipamento);
            }
            if ((chamada.AtividadeRejeicao != null) && (chamada.AtividadeRejeicao.IdAtividadeRejeicao != 0))
            {
                sql += " AND id_atividaderejeicao = @AtividadeRejeicao ";
                filtros.Add("@AtividadeRejeicao", chamada.AtividadeRejeicao.IdAtividadeRejeicao);
            }
            if ((chamada.Atividade != null) && (chamada.Atividade.IdAtividade != 0))
            {
                sql += " AND id_atividade = @Atividade ";
                filtros.Add("@Atividade", chamada.Atividade.IdAtividade);
            }
        }

        if ((listaStatus != null) && (listaStatus.Count > 0))
        {
            var listaStatusEnumToString = string.Join(",", listaStatus.Select(s => (int)s).ToArray()); // listEnumToString(listaStatus)
            sql += " AND fg_status in (" + listaStatusEnumToString + ")";
        }

        var parameters = new DynamicParameters(filtros);


        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var lista = (await conexao.QueryAsync<ChamadaQuery>(sql, parameters)).ToList();
            var listaFormatada = new List<ChamadaModel>();

            foreach (var chamadaEncontrada in lista)
            {
                var chamadaFormatada = new ChamadaModel
                {
                    IdChamada = chamadaEncontrada.id_chamada,
                    FgStatus = (StatusChamada)chamadaEncontrada.fg_status,
                    DtChamada = chamadaEncontrada.dt_chamada,
                    DtRecebida = chamadaEncontrada.dt_recebida,
                    DtAtendida = chamadaEncontrada.dt_atendida,
                    DtFinalizada = chamadaEncontrada.dt_finalizada,
                    DtRejeitada = chamadaEncontrada.dt_rejeitada,
                    DtSuspensa = null,
                };

                if (chamadaEncontrada.id_palletorigem != null)
                {
                    chamadaFormatada.PalletOrigem = new PalletModel
                    {
                        IdPallet = chamadaEncontrada.id_palletorigem.Value
                    };
                }

                if (chamadaEncontrada.id_palletdestino != null)
                {
                    chamadaFormatada.PalletDestino = new PalletModel
                    {
                        IdPallet = chamadaEncontrada.id_palletdestino.Value
                    };
                }
                if (chamadaEncontrada.id_palletleitura != null)
                {
                    chamadaFormatada.PalletLeitura = new PalletModel
                    {
                        IdPallet = chamadaEncontrada.id_palletleitura.Value
                    };
                }

                if (chamadaEncontrada.id_areaarmazenagemorigem != null)
                {
                    chamadaFormatada.AreaArmazenagemOrigem = new AreaArmazenagemModel
                    {
                        IdAreaArmazenagem = chamadaEncontrada.id_areaarmazenagemorigem.Value
                    };
                }
                if (chamadaEncontrada.id_areaarmazenagemdestino != null)
                {
                    chamadaFormatada.AreaArmazenagemDestino = new AreaArmazenagemModel
                    {
                        IdAreaArmazenagem = chamadaEncontrada.id_areaarmazenagemdestino.Value
                    };
                }
                if (chamadaEncontrada.id_areaarmazenagemleitura != null)
                {
                    chamadaFormatada.AreaArmazenagemLeitura = new AreaArmazenagemModel
                    {
                        IdAreaArmazenagem = chamadaEncontrada.id_areaarmazenagemleitura.Value
                    };
                }
                if (chamadaEncontrada.id_operador != null)
                {
                    chamadaFormatada.Operador = new OperadorModel
                    {
                        IdOperador = chamadaEncontrada.id_operador.Value
                    };
                }
                if (chamadaEncontrada.id_equipamento != null)
                {
                    chamadaFormatada.Equipamento = new EquipamentoModel
                    {
                        IdEquipamento = chamadaEncontrada.id_equipamento.Value
                    };
                }
                if (chamadaEncontrada.id_atividaderejeicao != null)
                {
                    chamadaFormatada.AtividadeRejeicao = new AtividadeRejeicaoModel
                    {
                        IdAtividadeRejeicao = chamadaEncontrada.id_atividaderejeicao.Value
                    };
                }
                if (chamadaEncontrada.id_atividade != null)
                {
                    chamadaFormatada.Atividade = new AtividadeModel
                    {
                        IdAtividade = chamadaEncontrada.id_atividade.Value
                    };
                }

                if (chamadaEncontrada.id_chamadasuspensa != null)
                {
                    chamadaFormatada.IdChamadaSuspensa = chamadaEncontrada.id_chamadasuspensa.Value;
                }

                listaFormatada.Add(chamadaFormatada);
            }

            return listaFormatada;
        }
    }

    public async Task<List<ChamadaModel>> ConsultarListaNew(ChamadaModel? chamada = null, List<StatusChamada>? listaStatus = null)
    {
        var lista = await SiagAPI.ConsultarChamadaAsync(new()
        {
            Chamada = chamada,
            ListaStatusChamada = listaStatus ?? new()
        });

        var listaFormatada = new List<ChamadaModel>();

        foreach (var chamadaEncontrada in lista)
        {
            var chamadaFormatada = FormatarOutputChamada(chamadaEncontrada);

            listaFormatada.Add(chamadaFormatada);
        }

        return listaFormatada;
    }

    public ChamadaModel FormatarInputChamada(ChamadaModel chamada)
    {
        if (chamada.PalletOrigem != null)
        {
            chamada.IdPalletOrigem = chamada.PalletOrigem.IdPallet;
        }

        if (chamada.PalletDestino != null)
        {
            chamada.IdPalletDestino = chamada.PalletDestino.IdPallet;
        }

        if (chamada.PalletLeitura != null)
        {
            chamada.IdPalletLeitura = chamada.PalletLeitura.IdPallet;
        }

        if (chamada.AreaArmazenagemOrigem != null)
        {
            chamada.IdAreaArmazenagemOrigem = chamada.AreaArmazenagemOrigem.IdAreaArmazenagem;
        }

        if (chamada.AreaArmazenagemDestino != null)
        {
            chamada.IdAreaArmazenagemDestino = chamada.AreaArmazenagemDestino.IdAreaArmazenagem;
        }

        if (chamada.AreaArmazenagemLeitura != null)
        {
            chamada.IdAreaArmazenagemLeitura = chamada.AreaArmazenagemLeitura.IdAreaArmazenagem;
        }

        if (chamada.Operador != null)
        {
            chamada.IdOperador = chamada.Operador.IdOperador;
        }

        if (chamada.Equipamento != null)
        {
            chamada.IdEquipamento = chamada.Equipamento.IdEquipamento;
        }

        if (chamada.AtividadeRejeicao != null)
        {
            chamada.IdAtividadeRejeicao = chamada.AtividadeRejeicao.IdAtividadeRejeicao;
        }

        if (chamada.Atividade != null)
        {
            chamada.IdAtividade = chamada.Atividade.IdAtividade;
        }

        return chamada;
    }

    public ChamadaModel FormatarOutputChamada(ChamadaModel chamada)
    {
        chamada.DtSuspensa = null;

        if (chamada.IdPalletOrigem != 0)
        {
            chamada.PalletOrigem = new()
            {
                IdPallet = chamada.IdPalletOrigem
            };
        }

        if (chamada.IdPalletDestino != 0)
        {
            chamada.PalletDestino = new()
            {
                IdPallet = chamada.IdPalletDestino
            };
        }

        if (chamada.IdPalletLeitura != 0)
        {
            chamada.PalletLeitura = new()
            {
                IdPallet = chamada.IdPalletLeitura
            };
        }

        if (chamada.IdAreaArmazenagemOrigem != 0)
        {
            chamada.AreaArmazenagemOrigem = new()
            {
                IdAreaArmazenagem = chamada.IdAreaArmazenagemOrigem
            };
        }

        if (chamada.IdAreaArmazenagemDestino != 0)
        {
            chamada.AreaArmazenagemDestino = new()
            {
                IdAreaArmazenagem = chamada.IdAreaArmazenagemDestino
            };
        }

        if (chamada.IdAreaArmazenagemLeitura != 0)
        {
            chamada.AreaArmazenagemLeitura = new()
            {
                IdAreaArmazenagem = chamada.IdAreaArmazenagemLeitura
            };
        }

        if (chamada.IdOperador != 0)
        {
            chamada.Operador = new()
            {
                IdOperador = chamada.IdOperador
            };
        }

        if (chamada.IdEquipamento != 0)
        {
            chamada.Equipamento = new()
            {
                IdEquipamento = chamada.IdEquipamento
            };
        }

        if (chamada.IdAtividadeRejeicao != 0)
        {
            chamada.AtividadeRejeicao = new()
            {
                IdAtividadeRejeicao = chamada.IdAtividadeRejeicao
            };
        }

        if (chamada.IdAtividade != 0)
        {
            chamada.Atividade = new()
            {
                IdAtividade = chamada.IdAtividade
            };
        }

        return chamada;
    }
}
