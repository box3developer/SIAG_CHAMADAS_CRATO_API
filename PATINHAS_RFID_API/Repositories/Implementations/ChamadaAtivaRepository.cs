using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class ChamadaAtivaRepository : IChamadaAtivaRepository
{
    public async Task<List<EnderecoModel>> RetornaEnderecos(int idPallet)
    {
        var sql = "exec sp_siag_destinopallet @id_pallet";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var listaDestinoPallet = (await conexao.QueryAsync<EnderecoModel>(sql, new
            {
                id_pallet = idPallet,
            })).ToList();

            return listaDestinoPallet;
        }
    }

    public async Task<int> QuantidadePalletsAgupador(int idEndereco, int idPallet)
    {
        SqlParameter outputReturn = new SqlParameter("@retorno", SqlDbType.Int);
        outputReturn.Direction = ParameterDirection.Output;

        string sql = "exec sp_siag_busca_qtde_pallets @idChamada, @idRejeicao";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var resultado = await conexao.ExecuteAsync(sql, new
            {
                endereco = idEndereco,
                pallet = idPallet,
                retorno = outputReturn, // VALIDAR OUTPUT RETORNO
            });

            return (int)outputReturn.Value;
        }
    }

    public async Task<AreaArmazenagemModel?> RetornaStageInDisponivel(int id_endereco)
    {
        var sql = "exec sp_rotina_retornastageinlivre @id_endereco";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var areaRetorno = await conexao.QueryFirstOrDefaultAsync<AreaArmazenagemModel?>(sql, new
            {
                id_endereco = id_endereco,
            });

            if (areaRetorno != null && (areaRetorno.CdIdentificacao != null) && (!String.IsNullOrEmpty(areaRetorno.CdIdentificacao.ToString())))
            {
                areaRetorno.IdAreaArmazenagem = Convert.ToInt64(areaRetorno.CdIdentificacao.ToString());
                return areaRetorno;
            }

            return null;
        }
    }

    public async Task<bool> ReservaArea(AreaArmazenagemModel areaArmazenagem)
    {
        string sql = "update areaarmazenagem " +
                     " set fg_status = @Status" +
                     " where id_areaarmazenagem = @Codigo";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var resercada = await conexao.ExecuteAsync(sql, new
            {
                Status = StatusAreaArmazenagem.Reservado,
                Codigo = areaArmazenagem.IdAreaArmazenagem
            });

            return resercada > 0;
        }
    }

    public async Task<bool> CriaAreaTransicao(AreaArmazenagemModel areaOrigem, AreaArmazenagemModel areaDestino)
    {
        string sqlD = "delete from tmp_transicaochamada " +
                      " where id_areaarmazenagemorigem = @Origem";

        string sqlI = "insert into tmp_transicaochamada (id_areaarmazenagemorigem,id_areaarmazenagemdestino) values(@Origem, @Destino)";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var remover = await conexao.ExecuteAsync(sqlD, new
            {
                Origem = areaOrigem.IdAreaArmazenagem,
            });

            var criar = await conexao.ExecuteAsync(sqlI, new
            {
                Origem = areaOrigem.IdAreaArmazenagem,
                Destino = areaDestino.IdAreaArmazenagem,
            });

            return remover > 0 && criar > 0;
        }
    }

    public async Task<AreaArmazenagemModel?> BuscaAreaLivre()
    {
        var sql = "exec sp_siag_buscaarealivre";

        using (var conexao = new SqlConnection(Global.Conexao))
        {
            var areaArmazenagem = await conexao.QueryFirstOrDefaultAsync<AreaArmazenagemModel?>(sql);
            return areaArmazenagem;
        }

    }
}
