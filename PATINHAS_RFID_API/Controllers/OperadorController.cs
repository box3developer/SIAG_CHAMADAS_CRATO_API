using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers
{
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
                //return Ok(new OperadorModel
                //{
                //    Codigo = 13832138,
                //    Descricao = "Operador Teste",
                //    DataLogin = DateTime.Now,
                //    CPF = "",
                //    Localidade = Estabelecimentos.Sobral,
                //    FuncaoOperador = FuncaoOperador.OperadorEmpilhadeira,
                //    Responsavel = null
                //});

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
                //return Ok(new OperadorModel
                //{
                //    Codigo = 13832138,
                //    Descricao = "Operador Teste",
                //    DataLogin = DateTime.Now,
                //    CPF = "",
                //    Localidade = Estabelecimentos.Sobral,
                //    FuncaoOperador = FuncaoOperador.OperadorEmpilhadeira,
                //    Responsavel = null
                //});

                var operador = await _operadorService.Logoff(cracha, identificadorEquipamento);
                return Ok(operador);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
