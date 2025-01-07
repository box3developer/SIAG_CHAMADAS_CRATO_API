using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class AtividadeRepository : IAtividadeRepository
    {
        const string sqlSelect = " SELECT id_atividade, nm_atividade, id_equipamentomodelo, fg_permite_rejeitar, id_atividadeanterior, id_setortrabalho, fg_tipoatribuicaoautomatica, id_atividaderotinavalidacao, fg_evitaconflitoendereco FROM atividade WITH(nolock) WHERE 1 = 1 ";
  
        public async Task<AtividadeModel> Consultar(AtividadeModel atividade)
        {
            string sql = sqlSelect;
            sql += " AND id_atividade = @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var atividadeEncontrada = await conexao.QueryFirstOrDefaultAsync<AtividadeQuery>(sql, new
                {
                    Codigo = atividade.IdAtividade
                });

                return new AtividadeModel
                {
                    IdAtividade = atividadeEncontrada.id_atividade,
                    NmAtividade = atividadeEncontrada.nm_atividade,
                    EquipamentoModelo = new EquipamentoModeloModel
                    {
                        Codigo = atividadeEncontrada.id_equipamentomodelo,
                    },
                    FgPermiteRejeitar = (RejeicaoTarefa)atividadeEncontrada.fg_permite_rejeitar,
                    AtividadeAnterior = new AtividadeModel
                    {
                        IdAtividade= atividadeEncontrada.id_atividadeanterior,
                    },
                    SetorTrabalho = new SetorModel
                    {
                        Codigo = atividadeEncontrada.id_setortrabalho
                    },
                    FgTipoAtribuicaoAutomatica = (TipoAtribuicaoAutomatica)atividadeEncontrada.fg_tipoatribuicaoautomatica,
                    AtividadeRotinaValidacao = new AtividadeRotinaModel
                    {
                        IdAtividadeRotina = atividadeEncontrada.id_atividaderotinavalidacao
                    },
                    FgEvitaConflitoEndereco = (ConflitoDeEnderecos)atividadeEncontrada.fg_evitaconflitoendereco,
                };
            }
        }
    }
}
