using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;

namespace PATINHAS_RFID_API.Models.Chamada;

public class ChamadaModel
{
    public Guid IdChamada { get; set; }
    public int IdPalletOrigem { get; set; }
    public PalletModel? PalletOrigem { get; set; }
    public int IdPalletDestino { get; set; }
    public PalletModel? PalletDestino { get; set; }
    public int IdPalletLeitura { get; set; }
    public PalletModel? PalletLeitura { get; set; }
    public long IdAreaArmazenagemOrigem { get; set; }
    public AreaArmazenagemModel? AreaArmazenagemOrigem { get; set; }
    public long IdAreaArmazenagemDestino { get; set; }
    public AreaArmazenagemModel? AreaArmazenagemDestino { get; set; }
    public long IdAreaArmazenagemLeitura { get; set; }
    public AreaArmazenagemModel? AreaArmazenagemLeitura { get; set; }
    public long IdOperador { get; set; }
    public OperadorModel? Operador { get; set; }
    public int IdEquipamento { get; set; }
    public EquipamentoModel? Equipamento { get; set; }
    public int IdAtividadeRejeicao { get; set; }
    public AtividadeRejeicaoModel? AtividadeRejeicao { get; set; }
    public int IdAtividade { get; set; }
    public AtividadeModel? Atividade { get; set; }
    public StatusChamada FgStatus { get; set; }
    public DateTime? DtChamada { get; set; }
    public DateTime? DtRecebida { get; set; }
    public DateTime? DtAtendida { get; set; }
    public DateTime? DtFinalizada { get; set; }
    public DateTime? DtRejeitada { get; set; }
    public DateTime? DtSuspensa { get; set; }
    public Guid IdChamadaSuspensa { get; set; }
}
