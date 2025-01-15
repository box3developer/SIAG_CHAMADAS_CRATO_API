using Dapper;
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

    private readonly IEnderecoRepository _enderecoRepository;

    public AtividadeTarefaRepository(IEnderecoRepository enderecoRepository)
    {
        _enderecoRepository = enderecoRepository;
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

    public async Task<List<AtividadeTarefaModel>> ConsultarLista(AtividadeTarefaModel? atividade = null, ChamadaModel? chamada = null)
    {
        var atividades = await SiagAPI.GetListaAtividadeTarefaAsync(atividade);

        if (chamada != null)
        {
            AjustaMensagens(chamada, atividades);
        }

        return atividades;
    }

    public async void AjustaMensagens(ChamadaModel chamada, List<AtividadeTarefaModel> atividades)
    {
        if (chamada == null)
        {
            return;
        }

        string CodigoDestino = "";
        string CodigoOrigem = "";

        AreaArmazenagemModel? areaOrigem;
        EnderecoModel? enderecoOrigem;

        AreaArmazenagemModel? areaDestino;
        EnderecoModel? enderecoDestino;


        foreach (var atividade in atividades)
        {
            if (atividade.NmMensagem.Contains('#'))
            {
                if (chamada.AreaArmazenagemDestino != null)
                {
                    CodigoDestino = chamada.AreaArmazenagemDestino.IdAreaArmazenagem.ToString().PadLeft(10, '0');
                }

                if (chamada.AreaArmazenagemOrigem != null)
                {
                    CodigoOrigem = chamada.AreaArmazenagemOrigem.IdAreaArmazenagem.ToString().PadLeft(10, '0');
                }

                var chave = atividade.NmMensagem.Split('#');

                for (int i = 1; i < chave.Length; i += 2)
                {
                    try
                    {
                        if (chave[i].ToLower().Contains("caracoldest"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(4, 3));
                        }
                        else if (chave[i].ToLower().Contains("caracolori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(4, 3));
                        }
                        else if (chave[i].ToLower().Contains("posicaodest"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(7, 2));
                        }
                        else if (chave[i].ToLower().Contains("posicaoori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(7, 2));
                        }
                        else if (chave[i].ToLower().Contains("corredordes"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(2, 3));
                        }
                        else if (chave[i].ToLower().Contains("corredorori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(2, 3));
                        }
                        else if (chave[i].ToLower().Contains("colunades"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(5, 2));
                        }
                        else if (chave[i].ToLower().Contains("colunaori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(5, 2));
                        }
                        else if (chave[i].ToLower().Contains("alturades"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(7, 2));
                        }
                        else if (chave[i].ToLower().Contains("alturaori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(7, 2));
                        }
                        else if (chave[i].ToLower().Contains("ladodes"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoDestino.Substring(9, 1));
                        }
                        else if (chave[i].ToLower().Contains("ladoori"))
                        {
                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", CodigoOrigem.Substring(9, 1));
                        }
                        else if (chave[i].ToLower().Contains("enderecoori"))
                        {
                            areaOrigem = await SiagAPI.GetAreaArmazenagemByIdAsync(chamada.IdAreaArmazenagemOrigem);
                            enderecoOrigem = await _enderecoRepository.GetById(areaOrigem?.IdEndereco ?? 0);

                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", enderecoOrigem?.NmEndereco);
                        }
                        else if (chave[i].ToLower().Contains("enderecodes"))
                        {
                            areaDestino = await SiagAPI.GetAreaArmazenagemByIdAsync(chamada.IdAreaArmazenagemDestino);
                            enderecoDestino = await _enderecoRepository.GetById(areaDestino?.IdEndereco ?? 0);

                            atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", enderecoDestino?.NmEndereco);
                        }
                    }
                    catch (Exception)
                    {
                        atividade.NmMensagem = atividade.NmMensagem.Replace("#" + chave[i] + "#", "?");
                    }

                    atividade.NmTarefa = atividade.NmMensagem;
                }
            }
        }
    }
}
