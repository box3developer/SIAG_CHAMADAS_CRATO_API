using PATINHAS_RFID_API.Models.Atividade;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IAtividadeRepository
    {
        public Task<AtividadeModel> Consultar(AtividadeModel atividade);
    }
}
