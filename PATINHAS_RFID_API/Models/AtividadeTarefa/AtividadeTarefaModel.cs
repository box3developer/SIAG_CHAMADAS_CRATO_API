using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRotina;

namespace PATINHAS_RFID_API.Models.AtividadeTarefa;

public class AtividadeTarefaModel
{
    public int IdTarefa { get; set; }
    public string NmTarefa { get; set; } = string.Empty;
    public string NmMensagem { get; set; } = string.Empty;
    public int IdAtividade { get; set; }
    public AtividadeModel? Atividade { get; set; }
    public int CdSequencia { get; set; }
    public Recursos? FgRecurso { get; set; }
    public int IdAtividadeRotina { get; set; }
    public AtividadeRotinaModel? AtividadeRotina { get; set; }
    public int QtPotenciaNormal { get; set; }
    public int QtPotenciaAumentada { get; set; }
}
