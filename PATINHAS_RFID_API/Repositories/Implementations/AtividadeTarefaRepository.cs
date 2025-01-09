using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class AtividadeTarefaRepository : IAtividadeTarefaRepository
{
    const string sqlSelect = " SELECT id_tarefa, nm_tarefa, nm_mensagem, id_atividade, cd_sequencia, fg_recurso, id_atividaderotina, qt_potencianormal, qt_potenciaaumentada FROM atividadetarefa with(NOLOCK) WHERE 1 = 1 ";
    const string sqlInsert = " INSERT INTO atividadetarefa VALUES (@Tarefa,@Mensagem,@Atividade,@sequencia,@Recurso,@Rotina,@PotenciaNormal,@PotenciaAumentada)";
    const string sqlUpdate = " UPDATE atividadetarefa SET nm_tarefa = @Tarefa, nm_mensagem = @Mensagem, id_atividade = @Atividade, cd_sequencia = @sequencia, fg_recurso = @Recurso, id_atividaderotina = @Rotina, qt_potencianormal = @PotenciaNormal, qt_potenciaaumentada = @PotenciaAumentada WHERE id_tarefa = @idTarefa";
    const string sqlDelete = " DELETE FROM atividadetarefa WHERE id_tarefa = @Tarefa ";
    const string sqlMaxSequencia = "SELECT MAX(cd_sequencia) AS ultimaSequencia FROM atividadetarefa WHERE id_atividade = @Atividade";

    private readonly IAreaRepository _areaRepository;
    private readonly IEnderecoRepository _enderecoRepository;

    public AtividadeTarefaRepository(IAreaRepository areaRepository, IEnderecoRepository enderecoRepository)
    {
        _areaRepository = areaRepository;
        _enderecoRepository = enderecoRepository;
    }

    public async void AjustaMensagens(ChamadaModel chamada, List<AtividadeTarefaModel> atividades)
    {
        if (chamada != null)
        {
            string CodigoDestino;
            string CodigoOrigem;

            AreaArmazenagemModel? areaOrigem = null;
            EnderecoModel? enderecoOrigem = null;

            AreaArmazenagemModel? areaDestino = null;
            EnderecoModel? enderecoDestino = null;


            foreach (AtividadeTarefaModel item in atividades)
            {
                if (item.NmMensagem.Contains("#"))
                {

                    if (chamada.AreaArmazenagemDestino != null)
                    {
                        CodigoDestino = chamada.AreaArmazenagemDestino.IdAreaArmazenagem.ToString().PadLeft(10, '0');
                    }
                    else
                    {
                        CodigoDestino = "";
                    }

                    if (chamada.AreaArmazenagemOrigem != null)
                    {
                        CodigoOrigem = chamada.AreaArmazenagemOrigem.IdAreaArmazenagem.ToString().PadLeft(10, '0');
                    }
                    else
                    {
                        CodigoOrigem = "";
                    }

                    string[] chave = item.NmMensagem.Split('#');
                    for (int i = 1; i < chave.Length; i += 2)
                    {
                        try
                        {
                            if (chave[i].ToLower().Contains("caracoldest"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(4, 3));
                            }
                            else if (chave[i].ToLower().Contains("caracolori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(4, 3));
                            }
                            else if (chave[i].ToLower().Contains("posicaodest"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(7, 2));
                            }
                            else if (chave[i].ToLower().Contains("posicaoori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(7, 2));
                            }
                            else if (chave[i].ToLower().Contains("corredordes"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(2, 3));
                            }
                            else if (chave[i].ToLower().Contains("corredorori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(2, 3));
                            }
                            else if (chave[i].ToLower().Contains("colunades"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(5, 2));
                            }
                            else if (chave[i].ToLower().Contains("colunaori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(5, 2));
                            }
                            else if (chave[i].ToLower().Contains("alturades"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(7, 2));
                            }
                            else if (chave[i].ToLower().Contains("alturaori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(7, 2));
                            }
                            else if (chave[i].ToLower().Contains("ladodes"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(9, 1));
                            }
                            else if (chave[i].ToLower().Contains("ladoori"))
                            {
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(9, 1));
                            }
                            else if (chave[i].ToLower().Contains("enderecoori"))
                            {
                                areaOrigem = await SiagAPI.GetAreaArmazenagemByIdAsync(chamada?.AreaArmazenagemOrigem?.IdAreaArmazenagem ?? 0);
                                enderecoOrigem = await SiagAPI.GetEndereco(areaOrigem.Endereco?.IdEndereco ?? 0);
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", enderecoOrigem.NmEndereco);
                            }
                            else if (chave[i].ToLower().Contains("enderecodes"))
                            {
                                areaDestino = await SiagAPI.GetAreaArmazenagemByIdAsync(chamada?.AreaArmazenagemDestino?.IdAreaArmazenagem ?? 0);
                                enderecoDestino = await SiagAPI.GetEndereco(areaDestino.Endereco?.IdEndereco ?? 0);
                                item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", enderecoDestino.NmEndereco);
                            }
                        }
                        catch (Exception)
                        {
                            item.NmMensagem = item.NmMensagem.Replace("#" + chave[i] + "#", "?");
                        }

                        item.NmTarefa = item.NmMensagem;
                    }
                }
            }
        }
    }

    public async Task<List<AtividadeTarefaModel>> ConsultarLista(AtividadeTarefaModel? atividade = null, ChamadaModel? chamada = null)
    {
        List<AtividadeTarefaModel> atividades = await SiagAPI.GetListaAtividadeTarefa(atividade);
        if (chamada != null)
        {
            AjustaMensagens(chamada, atividades);
        }

        return atividades;
    }

    public async Task<List<AtividadeTarefaModel>> ConsultarLista(AtividadeTarefaModel tarefa)
    {
        string sql = sqlSelect;

        var filtros = new Dictionary<string, object>();

        if (tarefa != null)
        {
            if (tarefa.IdTarefa != 0)
            {
                sql += " AND id_tarefa = @Codigo ";
                filtros.Add("@Codigo", tarefa.IdTarefa);
            }
            if (tarefa.CdSequencia != 0)
            {
                sql += " AND cd_sequencia = @Seq ";
                filtros.Add("@Seq", tarefa.CdSequencia);
            }
            if (tarefa.FgRecurso != null)
            {
                sql += " AND fg_recurso = @Rec ";
                filtros.Add("@Rec", (int)tarefa.FgRecurso);
            }
            if (!String.IsNullOrEmpty(tarefa.NmTarefa))
            {
                sql += " AND nm_tarefa like @Nome ";
                filtros.Add("@Nome", "%" + tarefa.NmTarefa + "%");
            }
            if (tarefa.NmMensagem != null)
            {
                sql += " AND nm_mensagem like @Mensangem ";
                filtros.Add("@Mensangem", "%" + tarefa.NmMensagem + "%");
            }
            if (tarefa.Atividade != null && tarefa.Atividade.IdAtividade != 0)
            {
                sql += " AND id_atividade = @Equipamento ";
                filtros.Add("@Equipamento", tarefa.Atividade.IdAtividade);
            }
            if (tarefa.AtividadeRotina != null && tarefa.AtividadeRotina.IdAtividadeRotina != 0)
            {
                sql += " AND id_atividaderotina = @AtividadeAnterior ";
                filtros.Add("@AtividadeAnterior", tarefa.AtividadeRotina.IdAtividadeRotina);
            }
            sql += " ORDER BY cd_sequencia ";
        }

        var parametros = new DynamicParameters(filtros);

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var atividades = (await conexao.QueryAsync<AtividadeTarefaQuery>(sql, parametros))
                .Select(x => new AtividadeTarefaModel
                {
                    IdTarefa = x.id_tarefa,
                    NmTarefa = x.nm_tarefa,
                    NmMensagem = x.nm_mensagem,
                    Atividade = new AtividadeModel
                    {
                        IdAtividade = x.id_atividade,
                    },
                    CdSequencia = x.cd_sequencia,
                    FgRecurso = (Recursos)x.fg_recurso.Value,
                    AtividadeRotina = new AtividadeRotinaModel
                    {
                        IdAtividadeRotina = x.id_atividaderotina
                    },
                    QtPotenciaNormal = x.qt_potencianormal,
                    QtPotenciaAumentada = x.qt_potenciaaumentada
                }).ToList();

            return atividades;
        }
    }

    public async Task<AtividadeTarefaModel> Consultar(AtividadeTarefaModel tarefa)
    {
        string sql = sqlSelect;
        sql += " AND id_tarefa = @Codigo ";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var atividade = await conexao.QueryFirstOrDefaultAsync<AtividadeTarefaQuery>(sql, new
            {
                Codigo = tarefa.IdTarefa,
            });

            return new AtividadeTarefaModel
            {
                IdTarefa = atividade.id_tarefa,
                NmTarefa = atividade.nm_tarefa,
                NmMensagem = atividade.nm_mensagem,
                Atividade = new AtividadeModel
                {
                    IdAtividade = atividade.id_atividade,
                },
                CdSequencia = atividade.cd_sequencia,
                FgRecurso = (Recursos)atividade.fg_recurso,
                AtividadeRotina = new AtividadeRotinaModel
                {
                    IdAtividadeRotina = atividade.id_atividaderotina
                },
                QtPotenciaNormal = atividade.qt_potencianormal,
                QtPotenciaAumentada = atividade.qt_potenciaaumentada
            };
        }
    }
}
