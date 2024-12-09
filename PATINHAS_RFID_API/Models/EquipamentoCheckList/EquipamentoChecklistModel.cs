using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.EquipamentoCheckList
{
    public class EquipamentoChecklistModel
    {
        public int Codigo { get; set; }
        public EquipamentoModeloModel Modelo { get; set; }
        public string Descricao { get; set; }
        public bool Critico { get; set; }
        public Status Status { get; set; }
    }
}
