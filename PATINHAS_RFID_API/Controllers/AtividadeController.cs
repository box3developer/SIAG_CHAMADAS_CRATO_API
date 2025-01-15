using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AtividadeController : ControllerBase
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

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("SelecionaTarefa")]
    public async Task<ActionResult> SelecionaTarefa([FromBody] SelecionarTarefaDTO selecionaTarefaDTO)
    {
        try
        {
            var response = await _atividadeService.SelecionaTarefa(selecionaTarefaDTO);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("IniciarTarefa/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> IniciarTarefa(string idChamada, int idTarefa)
    {
        try
        {
            var response = await _atividadeService.IniciarTarefa(idChamada, idTarefa);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("RejeitarTarefa/{Cracha}/{idMotivo}/{idChamada}")]
    public async Task<ActionResult> RejeitarTarefa(long Cracha, int idMotivo, string? idChamada)
    {
        try
        {
            var response = await _atividadeService.RejeitarTarefa(Cracha, idMotivo, idChamada);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("EnviarLeitura/{identificadorPallet}/{identificadorAreaArmazenagem}/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> EnviarLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa)
    {
        try
        {
            var response = await _atividadeService.EfetivaLeitura(identificadorPallet, identificadorAreaArmazenagem, idChamada, idTarefa);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
