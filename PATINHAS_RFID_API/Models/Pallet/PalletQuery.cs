namespace PATINHAS_RFID_API.Models.Pallet
{
    public class PalletQuery
    {
        public int id_pallet { get; set; }
        public int id_areaarmazenagem { get; set; }
        public Guid id_agrupador { get; set; }
        public int fg_status { get; set; }
        public int qt_utilizacao { get; set; }
        public DateTime? dt_ultimamovimentacao { get; set; }
        public string cd_identificacao { get; set; }
    }
}
