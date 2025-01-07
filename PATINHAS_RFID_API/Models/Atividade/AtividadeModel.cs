using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Models.Atividade;

public class AtividadeModel
{
    public int IdAtividade { get; set; }
    public string NmAtividade { get; set; } = string.Empty;
    public int IdEquipamentoModelo { get; set; }
    public EquipamentoModeloModel? EquipamentoModelo { get; set; }
    public int IdAtividadeRotinaValidacao { get; set; }
    public AtividadeRotinaModel? AtividadeRotinaValidacao { get; set; }
    public int IdAtividadeAnterior { get; set; }
    public AtividadeModel? AtividadeAnterior { get; set; }
    public int IdSetorTrabalho { get; set; }
    public SetorModel? SetorTrabalho { get; set; }
    public RejeicaoTarefa FgPermiteRejeitar { get; set; }
    public TipoAtribuicaoAutomatica FgTipoAtribuicaoAutomatica { get; set; }
    public ConflitoDeEnderecos FgEvitaConflitoEndereco { get; set; }
}
