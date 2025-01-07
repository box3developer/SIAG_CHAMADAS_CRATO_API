using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.Operador;

public class OperadorModel
{
    public long IdOperador { get; set; }
    public string NmOperador { get; set; } = string.Empty;
    public string NmCpf { get; set; } = string.Empty;
    public string NFC { get; set; } = string.Empty;
    public Estabelecimentos NrLocalidade { get; set; }
    public FuncaoOperador FgFuncao { get; set; }
    public int IdResponsavel { get; set; }
    public OperadorModel? Responsavel { get; set; }
    public DateTime? DtLogin { get; set; }
}
