using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;
using SIAG_CADASTRO_API.Util;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperadorController : ControllerCustom
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

            return OkResponse(operador);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("Logoff/{cracha}/{identificadorEquipamento}")]
    public async Task<ActionResult> Logoff(string cracha, string identificadorEquipamento)
    {
        try
        {
            var operador = await _operadorService.Logoff(cracha, identificadorEquipamento);

            return OkResponse(operador);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}
