using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipamentoController : ControllerBase
{
    private readonly IEquipamentoService _equipamentoService;

    public EquipamentoController(IEquipamentoService equipamentoService)
    {
        _equipamentoService = equipamentoService;
    }

    [HttpGet("ConsultarConfiguracoes/{cracha}/{identificadorEquipamento}")]
    public async Task<ActionResult> ConsultarConfiguracoes(long cracha, string identificadorEquipamento)
    {
        try
        {
            var result = await _equipamentoService.ConsultarConfiguracoes(cracha, identificadorEquipamento);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("ConsultarPerformance/{cracha}")]
    public async Task<ActionResult> ConsultarPerformance(long cracha)
    {
        try
        {
            var result = await _equipamentoService.ConsultarPerformance(cracha);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("EnviaLocalizacaoEquipamento/{macEquipamento}/{retornoEquipamento}")]
    public async Task<ActionResult> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento)
    {
        try
        {
            var result = await _equipamentoService.EnviaLocalizacaoEquipamento(macEquipamento, retornoEquipamento);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetCheckList/{identificadorEquipamento}")]
    public async Task<ActionResult> GetCheckList(string identificadorEquipamento)
    {
        try
        {
            var result = await _equipamentoService.GetCheckList(identificadorEquipamento);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("SetCheckList")]
    public async Task<ActionResult> SetCheckList([FromBody] SetCheckListDTO setCheckListDTO)
    {
        try
        {
            var result = await _equipamentoService.SetCheckList(setCheckListDTO);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("KeepAlive")]
    public ActionResult KeepAlive()
    {
        try
        {
            return Ok(true);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
