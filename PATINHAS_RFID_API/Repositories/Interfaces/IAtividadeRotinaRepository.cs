using PATINHAS_RFID_API.Models.AtividadeRotina;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IAtividadeRotinaRepository
    {
        public Task<AtividadeRotinaModel> Consultar(AtividadeRotinaModel rotina);
    }
}
