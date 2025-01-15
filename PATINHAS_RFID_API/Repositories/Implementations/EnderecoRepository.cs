using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class EnderecoRepository : IEnderecoRepository
{
    public async Task<EnderecoModel?> GetById(int idEndereco)
    {
        var endereco = await SiagAPI.GetEnderecoByIdAsync(idEndereco);

        if (endereco == null)
        {
            return null;
        }

        return FormatarEnderecoOutput(endereco);
    }

    private static EnderecoModel FormatarEnderecoOutput(EnderecoModel endereco)
    {
        endereco.TipoEndereco = new()
        {
            Codigo = endereco.IdTipoEndereco
        };

        endereco.SetorTrabalho = new()
        {
            IdSetorTrabalho = endereco.IdSetorTrabalho
        };

        endereco.RegiaoTrabalho = new()
        {
            Codigo = endereco.IdRegiaoTrabalho
        };

        return endereco;
    }
}
