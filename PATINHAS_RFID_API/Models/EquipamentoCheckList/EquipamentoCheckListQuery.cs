namespace PATINHAS_RFID_API.Models.EquipamentoCheckList
{
    public class EquipamentoCheckListQuery
    {
        public int id_equipamentochecklist { get; set; }
        public int id_equipamentomodelo { get; set; }
        public string nm_descricao { get; set; }
        public int fg_critico { get; set; }
        public int fg_status{ get; set; }
    }
}
