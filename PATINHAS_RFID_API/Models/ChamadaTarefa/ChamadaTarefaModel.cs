using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.Models.ChamadaTarefa;

public class ChamadaTarefaModel
{
    public int IdTarefa { get; set; }
    public AtividadeTarefaModel? Tarefa { get; set; }
    public Guid IdChamada { get; set; }
    public ChamadaModel? Chamada { get; set; }
    public DateTime? DtInicio { get; set; }
    public DateTime? DtFim { get; set; }
}
