using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.DTOs.Chamada;

public class ChamadaFiltroDTO
{
    public ChamadaModel? Chamada { get; set; }
    public List<StatusChamada> ListaStatusChamada { get; set; } = new();
}
