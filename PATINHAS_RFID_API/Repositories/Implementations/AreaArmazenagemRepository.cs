using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class AreaArmazenagemRepository : IAreaArmazenagemRepository
    {

        public async Task<AreaArmazenagemModel> Consultar(AreaArmazenagemModel areaArmazenagem)
        {
            const string sqlSelect = "SELECT id_areaarmazenagem, id_tipoarea, id_endereco, id_agrupador, nr_posicaox, nr_posicaoy, nr_lado, fg_status, cd_identificacao FROM areaarmazenagem with (nolock) WHERE 1 = 1 ";

            string sql = sqlSelect;
            sql += "AND id_areaarmazenagem = @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var areaArmazenagemEncontrada = (await conexao.QueryFirstOrDefaultAsync<AreaArmazenagemQuery>(sql, new
                {
                    Codigo = areaArmazenagem.Codigo
                }));

                return new AreaArmazenagemModel
                {
                    Codigo = areaArmazenagemEncontrada.id_areaarmazenagem,
                    TipoArea = new TipoAreaModel { Codigo = areaArmazenagemEncontrada.id_tipoarea },
                    Endereco = new EnderecoModel { Codigo = areaArmazenagemEncontrada.id_endereco },
                    Agrupador = new AgrupadorAtivoModel { Codigo = areaArmazenagemEncontrada.id_agrupador },
                    PosicaoX = areaArmazenagemEncontrada.nr_posicaox,
                    PosicaoY = areaArmazenagemEncontrada.nr_posicaoy,
                    Lado = areaArmazenagemEncontrada.nr_lado,
                    Status = (StatusAreaArmazenagem)areaArmazenagemEncontrada.fg_status,
                    Identificacao = areaArmazenagemEncontrada.cd_identificacao
                };
            }
        }
    }
}
