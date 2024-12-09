using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Endereco;
using System.Data;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IChamadaAtivaRepository
    {
        public Task<List<EnderecoModel>> RetornaEnderecos(int idPallet);
        public Task<int> QuantidadePalletsAgupador(int idEndereco, int idPallet);
        public Task<AreaArmazenagemModel?> RetornaStageInDisponivel(int id_endereco);
        public Task<bool> ReservaArea(AreaArmazenagemModel areaArmazenagem);
        public Task<bool> CriaAreaTransicao(AreaArmazenagemModel areaOrigem, AreaArmazenagemModel areaDestino);
        public Task<AreaArmazenagemModel?> BuscaAreaLivre();
    }
}
