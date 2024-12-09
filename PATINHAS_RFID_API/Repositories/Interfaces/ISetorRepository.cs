using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface ISetorRepository
    {
        public Task<SetorModel> Consultar(SetorModel setor);
    }
}
