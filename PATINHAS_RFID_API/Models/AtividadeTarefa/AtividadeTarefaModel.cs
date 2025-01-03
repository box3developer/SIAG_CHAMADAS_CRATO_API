using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRotina;

namespace PATINHAS_RFID_API.Models.AtividadeTarefa
{
    public class AtividadeTarefaModel
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Mensagem { get; set; }
        public AtividadeModel Atividade { get; set; }
        public int AtividadeId { get; set; }
        public int Sequencia { get; set; }
        public Recursos? Recursos { get; set; }
        public AtividadeRotinaModel AtividadeRotina { get; set; }
        public int AtividadeRotinaId { get; set; }
        public int PotenciaNormal { get; set; }
        public int PotenciaAumentada { get; set; }
    }
}
