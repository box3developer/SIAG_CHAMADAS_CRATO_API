using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.Operador
{
    public class OperadorModel
    {
        public long Codigo { get; set; }
        public string NFC { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataLogin { get; set; }
        public string CPF { get; set; }
        public Estabelecimentos Localidade { get; set; }
        public FuncaoOperador FuncaoOperador { get; set; }
        public OperadorModel Responsavel { get; set; }
    }
}
