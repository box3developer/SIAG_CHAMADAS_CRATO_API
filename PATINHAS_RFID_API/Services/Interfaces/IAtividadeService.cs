using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Utils;

namespace PATINHAS_RFID_API.Services.Interfaces;

public interface IAtividadeService
{
    public Task<bool> IniciarTarefa(string idChamada, int idTarefa);
    public Task<MensagemWebservice> EfetivaLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa);
    public Task<List<ChamadaCompletaModel>?> SelecionaTarefa(SelecionarTarefaDTO solicitarTarefaDTO);
    public Task<bool> RejeitarTarefa(long Cracha, int idMotivo, string? idChamada = null);
}
