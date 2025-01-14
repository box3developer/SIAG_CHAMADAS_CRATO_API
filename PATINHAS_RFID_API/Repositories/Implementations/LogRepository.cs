using Dapper;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class LogRepository : ILogRepository
    {
        public async Task<bool> Insere(string mensagem)
        {
            var sql = "insert into logsiag (mensagem) values (@Mensagem) ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.ExecuteAsync(sql, new
                {
                    Mensagem = mensagem,
                });

                return equipamentoAtualizado > 0;
            }
        }
    }
}
