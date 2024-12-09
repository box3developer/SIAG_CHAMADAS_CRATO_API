using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IOperadorRepository
    {
        public Task<OperadorModel> Consultar(OperadorModel operador);
    }
}
