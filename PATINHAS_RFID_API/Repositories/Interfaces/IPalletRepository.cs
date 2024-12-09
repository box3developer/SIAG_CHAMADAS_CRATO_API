using PATINHAS_RFID_API.Models.Pallet;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IPalletRepository
    {
        public Task<PalletModel> Consultar(string identificador, int codigo = 0);
        public Task<bool> Inserir(PalletModel pallet);
    }
}
