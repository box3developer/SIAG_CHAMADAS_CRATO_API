using PATINHAS_RFID_API.Models.AtividadeRejeicao;

namespace PATINHAS_RFID_API.Repositories.Interfaces
{
    public interface IAtividadeRejeicaoRepository
    {
        public Task<List<AtividadeRejeicaoModel>> ConsultarLista(AtividadeRejeicaoModel? atividade);
    }
}
