namespace PATINHAS_RFID_API.Models.Atividade
{
    public class AtividadeQuery
    {
        public int id_atividade { get; set; }
        public string nm_atividade { get; set; }
        public int id_equipamentomodelo { get; set; }
        public int fg_permite_rejeitar { get; set; }
        public int id_atividadeanterior { get; set; }
        public int id_setortrabalho { get; set; }
        public int fg_tipoatribuicaoautomatica { get; set; }
        public int id_atividaderotinavalidacao { get; set; }
        public int fg_evitaconflitoendereco { get; set; }
    }
}
