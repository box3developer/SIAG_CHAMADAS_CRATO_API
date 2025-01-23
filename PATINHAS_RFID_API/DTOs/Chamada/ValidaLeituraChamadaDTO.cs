using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Chamada;

namespace PATINHAS_RFID_API.DTOs.Chamada;

public class ValidaLeituraChamadaDTO
{
    public ChamadaModel? Chamada { get; set; }
    public AtividadeRotinaModel? AtividadeRotina { get; set; }
}
