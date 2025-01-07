using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Models.AreaArmazenagem;

public class AreaArmazenagemModel
{
    public long IdAreaArmazenagem { get; set; }
    public int IdTipoArea { get; set; }
    public TipoAreaModel? TipoArea { get; set; }
    public int IdEndereco { get; set; }
    public EnderecoModel? Endereco { get; set; }
    public Guid IdAgrupador { get; set; }
    public AgrupadorAtivoModel? Agrupador { get; set; }
    public string? IdCaracol { get; set; }
    public int NrPosicaoX { get; set; }
    public int NrPosicaoY { get; set; }
    public int NrLado { get; set; }
    public StatusAreaArmazenagem FgStatus { get; set; }
    public string CdIdentificacao { get; set; } = string.Empty;
}
