using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Util;

namespace SIAG_CADASTRO_API.Util
{
    public class ControllerCustom : ControllerBase
    {
        [NonAction]
        public ActionResult HandleException(Exception ex)
        {
            var result = new APIResultDTO
            {
                Sucesso = false,
                Dados = null,
                Mensagem = ex.Message
            };

            if (ex is ValidacaoException)
            {
                result.Tipo = "warning";
                return BadRequest(result);
            }
            else
            {
                result.Tipo = "error";
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [NonAction]
        public ActionResult OkResponse(object dados, string mensagem = "")
        {
            var result = new APIResultDTO
            {
                Sucesso = true,
                Dados = dados,
                Mensagem = mensagem,
                Tipo = "success"
            };

            return Ok(result);
        }
    }
}
