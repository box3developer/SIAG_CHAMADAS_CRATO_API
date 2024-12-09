using PATINHAS_RFID_API.Models.AreaArmazenagem;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IAreaArmazenagemRepository
    {
        public Task<AreaArmazenagemModel> Consultar(AreaArmazenagemModel areaArmazenagem);
    }
}
