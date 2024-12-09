namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface ILogRepository
    {
        public Task<bool> Insere(string mensagem);
    }
}
