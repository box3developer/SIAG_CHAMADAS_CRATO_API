using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface ICheckListRepository
    {
        public Task<List<EquipamentoChecklistModel>> ConsultarListaPorEquipamento(EquipamentoModel equipamento);
    }
}
