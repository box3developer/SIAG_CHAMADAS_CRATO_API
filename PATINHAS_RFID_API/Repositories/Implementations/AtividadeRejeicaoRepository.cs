using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class AtividadeRejeicaoRepository : IAtividadeRejeicaoRepository
    {
        public async Task<List<AtividadeRejeicaoModel>> ConsultarLista(AtividadeRejeicaoModel? atividade)
        {
            const string sqlSelect = " SELECT id_atividaderejeicao, nm_atividaderejeicao, nm_email_alerta FROM atividaderejeicao with(NOLOCK) WHERE 1 = 1 ";

            string sql = sqlSelect;
            if (atividade != null)
            {
                if (atividade.Codigo != 0)
                {
                    sql += " AND id_atividaderejeicao = @Codigo ";
                }
                if (!String.IsNullOrEmpty(atividade.Descricao))
                {
                    sql += " AND nm_atividaderejeicao like @Nome ";
                }
                if (!String.IsNullOrEmpty(atividade.Email))
                {
                    sql += " AND nm_email_alerta = @Email ";
                }
            }

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var listaAtividadeRejeicao = (await conexao.QueryAsync<AtividadeRejeicaoQuery>(sql, new
                {
                    Codigo = atividade?.Codigo,
                    Nome = atividade?.Descricao,
                    Email = atividade?.Email
                })).Select(x => new AtividadeRejeicaoModel
                {
                    Codigo = x.id_atividaderejeicao,
                    Descricao = x.nm_atividaderejeicao,
                    Email = x.nm_email_alerta,
                }).ToList();

                return listaAtividadeRejeicao;
            }
        }
    }
}
