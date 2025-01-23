using PATINHAS_RFID_API.Models.ChamadaTarefa;

namespace PATINHAS_RFID_API.Repositories.Interfaces;

public interface IChamadaTarefaRepository
{
    public Task Editar(ChamadaTarefaModel chamadaTarefa);
    public Task<List<ChamadaTarefaModel>> ConsultarLista(ChamadaTarefaModel? chamadaTarefa = null);
}
