using PATINHAS_RFID_API.Models;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface ICheckListOperadorRepository
    {
        public Task<bool> Inserir(EquipamentoChecklistOperadorModel checklistOperador);
    }
}
