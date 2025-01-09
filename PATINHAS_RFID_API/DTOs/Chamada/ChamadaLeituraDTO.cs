namespace PATINHAS_RFID_API.DTOs.Chamada;

public class ChamadaLeituraDTO
{
    public Guid IdChamada { get; set; }
    public long? IdAreaArmazenagem { get; set; }
    public int? IdPallet { get; set; }
}
