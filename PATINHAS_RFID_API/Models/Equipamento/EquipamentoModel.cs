using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Models.Equipamento
{
    public class EquipamentoModel
    {
        public int Codigo { get; set; }
        public EquipamentoModeloModel Modelo { get; set; }
        public SetorModel Setor { get; set; }
        public OperadorModel Operador { get; set; }
        public string Descricao { get; set; }
        public string DescricaoAbreviada { get; set; }
        public string Identificador { get; set; }
        public StatusEquipamento Status { get; set; }
        public DateTime? DataInclusao { get; set; }
        public DateTime? DataManutencao { get; set; }
        public DateTime? DataUltimaLeitura { get; set; }
        public EnderecoModel EnderecoTrabalho { get; set; }
        public string IP { get; set; }
        public Ativo StatusTrocaCaracol { get; set; }
        public string UltimaLeitura { get; set; }
        public string observacao { get; set; }
    }
}
