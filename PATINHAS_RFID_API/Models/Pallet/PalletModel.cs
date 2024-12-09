using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Models.Pallet
{
    public class PalletModel
    {
        public int Codigo { get; set; }
        public AreaArmazenagemModel AreaArmazenagem { get; set; }
        public AgrupadorAtivoModel Agrupador { get; set; }
        public StatusPallet Status { get; set; }
        public int QtUtilizacao { get; set; }
        public DateTime? DataUltimaMovimentacao { get; set; }
        public string Identificacao { get; set; }
    }
}
