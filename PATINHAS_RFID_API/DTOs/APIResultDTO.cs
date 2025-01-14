namespace PATINHAS_RFID_API.DTOs;

public class APIResultDTO<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
    public string? Tipo { get; set; }
}
