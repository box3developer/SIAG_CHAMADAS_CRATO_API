using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Services.Interfaces
{
    public interface IOperadorService
    {
        public Task<OperadorModel?> ConsultarOperador(ConsultarOperadorDTO consultarOperadorDTO);
        public Task<bool> Logoff(string cracha, string identificadorEquipamento);
    }
}
