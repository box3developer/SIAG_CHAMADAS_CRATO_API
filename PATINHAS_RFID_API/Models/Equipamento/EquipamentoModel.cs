using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Models.Equipamento;

public class EquipamentoModel
{
    public int IdEquipamento { get; set; }
    public int IdEquipamentoModelo { get; set; }
    public EquipamentoModeloModel? EquipamentoModelo { get; set; }
    public int? IdSetorTrabalho { get; set; }
    public SetorModel? SetorTrabalho { get; set; }
    public int IdOperador { get; set; }
    public OperadorModel? Operador { get; set; }
    public string NmEquipamento { get; set; } = string.Empty;
    public string NmAbreviadoEquipamento { get; set; } = string.Empty;
    public string NmIdentificador { get; set; } = string.Empty;
    public StatusEquipamento FgStatus { get; set; }
    public DateTime? DtInclusao { get; set; }
    public DateTime? DtManutencao { get; set; }
    public DateTime? DtUltimaLeitura { get; set; }
    public int? IdEndereco { get; set; }
    public EnderecoModel? Endereco { get; set; }
    public string NmIP { get; set; } = string.Empty;
    public Ativo FgStatusTrocaCaracol { get; set; }
    public string CdLeituraPendente { get; set; } = string.Empty;
    public string CdUltimaLeitura { get; set; } = string.Empty;
    public string Observacao { get; set; } = string.Empty;
}
