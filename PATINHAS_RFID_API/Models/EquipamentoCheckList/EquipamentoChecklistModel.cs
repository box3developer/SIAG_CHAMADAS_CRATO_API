using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.EquipamentoCheckList;

public class EquipamentoChecklistModel
{
    public int IdEquipamentoChecklist { get; set; }
    public int IdEquipamentoModelo { get; set; }
    public EquipamentoModeloModel? EquipamentoModelo { get; set; }
    public string NmDescricao { get; set; } = string.Empty;
    public bool? FgCritico { get; set; }
    public Status? FgStatus { get; set; }
}
