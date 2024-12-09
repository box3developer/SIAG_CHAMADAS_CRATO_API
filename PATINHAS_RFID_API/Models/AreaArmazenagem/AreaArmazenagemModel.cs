using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Models.AreaArmazenagem
{
    public class AreaArmazenagemModel
    {
        public long Codigo { get; set; }
        public TipoAreaModel TipoArea { get; set; }
        public EnderecoModel Endereco { get; set; }
        public AgrupadorAtivoModel Agrupador { get; set; }
        public int PosicaoX { get; set; }
        public int PosicaoY { get; set; }
        public int Lado { get; set; }
        public StatusAreaArmazenagem Status { get; set; }
        public string Identificacao { get; set; }
    }
}
