using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Models.Equipamento
{
    public class EquipamentoQuery
    {
        public int id_equipamento { get; set; }
        public int id_equipamentomodelo { get; set; }
        public int id_setortrabalho { get; set; }
        public int id_operador { get; set; }
        public string nm_equipamento { get; set; }
        public string nm_abreviado_equipamento { get; set; }
        public string nm_identificador { get; set; }
        public int fg_status { get; set; }
        public DateTime? dt_inclusao { get; set; }
        public DateTime? dt_manutencao { get; set; }
        public DateTime? dt_ultimaleitura { get; set; }
        public int id_endereco { get; set; }
        public string nm_ip { get; set; }
        public int fg_statustrocacaracol { get; set; }
    }
}
