namespace PATINHAS_RFID_API.Models.Operador
{
    public class OperadorQuery
    {
        public int id_operador { get; set; }
        public string nm_operador { get; set; }
        public DateTime? dt_login { get; set; }
        public string nm_cpf { get; set; }
        public int nr_localidade { get; set; }
        public int fg_funcao { get; set; }
        public int id_responsavel { get; set; }
    }
}
