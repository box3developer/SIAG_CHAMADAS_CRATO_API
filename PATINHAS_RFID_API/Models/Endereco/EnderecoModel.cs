using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models.Endereco;

public class EnderecoModel
{
    public int IdEndereco { get; set; }
    public int IdRegiaoTrabalho { get; set; }
    public RegiaoModel? RegiaoTrabalho { get; set; }
    public int IdSetorTrabalho { get; set; }
    public SetorModel? SetorTrabalho { get; set; }
    public int IdTipoEndereco { get; set; }
    public TipoEndereco? TipoEndereco { get; set; }
    public string NmEndereco { get; set; } = string.Empty;
    public int QtEstoqueMinimo { get; set; }
    public int QtEstoqueMaximo { get; set; }
    public StatusEndereco FgStatus { get; set; }
    public TipoPreenchimento TpPreenchimento { get; set; }
}

public class TipoEndereco
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class RegiaoModel
{
    public int Codigo { get; set; }
    public DepositoModel? Deposito { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class SetorModel
{

    public int IdSetorTrabalho { get; set; }
    public int IdDeposito { get; set; }
    public DepositoModel? Deposito { get; set; }
    public string NmSetorTrabalho { get; set; } = string.Empty;
}

public class DepositoModel
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
