using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Models.Pallet;

public class PalletModel
{
    public int IdPallet { get; set; }
    public string CdIdentificacao { get; set; } = string.Empty;
    public long? IdAreaArmazenagem { get; set; }
    public AreaArmazenagemModel? AreaArmazenagem { get; set; }
    public Guid IdAgrupador { get; set; }
    public AgrupadorAtivoModel? Agrupador { get; set; }
    public StatusPallet FgStatus { get; set; }
    public int QtUtilizacao { get; set; }
    public DateTime? DtUltimaMovimentacao { get; set; }
}
