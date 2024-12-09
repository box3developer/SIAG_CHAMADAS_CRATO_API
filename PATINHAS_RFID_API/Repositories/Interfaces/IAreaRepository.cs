using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        public Task<AreaArmazenagemModel> Consultar(AreaArmazenagemModel area);
        public Task<AreaArmazenagemModel> Consultar(string identificador, long codigo = 0);
    }
}
