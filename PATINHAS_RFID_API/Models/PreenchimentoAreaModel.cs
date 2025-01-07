using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Models;

public class PreenchimentoAreaModel
{
    public int IdPreenchimento { get; set; }
    public List<AreaArmazenagemModel> ListaAreas { get; set; } = new();
}
