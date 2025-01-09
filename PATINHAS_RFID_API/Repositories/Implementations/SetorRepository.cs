using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Setor;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class SetorRepository : ISetorRepository
    {

        public async Task<SetorModel> Consultar(SetorModel setor)
        {
            const string sqlSelect = "SELECT id_setortrabalho, id_deposito, nm_setortrabalho FROM setortrabalho with(nolock) WHERE 1 = 1 ";

            string sql = sqlSelect;
            sql += "AND id_setortrabalho = @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var setorEncontrado = (await conexao.QueryFirstOrDefaultAsync<SetorQuery>(sql, new
                {
                    Codigo = setor.IdSetorTrabalho
                }));

                return new SetorModel
                {
                    IdSetorTrabalho = setorEncontrado.id_setortrabalho,
                    Deposito = new DepositoModel
                    {
                        Codigo = setorEncontrado.id_deposito
                    },
                    NmSetorTrabalho = setorEncontrado.nm_setortrabalho
                };
            }
        }
    }
}
