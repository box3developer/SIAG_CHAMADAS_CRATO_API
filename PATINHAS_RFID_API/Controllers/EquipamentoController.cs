using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;
using SIAG_CADASTRO_API.Util;
using System.Diagnostics;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipamentoController : ControllerCustom
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

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("ConsultarPerformance/{cracha}")]
    public async Task<ActionResult> ConsultarPerformance(long cracha)
    {
        try
        {
            var result = await _equipamentoService.ConsultarPerformance(cracha);

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("EnviaLocalizacaoEquipamento/{macEquipamento}/{retornoEquipamento}")]
    public async Task<ActionResult> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento)
    {
        try
        {
            var result = await _equipamentoService.EnviaLocalizacaoEquipamento(macEquipamento, retornoEquipamento);

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("GetCheckList/{identificadorEquipamento}")]
    public async Task<ActionResult> GetCheckList(string identificadorEquipamento)
    {
        try
        {
            var result = await _equipamentoService.GetCheckList(identificadorEquipamento);

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("SetCheckList")]
    public async Task<ActionResult> SetCheckList([FromBody] SetCheckListDTO setCheckListDTO)
    {
        try
        {
            var result = await _equipamentoService.SetCheckList(setCheckListDTO);

            return OkResponse(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("KeepAlive")]
    public ActionResult KeepAlive()
    {
        try
        {
            return OkResponse(true);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("Reiniciar")]
    public ActionResult Reiniciar()
    {
        try
        {
            Process.Start("shutdown", "-r -t 0");
            return OkResponse(true);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("Desligar")]
    public ActionResult Desligar()
    {
        try
        {
            Process.Start("shutdown", "-s -t 0");
            return OkResponse(true);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}
