using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Models.Atividade
{
    public class AtividadeModel
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public EquipamentoModeloModel EquipamentoModelo { get; set; }
        public RejeicaoTarefa PermiteRejeitar { get; set; }
        public AtividadeModel AtividadeAnterior { get; set; }
        public SetorModel SetorTrabalho { get; set; }
        public TipoAtribuicaoAutomatica TipoAtribuicaoAutomatica { get; set; }
        public AtividadeRotinaModel AtividadeRotinaValidacao { get; set; }
        public ConflitoDeEnderecos EvitarConflitoEndereco { get; set; }
    }
}
