namespace PATINHAS_RFID_API.DTOs.Chamada;

public class ValidaLeituraChamadaRetornoDTO
{
    public bool Valido { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
