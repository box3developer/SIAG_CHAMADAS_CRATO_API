using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class CheckListOperadorRepository : ICheckListOperadorRepository
{
    public async Task<bool> Inserir(EquipamentoChecklistOperadorModel checklistOperador)
    {
        await SiagAPI.InsertChecklistOperadorAsync(checklistOperador);

        return true;
    }
}
