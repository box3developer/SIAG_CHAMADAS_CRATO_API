using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.AtividadeRotina;

public class AtividadeRotinaModel
{
    public int IdAtividadeRotina { get; set; }
    public string NmAtividadeRotina { get; set; } = string.Empty;
    public string NmProcedure { get; set; } = string.Empty;
    public TipoRotina FgTipo { get; set; }
}
