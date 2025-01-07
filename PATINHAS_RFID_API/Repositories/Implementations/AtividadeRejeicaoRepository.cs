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
                if (atividade.IdAtividadeRejeicao != 0)
                {
                    sql += " AND id_atividaderejeicao = @Codigo ";
                }
                if (!String.IsNullOrEmpty(atividade.NmAtividadeRejeicao))
                {
                    sql += " AND nm_atividaderejeicao like @Nome ";
                }
                if (!String.IsNullOrEmpty(atividade.NmEmailAlerta))
                {
                    sql += " AND nm_email_alerta = @Email ";
                }
            }

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var listaAtividadeRejeicao = (await conexao.QueryAsync<AtividadeRejeicaoQuery>(sql, new
                {
                    Codigo = atividade?.IdAtividadeRejeicao,
                    Nome = atividade?.NmAtividadeRejeicao,
                    Email = atividade?.NmEmailAlerta
                })).Select(x => new AtividadeRejeicaoModel
                {
                    IdAtividadeRejeicao = x.id_atividaderejeicao,
                    NmAtividadeRejeicao = x.nm_atividaderejeicao,
                    NmEmailAlerta = x.nm_email_alerta,
                }).ToList();

                return listaAtividadeRejeicao;
            }
        }
    }
}
