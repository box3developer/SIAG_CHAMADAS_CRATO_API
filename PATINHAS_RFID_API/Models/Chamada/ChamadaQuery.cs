namespace PATINHAS_RFID_API.Models.Chamada
{
    public class ChamadaQuery
    {
        public Guid id_chamada { get; set; }
        public int? id_palletorigem { get; set; }
        public int? id_palletdestino { get; set; }
        public int? id_palletleitura { get; set; }
        public int? id_areaarmazenagemorigem { get; set; }
        public int? id_areaarmazenagemdestino { get; set; }
        public int? id_areaarmazenagemleitura { get; set; }
        public int? id_operador { get; set; }
        public int? id_equipamento { get; set; }
        public int? id_atividaderejeicao { get; set; }
        public int? id_atividade { get; set; }
        public int? fg_status { get; set; }
        public DateTime? dt_chamada { get; set; }
        public DateTime? dt_recebida { get; set; }
        public DateTime? dt_atendida { get; set; }
        public DateTime? dt_finalizada { get; set; }
        public DateTime? dt_rejeitada { get; set; }
        public DateTime? DataSuspensa { get; set; }
        public Guid? id_chamadasuspensa { get; set; }
    }
}
