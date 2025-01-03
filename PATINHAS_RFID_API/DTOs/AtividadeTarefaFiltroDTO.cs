namespace PATINHAS_RFID_API.DTOs;

public class AtividadeTarefaFiltroDTO
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public int AtividadeId { get; set; }
    public int Sequencia { get; set; }
    public int Recursos { get; set; }
    public int AtividadeRotinaId { get; set; }
}
