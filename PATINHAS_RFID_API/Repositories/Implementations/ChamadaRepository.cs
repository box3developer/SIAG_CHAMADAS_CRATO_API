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

namespace PATINHAS_RFID_API.Repositories.Implementations
{
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
            var chamadaEncontrada = await SiagAPI.GetChamadaById(chamada.Codigo);

            if (chamadaEncontrada == null)
            {
                return null;
            }

            var chamadaFormatada = new ChamadaModel
            {
                Codigo = chamadaEncontrada.Codigo,
                Status = (StatusChamada)chamadaEncontrada.Status,
                DataChamada = chamadaEncontrada.DataChamada,
                DataRecebida = chamadaEncontrada.DataRecebida,
                DataAtendida = chamadaEncontrada.DataRecebida,
                DataFinalizada = chamadaEncontrada.DataFinalizada,
                DataRejeitada = chamadaEncontrada.DataRejeitada,
                DataSuspensa = null
            };

            if (chamadaEncontrada.PalletOrigemId != 0)
            {
                chamadaFormatada.PalletOrigem = new PalletModel
                {
                    Codigo = chamadaEncontrada.PalletOrigemId
                };
            }

            if (chamadaEncontrada.PalletDestinoId != 0)
            {
                chamadaFormatada.PalletDestino = new PalletModel
                {
                    Codigo = chamadaEncontrada.PalletDestinoId
                };
            }
            if (chamadaEncontrada.PalletLeituraId != 0)
            {
                chamadaFormatada.PalletLeitura = new PalletModel
                {
                    Codigo = chamadaEncontrada.PalletLeituraId
                };
            }

            if (chamadaEncontrada.AreaArmazenagemOrigemId != 0)
            {
                chamadaFormatada.AreaArmazenagemOrigem = new AreaArmazenagemModel
                {
                    Codigo = chamadaEncontrada.AreaArmazenagemOrigemId
                };
            }
            if (chamadaEncontrada.AreaArmazenagemDestinoId != 0)
            {
                chamadaFormatada.AreaArmazenagemDestino = new AreaArmazenagemModel
                {
                    Codigo = chamadaEncontrada.AreaArmazenagemDestinoId
                };
            }
            if (chamadaEncontrada.AreaArmazenagemLeituraId != 0)
            {
                chamadaFormatada.AreaArmazenagemLeitura = new AreaArmazenagemModel
                {
                    Codigo = chamadaEncontrada.AreaArmazenagemLeituraId
                };
            }
            if (chamadaEncontrada.OperadorId != 0)
            {
                chamadaFormatada.Operador = new OperadorModel
                {
                    Codigo = chamadaEncontrada.OperadorId
                };
            }
            if (chamadaEncontrada.EquipamentoId != 0)
            {
                chamadaFormatada.Equipamento = new EquipamentoModel
                {
                    Codigo = chamadaEncontrada.EquipamentoId
                };
            }
            if (chamadaEncontrada.AtividadeRejeicaoId != 0)
            {
                chamadaFormatada.AtividadeRejeicao = new AtividadeRejeicaoModel
                {
                    Codigo = chamadaEncontrada.AtividadeRejeicaoId
                };
            }
            if (chamadaEncontrada.AtividadeId != 0)
            {
                chamadaFormatada.Atividade = new AtividadeModel
                {
                    Codigo = chamadaEncontrada.AtividadeId
                };
            }

            if (chamadaEncontrada.CodigoChamadaSuspensa != Guid.Empty)
            {
                chamadaFormatada.CodigoChamadaSuspensa = chamadaEncontrada.CodigoChamadaSuspensa;
            }

            if (chamadaFormatada.PalletOrigem != null)
            {
                var palletO = await SiagAPI.GetPalletById(chamadaFormatada.PalletDestino.Codigo);
                if (palletO != null)
                {
                    chamadaFormatada.PalletOrigem = new PalletModel
                    {
                        Codigo = palletO.Codigo,
                        Identificacao = palletO.Identificacao
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
                var palletD = await SiagAPI.GetPalletById(chamadaFormatada.PalletDestino.Codigo);

                if (palletD != null)
                {
                    chamadaFormatada.PalletDestino = new PalletModel
                    {
                        Codigo = palletD.Codigo,
                        Identificacao = palletD.Identificacao,
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
                var areaO = await SiagAPI.GetAreaArmazenagemByIdAsync(chamadaFormatada.AreaArmazenagemOrigem.Codigo);

                if (areaO != null)
                {
                    chamadaFormatada.AreaArmazenagemOrigem = new AreaArmazenagemModel
                    {
                        Codigo = areaO.Codigo,
                        Identificacao = areaO.Identificacao
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
                var areaD = await SiagAPI.GetAreaArmazenagemByIdAsync(chamadaFormatada.AreaArmazenagemDestino.Codigo);

                if (areaD != null)
                {
                    chamadaFormatada.AreaArmazenagemDestino = new AreaArmazenagemModel
                    {
                        Codigo = areaD.Codigo,
                        Identificacao = areaD.Identificacao
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
            var filtros = new Dictionary<string, object>();
            filtros.Add("@id_operador", chamada.Operador.Codigo);
            filtros.Add("@id_equipamento", chamada.Equipamento.Codigo);

            var parametros = new DynamicParameters(filtros);
            parametros.Add("@id_chamada", dbType: DbType.Guid, direction: ParameterDirection.Output);

            var query = "EXEC sp_siag_selecionachamada @id_operador, @id_equipamento, @id_chamada output";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = await conexao.ExecuteAsync(query, parametros);
            }

            Guid? outputValido = parametros.Get<Guid?>("@id_chamada");

            if (Guid.Empty == outputValido || outputValido == null)
            {
                return null;
            }
            else
            {
                chamada.Codigo = outputValido.Value;
                return await Consultar(chamada);
            }
        }

        public async void AtribuirChamada(ChamadaModel chamada)
        {
            string sql = "UPDATE chamada SET " +
                " fg_status = @Status, id_operador = @Operador, id_equipamento = @Equipamento, dt_recebida = GETDATE() " +
                " WHERE id_chamada = @Codigo AND fg_status < @StatusRejeitada";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.ExecuteAsync(sql, new
                {
                    Codigo = chamada.Codigo,
                    Operador = chamada.Operador.Codigo,
                    Equipamento = chamada.Equipamento.Codigo,
                    Status = chamada.Status,
                    StatusRejeitada = StatusChamada.Rejeitado
                });
            }
        }

        public async Task EditarStatus(ChamadaModel chamada)
        {
            string sql = "UPDATE chamada SET fg_status = @Status ";
            switch (chamada.Status)
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
                    Codigo = chamada.Codigo,
                    Status = chamada.Status,
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

            filtros.Add("@Codigo", chamada.Codigo);

            var update = new List<string>();

            if (chamada.PalletLeitura != null)
            {
                filtros.Add("@PalletLeitura", chamada.PalletLeitura.Codigo);
                update.Add("id_palletleitura = @PalletLeitura");
            }
            if (chamada.AreaArmazenagemLeitura != null)
            {
                filtros.Add("@AreaArmazenagemLeitura", chamada.AreaArmazenagemLeitura.Codigo);
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
            if (atividadeRotina.Tipo == TipoRotina.MetodoClasse)
            {
                MensagemWebservice? msg = (MensagemWebservice)typeof(IChamadaAtivaRepository).GetMethod(atividadeRotina.Procedure).Invoke(_chamadaTarefaRepository, new[] { chamada });
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
                filtros.Add("@id_chamada", chamada.Codigo);

                var parametros = new DynamicParameters(filtros);
                parametros.Add("@mensagem", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
                parametros.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                var query = $"{atividadeRotina.Procedure}";

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

            await SiagAPI.UpdateChamadaTarefa(chamadaTarefa);
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
                    idChamada = chamada.Codigo,
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
                idRejeicao = chamada.AtividadeRejeicao.Codigo;
            }

            string sql = $"exec {sqlRejeitarChamada}";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var resultado = await conexao.ExecuteAsync(sql, new
                {
                    idChamada = chamada.Codigo,
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
                    id_chamada = chamada.Codigo,
                });
            }
        }

        public async Task<List<ChamadaModel>> ConsultarLista(ChamadaModel? chamada = null, List<StatusChamada>? listaStatus = null)
        {
            string sql = sqlSelect;

            var filtros = new Dictionary<string, object>();

            if (chamada != null)
            {
                if ((chamada.Codigo != null) && (chamada.Codigo != Guid.Empty))
                {
                    sql += " AND id_chamada = @Codigo ";
                    filtros.Add("@Codigo", chamada.Codigo);
                }
                if ((chamada.CodigoChamadaSuspensa != null) && (chamada.CodigoChamadaSuspensa != Guid.Empty))
                {
                    sql += " AND id_chamadasuspensa = @CodigoSuspensa ";
                    filtros.Add("@CodigoSuspensa", chamada.CodigoChamadaSuspensa);
                }
                if ((chamada.PalletOrigem != null) && (chamada.PalletOrigem.Codigo != 0))
                {
                    sql += " AND id_palletorigem = @PalletOrigem ";
                    filtros.Add("@PalletOrigem", chamada.PalletOrigem.Codigo);
                }
                if ((chamada.PalletDestino != null) && (chamada.PalletDestino.Codigo != 0))
                {
                    sql += " AND id_palletdestino = @PalletDestino ";
                    filtros.Add("@PalletDestino", chamada.PalletDestino.Codigo);
                }
                if ((chamada.PalletLeitura != null) && (chamada.PalletLeitura.Codigo != 0))
                {
                    sql += " AND id_palletleitura = @PalletLeitura ";
                    filtros.Add("@PalletLeitura", chamada.PalletLeitura.Codigo);
                }
                if ((chamada.AreaArmazenagemOrigem != null) && (chamada.AreaArmazenagemOrigem.Codigo != 0))
                {
                    sql += " AND id_areaarmazenagemorigem = @AreaArmazenagemOrigem ";
                    filtros.Add("@AreaArmazenagemOrigem", chamada.AreaArmazenagemOrigem.Codigo);
                }
                if ((chamada.AreaArmazenagemDestino != null) && (chamada.AreaArmazenagemDestino.Codigo != 0))
                {
                    sql += " AND id_areaarmazenagemdestino = @AreaArmazenagemDestino ";
                    filtros.Add("@AreaArmazenagemDestino", chamada.AreaArmazenagemDestino.Codigo);
                }
                if ((chamada.AreaArmazenagemLeitura != null) && (chamada.AreaArmazenagemLeitura.Codigo != 0))
                {
                    sql += " AND id_areaarmazenagemleitura = @AreaArmazenagemLeitura ";
                    filtros.Add("@AreaArmazenagemLeitura", chamada.AreaArmazenagemLeitura.Codigo);
                }
                if ((chamada.Operador != null) && (chamada.Operador.Codigo != 0))
                {
                    sql += " AND id_operador = @Operador ";
                    filtros.Add("@Operador", chamada.Operador.Codigo);
                }
                if ((chamada.Equipamento != null) && (chamada.Equipamento.Codigo != 0))
                {
                    sql += " AND id_equipamento = @Equipamento ";
                    filtros.Add("@Equipamento", chamada.Equipamento.Codigo);
                }
                if ((chamada.AtividadeRejeicao != null) && (chamada.AtividadeRejeicao.Codigo != 0))
                {
                    sql += " AND id_atividaderejeicao = @AtividadeRejeicao ";
                    filtros.Add("@AtividadeRejeicao", chamada.AtividadeRejeicao.Codigo);
                }
                if ((chamada.Atividade != null) && (chamada.Atividade.Codigo != 0))
                {
                    sql += " AND id_atividade = @Atividade ";
                    filtros.Add("@Atividade", chamada.Atividade.Codigo);
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
                        Codigo = chamadaEncontrada.id_chamada,
                        Status = (StatusChamada)chamadaEncontrada.fg_status,
                        DataChamada = chamadaEncontrada.dt_chamada,
                        DataRecebida = chamadaEncontrada.dt_recebida,
                        DataAtendida = chamadaEncontrada.dt_atendida,
                        DataFinalizada = chamadaEncontrada.dt_finalizada,
                        DataRejeitada = chamadaEncontrada.dt_rejeitada,
                        DataSuspensa = null,
                    };

                    if (chamadaEncontrada.id_palletorigem != null)
                    {
                        chamadaFormatada.PalletOrigem = new PalletModel
                        {
                            Codigo = chamadaEncontrada.id_palletorigem.Value
                        };
                    }

                    if (chamadaEncontrada.id_palletdestino != null)
                    {
                        chamadaFormatada.PalletDestino = new PalletModel
                        {
                            Codigo = chamadaEncontrada.id_palletdestino.Value
                        };
                    }
                    if (chamadaEncontrada.id_palletleitura != null)
                    {
                        chamadaFormatada.PalletLeitura = new PalletModel
                        {
                            Codigo = chamadaEncontrada.id_palletleitura.Value
                        };
                    }

                    if (chamadaEncontrada.id_areaarmazenagemorigem != null)
                    {
                        chamadaFormatada.AreaArmazenagemOrigem = new AreaArmazenagemModel
                        {
                            Codigo = chamadaEncontrada.id_areaarmazenagemorigem.Value
                        };
                    }
                    if (chamadaEncontrada.id_areaarmazenagemdestino != null)
                    {
                        chamadaFormatada.AreaArmazenagemDestino = new AreaArmazenagemModel
                        {
                            Codigo = chamadaEncontrada.id_areaarmazenagemdestino.Value
                        };
                    }
                    if (chamadaEncontrada.id_areaarmazenagemleitura != null)
                    {
                        chamadaFormatada.AreaArmazenagemLeitura = new AreaArmazenagemModel
                        {
                            Codigo = chamadaEncontrada.id_areaarmazenagemleitura.Value
                        };
                    }
                    if (chamadaEncontrada.id_operador != null)
                    {
                        chamadaFormatada.Operador = new OperadorModel
                        {
                            Codigo = chamadaEncontrada.id_operador.Value
                        };
                    }
                    if (chamadaEncontrada.id_equipamento != null)
                    {
                        chamadaFormatada.Equipamento = new EquipamentoModel
                        {
                            Codigo = chamadaEncontrada.id_equipamento.Value
                        };
                    }
                    if (chamadaEncontrada.id_atividaderejeicao != null)
                    {
                        chamadaFormatada.AtividadeRejeicao = new AtividadeRejeicaoModel
                        {
                            Codigo = chamadaEncontrada.id_atividaderejeicao.Value
                        };
                    }
                    if (chamadaEncontrada.id_atividade != null)
                    {
                        chamadaFormatada.Atividade = new AtividadeModel
                        {
                            Codigo = chamadaEncontrada.id_atividade.Value
                        };
                    }

                    if (chamadaEncontrada.id_chamadasuspensa != null)
                    {
                        chamadaFormatada.CodigoChamadaSuspensa = chamadaEncontrada.id_chamadasuspensa.Value;
                    }

                    listaFormatada.Add(chamadaFormatada);
                }

                return listaFormatada;
            }
        }
    }
}
