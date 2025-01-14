using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;

namespace PATINHAS_RFID_API.Repositories.Interfaces;

public interface IChamadaRepository
{
    public Task<ChamadaModel> Consultar(ChamadaModel chamada);
    public Task<ChamadaModel?> SelecionaChamadaEquipamento(ChamadaModel chamada);
    public void AtribuirChamada(ChamadaModel chamada);
    public Task EditarStatus(ChamadaModel chamada);
    public Task<List<ChamadaTarefaModel>> ConsultarTarefas(ChamadaModel? chamada = null);
    public Task<bool> AtualizaLeitura(ChamadaModel chamada);
    public bool ValidaLeitura(ChamadaModel chamada, AtividadeRotinaModel atividadeRotina, out string mensagem);
    public void EditarTarefa(ChamadaTarefaModel chamadaTarefa);
    public Task<bool> FinalizarChamada(ChamadaModel chamada);
    public Task<bool> RejeitarChamada(ChamadaModel chamada);
    public void ReiniciarChamada(ChamadaModel chamada);
    public Task<List<ChamadaModel>> ConsultarLista(ChamadaModel? chamada = null, List<StatusChamada>? listaStatus = null);
    public Task<List<ChamadaModel>> ConsultarListaNew(ChamadaModel? chamada = null, List<StatusChamada>? listaStatus = null);
}
