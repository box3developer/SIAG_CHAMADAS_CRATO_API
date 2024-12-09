namespace PATINHAS_RFID_API.DTOs
{
    public class SelecionarTarefaDTO
    {
        public long Cracha { get; set; }
        public string IdentificadorEquipamento { get; set; }
        public bool Reconsultar { get; set; }
        public string IpEquipamento { get; set; }
    }
}
