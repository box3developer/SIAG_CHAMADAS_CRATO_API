using Dapper;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class AtividadeRotinaRepository : IAtividadeRotinaRepository
{
    const string sqlSelect = " SELECT id_atividadeRotina, nm_AtividadeRotina, nm_procedure, fg_tipo FROM atividaderotina with(NOLOCK) WHERE 1 = 1 ";

    public async Task<AtividadeRotinaModel> Consultar(AtividadeRotinaModel rotina)
    {
        string sql = sqlSelect;
        sql += " AND id_atividadeRotina = @Codigo ";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var atividadeRotina = await conexao.QueryFirstOrDefaultAsync<AtividadeRotinaQuery>(sql, new
            {
                Codigo = rotina.IdAtividadeRotina,
            });

            return new AtividadeRotinaModel
            {
                IdAtividadeRotina = atividadeRotina.id_atividadeRotina,
                NmAtividadeRotina = atividadeRotina.nm_AtividadeRotina,
                NmProcedure = atividadeRotina.nm_procedure,
                FgTipo = (TipoRotina)atividadeRotina.fg_tipo,
            };
        }
    }
}
