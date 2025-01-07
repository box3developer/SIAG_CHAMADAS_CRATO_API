using PATINHAS_RFID_API.Models.AtividadeRejeicao;

namespace PATINHAS_RFID_API.Models;

public class ConfiguracaoModel
{
    public int TempoAquisicaoTarefas { get; set; } = 5000;
    public int TempoEstabilizacaoLeitura { get; set; } = 1000;
    public bool ConfirmarAceiteTarefa { get; set; } = false;
    public int TamanhoCodigoArmazenagem { get; set; } = 10;
    public int TamanhoCodigoPallet { get; set; } = 6;
    public int OrdemLeitura { get; set; } = 0;
    public bool InformarMotivoRejeicao { get; set; } = true;
    public List<AtividadeRejeicaoModel> MotivosRejeicao { get; set; } = new();
    public EquipamentoModeloModel? ModeloEquipamento { get; set; }
}
