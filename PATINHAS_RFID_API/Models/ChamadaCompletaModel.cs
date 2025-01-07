using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.Models;

public class ChamadaCompletaModel : ChamadaModel
{
    public List<AtividadeTarefaModel> Tarefas { get; set; } = new();
    public bool Concluida { get; set; }
}
