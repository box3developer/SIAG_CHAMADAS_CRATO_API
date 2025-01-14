using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperadorController : ControllerBase
{
    private readonly IOperadorService _operadorService;
    public OperadorController(IOperadorService operadorService)
    {
        _operadorService = operadorService;
    }

    [HttpPost("ConsultarOperador")]
    public async Task<ActionResult> ConsultarOperador([FromBody] ConsultarOperadorDTO consultarOperadorDTO)
    {
        try
        {
            var operador = await _operadorService.ConsultarOperador(consultarOperadorDTO);

            return Ok(operador);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("Logoff/{cracha}/{identificadorEquipamento}")]
    public async Task<ActionResult> Logoff(string cracha, string identificadorEquipamento)
    {
        try
        {
            var operador = await _operadorService.Logoff(cracha, identificadorEquipamento);

            return Ok(operador);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
