namespace PATINHAS_RFID_API.Models.ChamadaTarefa
{
    public class ChamadaTarefaQuery
    {
        public int id_tarefa { get; set; }
        public Guid id_chamada { get; set; }
        public DateTime? dt_inicio { get; set; }
        public DateTime? dt_fim { get; set; }
    }
}
