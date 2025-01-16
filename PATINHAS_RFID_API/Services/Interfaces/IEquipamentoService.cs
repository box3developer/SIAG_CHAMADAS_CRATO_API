using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;

namespace PATINHAS_RFID_API.Services.Interfaces;

public interface IEquipamentoService
{
    public Task<ConfiguracaoModel> ConsultarConfiguracoes(long cracha, string identificadorEquipamento);
    public Task<int> ConsultarPerformance(long cracha);
    public Task<bool> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento);
    public Task<List<EquipamentoChecklistModel>> GetCheckList(string identificadorEquipamento);
    public Task<bool> SetCheckList(SetCheckListDTO setCheckListDTO);
}
