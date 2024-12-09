using PATINHAS_RFID_API.Models.Endereco;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IEnderecoRepository
    {
        public Task<EnderecoModel> Consultar(EnderecoModel endereco);
    }
}
