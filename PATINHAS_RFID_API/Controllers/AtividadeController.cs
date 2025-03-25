using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;
using SIAG_CADASTRO_API.Util;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AtividadeController : ControllerCustom
{
    private readonly IAtividadeService _atividadeService;

    public AtividadeController(IAtividadeService atividadeService)
    {
        _atividadeService = atividadeService;
    }

    [HttpPost("ConsultarTarefa")]
    public async Task<ActionResult> ConsultarTarefa([FromBody] SelecionarTarefaDTO selecionarTarefaDTO)
    {
        try
        {
            selecionarTarefaDTO.Reconsultar = true;

            var response = await _atividadeService.SelecionaTarefa(selecionarTarefaDTO);

            return OkResponse(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("SelecionaTarefa")]
    public async Task<ActionResult> SelecionaTarefa([FromBody] SelecionarTarefaDTO selecionaTarefaDTO)
    {
        try
        {
            var response = await _atividadeService.SelecionaTarefa(selecionaTarefaDTO);

            return OkResponse(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("IniciarTarefa/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> IniciarTarefa(string idChamada, int idTarefa)
    {
        try
        {
            var response = await _atividadeService.IniciarTarefa(idChamada, idTarefa);

            return OkResponse(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("RejeitarTarefa/{Cracha}/{idMotivo}/{idChamada}")]
    public async Task<ActionResult> RejeitarTarefa(long Cracha, int idMotivo, string? idChamada)
    {
        try
        {
            var response = await _atividadeService.RejeitarTarefa(Cracha, idMotivo, idChamada);

            return OkResponse(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("EnviarLeitura/{identificadorPallet}/{identificadorAreaArmazenagem}/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> EnviarLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa)
    {
        try
        {
            var response = await _atividadeService.EfetivaLeitura(identificadorPallet, identificadorAreaArmazenagem, idChamada, idTarefa);

            return OkResponse(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}
