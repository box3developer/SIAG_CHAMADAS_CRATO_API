using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.Models.ChamadaTarefa
{
    public class ChamadaTarefaModel
    {
        public AtividadeTarefaModel Tarefa { get; set; }
        public int AtividadeId { get; set; }
        public ChamadaModel Chamada { get; set; }
        public Guid ChamadaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }
}
