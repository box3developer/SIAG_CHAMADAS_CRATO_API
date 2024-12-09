using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class CheckListOperadorRepository : ICheckListOperadorRepository
    {
        const string sqlSelect = "SELECT id_equipamento, id_operador, id_equipamentochecklist, fg_resposta, dt_checklist FROM equipamentochecklistoperador  with(NOLOCK) WHERE 1 = 1 ";
        const string sqlInsert = "INSERT INTO equipamentochecklistoperador (id_equipamento, id_operador, id_equipamentochecklist, fg_resposta, dt_checklist) VALUES (@Equipamento, @Operador, @Checklist, @Resposta, @Data)";

        public async Task<bool> Inserir(EquipamentoChecklistOperadorModel checklistOperador)
        {

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var qtdLinhas = await conexao.ExecuteAsync(sqlInsert, new
                {
                    Equipamento = checklistOperador.Equipamento.Codigo,
                    Operador = checklistOperador.Operador.Codigo,
                    Checklist = checklistOperador.Checklist.Codigo,
                    Resposta = checklistOperador.Resposta,
                    Data = checklistOperador.Data,
                });

                return qtdLinhas > 0;
            }
        }
    }
}
