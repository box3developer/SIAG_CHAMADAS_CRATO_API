using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class ChamadaTarefaRepository : IChamadaTarefaRepository
    {
        const string sqlSelect = "SELECT chamadatarefa.id_chamada, chamadatarefa.id_tarefa, chamadatarefa.dt_inicio, chamadatarefa.dt_fim " +
                         " FROM chamadatarefa with(NOLOCK) " +
                         " INNER JOIN atividadetarefa with(NOLOCK) ON (chamadatarefa.id_tarefa = atividadetarefa.id_tarefa) WHERE 1 = 1 ";

        const string sqlInsert = "INSERT INTO chamadatarefa (id_chamada, id_tarefa, dt_inicio, dt_fim) " +
                                 " VALUES (@Chamada, @Tarefa, @DataInicio, @DataFim)";

        const string sqlUpdate = "UPDATE chamadatarefa SET dt_inicio = @DataInicio, dt_fim = @DataFim " +
                                 " WHERE id_chamada = @Chamada and id_tarefa = @Tarefa";

        public async Task Editar(ChamadaTarefaModel chamadaTarefa)
        {
            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.QueryAsync(sqlUpdate, new
                {
                    Tarefa = chamadaTarefa.Tarefa.Codigo,
                    Chamada = chamadaTarefa.Chamada.Codigo,
                    DataInicio = chamadaTarefa.DataInicio,
                    DataFim = chamadaTarefa.DataFim,
                });
            }
        }

        public async Task<List<ChamadaTarefaModel>> ConsultarLista(ChamadaTarefaModel? chamadaTarefa = null)
        {
            string sql = sqlSelect;

            var filtros = new Dictionary<string, object>();

            if (chamadaTarefa != null)
            {
                if (((chamadaTarefa.Tarefa != null) && (chamadaTarefa.Tarefa.Codigo > 0)))
                {
                    sql += " AND chamadatarefa.id_tarefa = @Tarefa ";
                    filtros.Add("@Tarefa", chamadaTarefa.Tarefa.Codigo);
                }
                if (((chamadaTarefa.Chamada != null) && (chamadaTarefa.Chamada.Codigo != Guid.Empty)))
                {
                    sql += " AND chamadatarefa.id_chamada = @Chamada ";
                    filtros.Add("@Chamada", chamadaTarefa.Chamada.Codigo);
                }

            }
            sql += " ORDER BY atividadetarefa.cd_sequencia ";

            var parametros = new DynamicParameters(filtros);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var chamadas = (await conexao.QueryAsync<ChamadaTarefaQuery>(sql, parametros))
                    .Select(x => new ChamadaTarefaModel
                {
                    Tarefa = new AtividadeTarefaModel
                    {
                        Codigo = x.id_tarefa
                    },
                    Chamada = new ChamadaModel
                    {
                        Codigo = x.id_chamada
                    },
                    DataInicio = x.dt_inicio,
                    DataFim = x.dt_fim
                }).ToList();

                return chamadas;
            }
        }
    }

}
