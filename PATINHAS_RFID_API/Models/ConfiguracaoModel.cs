using PATINHAS_RFID_API.Models.AtividadeRejeicao;

namespace PATINHAS_RFID_API.Models
{
    public class ConfiguracaoModel
    {
        public int TempoAquisicaoTarefas { get; set; } = 5000;
        public int TempoEstabilizacaoLeitura { get; set; } = 1000;
        public Boolean ConfirmarAceiteTarefa { get; set; } = false;
        public int TamanhoCodigoArmazenagem { get; set; } = 10;
        public int TamanhoCodigoPallet { get; set; } = 6;
        public int OrdemLeitura { get; set; } = 0;
        public Boolean InformarMotivoRejeicao { get; set; } = true;
        public List<AtividadeRejeicaoModel> MotivosRejeicao { get; set; }
        public EquipamentoModeloModel ModeloEquipamento { get; set; }
    }
}
