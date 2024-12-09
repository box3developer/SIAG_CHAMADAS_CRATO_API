using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class EnderecoRepository: IEnderecoRepository
    {
        const string sqlSelect = "SELECT id_endereco, id_regiaotrabalho, id_setortrabalho, id_tipoendereco, nm_endereco, qt_estoqueminimo, qt_estoquemaximo, fg_status, tp_preenchimento FROM endereco with(NOLOCK) WHERE 1 = 1 ";

        public async Task<EnderecoModel> Consultar(EnderecoModel endereco)
        {
            string sql = sqlSelect;
            sql += "AND id_endereco = @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var enderecoEncontrado = await conexao.QueryFirstOrDefaultAsync<EnderecoQuery>(sql, new
                {
                    Codigo = endereco.Codigo,
                });

                return new EnderecoModel
                {
                    Codigo = enderecoEncontrado.id_endereco,
                    Regiao = new RegiaoModel
                    {
                        Codigo = enderecoEncontrado.id_regiaotrabalho,
                    },
                    Setor = new SetorModel
                    {
                        Codigo = enderecoEncontrado.id_setortrabalho
                    },
                    TipoEndereco = new TipoEndereco
                    {
                        Codigo = enderecoEncontrado.id_tipoendereco,
                    },
                    Descricao = enderecoEncontrado.nm_endereco,
                    EstoqueMinimo = enderecoEncontrado.qt_estoqueminimo,
                    EstoqueMaximo = enderecoEncontrado.qt_estoquemaximo,
                    Status = (StatusEndereco)enderecoEncontrado.fg_status,
                    TipoPreenchimento = (TipoPreenchimento)enderecoEncontrado.tp_preenchimento,
                };
            }
        }
    }
}
