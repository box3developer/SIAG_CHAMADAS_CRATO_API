using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Models
{
    public class AgrupadorAtivoModel
    {
        public Guid Codigo { get; set; }
        public TipoAgrupamento TipoAgrupamento {get; set;}
        public string Codigo1 {get; set;}
        public string Codigo2 {get; set;}
        public string Codigo3 {get; set;}
        public Int64 Sequencia {get; set;}
        public DateTime DataAgrupador {get; set;}
        public AreaArmazenagemModel AreaArmazenagem {get; set;}
        public StatusAgrupador Status {get; set;}
    }
}
