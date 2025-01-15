using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;

namespace PATINHAS_RFID_API.Repositories.Interfaces;

public interface IEquipamentoRepository
{
    public Task<EquipamentoModel?> GetByIdentificador(string identificador);
    public Task<EquipamentoModel> Consultar(string identificador, int? codigo = 0);
    public Task AtualizaMovimentacao(EquipamentoModel equipamento, EnderecoModel? endereco = null);
    public Task<List<EquipamentoModel>> ConsultarLista(EquipamentoModel? equipamento = null, int codInicial = 0, int codFinal = 0);
    public void Editar(EquipamentoModel equipamento);
    public void LogoffOperador(EquipamentoModel equipamento, OperadorModel? operador);
    public void LoginOperador(EquipamentoModel equipamento, OperadorModel? operador);
}
