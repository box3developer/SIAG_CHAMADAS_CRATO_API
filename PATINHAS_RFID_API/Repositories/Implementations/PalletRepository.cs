using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Pallet;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class PalletRepository : IPalletRepository
    {
        const string sqlSelect = "SELECT id_pallet, id_areaarmazenagem, id_agrupador, fg_status, qt_utilizacao, dt_ultimamovimentacao, cd_identificacao FROM pallet with (nolock) WHERE 1 = 1 ";
        const string sqlInsert = "INSERT INTO pallet (id_pallet, id_areaarmazenagem, id_agrupador, fg_status, qt_utilizacao, dt_ultimamovimentacao, cd_identificacao) VALUES (@Codigo, @AreaArmazenagem, @Agrupador, @Status, @QtUtilizacao, @DataUltimaMovimentacao, @Identificacao)";

        public async Task<PalletModel> Consultar(string identificador, int codigo = 0)
        {
            string sql = sqlSelect;
            sql += " AND cd_identificacao = @Identificador " +
                   " AND id_pallet <> @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var pallet = await conexao.QueryFirstOrDefaultAsync<PalletQuery>(sql, new
                {
                    Identificador = identificador,
                    Codigo = codigo,
                });

                if (pallet == null)
                {
                    return null;
                }

                return new PalletModel
                {
                    IdPallet = pallet.id_pallet,
                    AreaArmazenagem = new AreaArmazenagemModel
                    {
                        IdAreaArmazenagem = pallet.id_areaarmazenagem,
                    },
                    Agrupador = new AgrupadorAtivoModel
                    {
                        Codigo = pallet.id_agrupador,
                    },
                    FgStatus = (StatusPallet)pallet.fg_status,
                    QtUtilizacao = pallet.qt_utilizacao,
                    DtUltimaMovimentacao = pallet.dt_ultimamovimentacao,
                    CdIdentificacao = pallet.cd_identificacao
                };
            }
        }

        private static void ValidaCampos(PalletModel pallet, bool emEdicao = false)
        {
            if (pallet.IdPallet <= 0 && emEdicao)
            {
                throw new Exception("Para inserir/alterar um pallet o código deve ser maior que zero (0).");
            }

            if ((int)pallet.FgStatus <= 0)
            {
                throw new Exception("Para inserir/alterar um pallet é obrigatório informar status.");
            }
        }

        private void ValidaIdentificador(string identificador, int codigo = 0)
        {
            if (!String.IsNullOrEmpty(identificador))
            {
                if (SiagAPI.GetPalletByIdenfificador(identificador, codigo) != null)
                {
                    throw new Exception("Identificador já cadastrado para outro Pallet.");
                }
            }
        }

        public async Task<bool> Inserir(PalletModel pallet)
        {
            ValidaCampos(pallet);
            ValidaIdentificador(pallet.CdIdentificacao, pallet.IdPallet);

            await SiagAPI.InsertPallet(pallet);

            return true;
            //var filtros = new Dictionary<string, object>();

            //filtros.Add("@Codigo", pallet.IdPallet);
            //filtros.Add("@Status", (int)pallet.FgStatus);
            //filtros.Add("@QtUtilizacao", pallet.QtUtilizacao);

            //if (pallet.AreaArmazenagem == null) filtros.Add("@AreaArmazenagem", DBNull.Value);
            //else filtros.Add("@AreaArmazenagem", pallet.AreaArmazenagem.IdAreaArmazenagem);

            //if ((pallet.Agrupador == null) || (pallet.Agrupador.Codigo == Guid.Empty)) filtros.Add("@Agrupador", DBNull.Value);
            //else filtros.Add("@Agrupador", pallet.Agrupador.Codigo);

            //if (pallet.DtUltimaMovimentacao == null) filtros.Add("@DataUltimaMovimentacao", DBNull.Value);
            //else filtros.Add("@DataUltimaMovimentacao", pallet.DtUltimaMovimentacao);

            //if (pallet.CdIdentificacao == null) filtros.Add("@Identificacao", DBNull.Value);
            //else filtros.Add("@Identificacao", pallet.CdIdentificacao);

            //var parametros = new DynamicParameters(filtros);

            //using (var conexao = new SqlConnection(Global.Conexao))
            //{
            //    var palletEncontrado = await conexao.ExecuteAsync(sqlInsert, parametros);

            //    return palletEncontrado > 0;
            //}
        }
    }

}
