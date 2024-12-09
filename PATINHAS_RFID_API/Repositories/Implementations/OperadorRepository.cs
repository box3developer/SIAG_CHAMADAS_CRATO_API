using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class OperadorRepository : IOperadorRepository
    {
        const string sqlSelect = "SELECT id_operador, nm_operador, dt_login, nm_cpf, nr_localidade, fg_funcao, id_responsavel FROM operador with(nolock) WHERE 1 = 1 ";

        public async Task<OperadorModel> Consultar(OperadorModel operador)
        {
            string sql = sqlSelect;

            OperadorQuery operadorEncontrado = null;

            if(string.IsNullOrWhiteSpace(operador.NFC))
            {
                using (var conexao = new SqlConnection(Global.Conexao))
                {
                    String query = sqlSelect + "AND id_operador = @Codigo ";
                    operadorEncontrado = await conexao.QueryFirstOrDefaultAsync<OperadorQuery>(query, new
                    {
                        Codigo = operador.Codigo,
                    });
                }
            } else
            {
                using (var conexao = new SqlConnection(Global.Conexao))
                {
                    String query = sqlSelect + "AND nm_nfcoperador = @Codigo ";
                    operadorEncontrado = await conexao.QueryFirstOrDefaultAsync<OperadorQuery>(query, new
                    {
                        Codigo = operador.NFC,
                    });
                }
            }

            if (operadorEncontrado == null) return null;

            return new OperadorModel
            {
                Codigo = operadorEncontrado.id_operador,
                Descricao = operadorEncontrado.nm_operador,
                DataLogin = operadorEncontrado.dt_login,
                CPF = operadorEncontrado.nm_cpf,
                Localidade = (Estabelecimentos)operadorEncontrado.nr_localidade,
                FuncaoOperador = (FuncaoOperador)operadorEncontrado.fg_funcao,
                Responsavel = new OperadorModel
                {
                    Codigo = operadorEncontrado.id_responsavel
                },
            };
        }
    }
}
