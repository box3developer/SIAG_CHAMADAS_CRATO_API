namespace PATINHAS_RFID_API.Models.AtividadeTarefa
{
    public class AtividadeTarefaQuery
    {
        public int id_tarefa { get; set; }
        public string nm_tarefa { get; set; }
        public string nm_mensagem { get; set; }
        public int id_atividade { get; set; }
        public int cd_sequencia { get; set; }
        public int? fg_recurso { get; set; }
        public int id_atividaderotina { get; set; }
        public int qt_potencianormal { get; set; }
        public int qt_potenciaaumentada { get; set; }
    }
}
