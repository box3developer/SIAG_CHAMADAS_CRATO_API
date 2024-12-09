using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class CheckListRepository : ICheckListRepository
    {
        public async Task<List<EquipamentoChecklistModel>> ConsultarListaPorEquipamento(EquipamentoModel equipamento)
        {
            var sql = "exec sp_get_checklist_Equipamento @nm_identificador";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var areaArmazenagemEncontrada = (await conexao.QueryAsync<EquipamentoCheckListQuery>(sql, new
                {
                    nm_identificador = equipamento.Identificador
                })).Select(x => new EquipamentoChecklistModel
                {
                    Codigo = x.id_equipamentochecklist,
                    Modelo = new Models.EquipamentoModeloModel
                    {
                        Codigo = x.id_equipamentomodelo
                    },
                    Descricao = x.nm_descricao,
                    Critico = x.fg_critico == 1 ? true : false,
                    Status = (Status)x.fg_status
                }).ToList();

                return areaArmazenagemEncontrada;
            }
        }
    }
}
