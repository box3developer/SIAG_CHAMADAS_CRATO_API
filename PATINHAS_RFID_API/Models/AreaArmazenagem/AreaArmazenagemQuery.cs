namespace PATINHAS_RFID_API.Models.AreaArmazenagem
{
    public class AreaArmazenagemQuery
    {
        public int id_areaarmazenagem { get; set; }
        public int id_tipoarea { get; set; }
        public int id_endereco { get; set; }
        public Guid id_agrupador { get; set; }
        public int nr_posicaox { get; set; }
        public int nr_posicaoy { get; set; }
        public int nr_lado { get; set; }
        public int fg_status { get; set; }
        public string cd_identificacao { get; set; }
    }
}
