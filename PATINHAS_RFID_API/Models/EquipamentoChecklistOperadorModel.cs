using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Models;

public class EquipamentoChecklistOperadorModel
{
    public int IdEquipamento { get; set; }
    public EquipamentoModel? Equipamento { get; set; }
    public long IdOperador { get; set; }
    public OperadorModel? Operador { get; set; }
    public int IdEquipamentoChecklist { get; set; }
    public EquipamentoChecklistModel? Checklist { get; set; }
    public bool FgResposta { get; set; }
    public DateTime? DtChecklist { get; set; }
}
