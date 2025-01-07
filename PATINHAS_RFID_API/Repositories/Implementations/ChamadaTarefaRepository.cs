using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

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
        await SiagAPI.UpdateChamadaTarefa(chamadaTarefa);
    }

    public async Task<List<ChamadaTarefaModel>> ConsultarLista(ChamadaTarefaModel? chamadaTarefa = null)
    {
        var chamadas = new List<ChamadaTarefaModel>();
        if (chamadaTarefa != null)
        {
            chamadas = await SiagAPI.GetChamadasTarefasById(chamadaTarefa.Chamada.IdChamada, chamadaTarefa.Tarefa.IdTarefa);
        }
        else
        {
            chamadas = await SiagAPI.GetListaChamadasTarefas();
        }

        chamadas = chamadas.Select(x => new ChamadaTarefaModel
        {
            Tarefa = new AtividadeTarefaModel
            {
                IdTarefa = x.IdTarefa,
            },
            Chamada = new ChamadaModel
            {
                IdChamada = x.ChamadaId,
            },
            DataInicio = x.DataInicio,
            DataFim = x.DataFim
        }).ToList();

        return chamadas;
    }
}
