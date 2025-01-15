using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.Repositories.Interfaces;

public interface IAtividadeTarefaRepository
{
    public Task<List<AtividadeTarefaModel>> ConsultarLista(AtividadeTarefaModel tarefa, ChamadaModel? chamada = null);
    public Task<AtividadeTarefaModel> Consultar(AtividadeTarefaModel tarefa);
    public void AjustaMensagens(ChamadaModel chamada, List<AtividadeTarefaModel> atividades);
}
