using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Models;

public class EquipamentoChecklistOperadorModel
{
    public EquipamentoModel? Equipamento { get; set; }
    public OperadorModel? Operador { get; set; }
    public EquipamentoChecklistModel? Checklist { get; set; }
    public bool Resposta { get; set; }
    public DateTime? Data { get; set; }
}
