namespace PATINHAS_RFID_API.DTOs;

public class SetCheckListDTO
{
    public string IdentificadorEquipamento { get; set; }
    public long CodOperador { get; set; }
    public string? ChecklistResponse { get; set; }
}
