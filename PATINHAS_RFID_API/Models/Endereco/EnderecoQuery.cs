namespace PATINHAS_RFID_API.Models.Endereco
{
    public class EnderecoQuery
    {
        public int id_endereco { get; set; }
        public int id_regiaotrabalho { get; set; }
        public int id_setortrabalho { get; set; }
        public int id_tipoendereco { get; set; }
        public string nm_endereco { get; set; }
        public int qt_estoqueminimo { get; set; }
        public int qt_estoquemaximo { get; set; }
        public int fg_status { get; set; }
        public int tp_preenchimento { get; set; }
    }
}
