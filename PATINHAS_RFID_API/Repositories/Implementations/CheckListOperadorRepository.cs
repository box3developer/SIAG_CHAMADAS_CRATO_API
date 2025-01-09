using PATINHAS_RFID_API.Integration;
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
            checklistOperador.IdEquipamentoChecklist = checklistOperador.Checklist?.IdEquipamentoChecklist ?? 0;
            checklistOperador.IdEquipamento = checklistOperador.Equipamento?.IdEquipamento ?? 0;
            checklistOperador.IdOperador = checklistOperador.Operador?.IdOperador ?? 0;

            await SiagAPI.InsertChecklistOperador(checklistOperador);

            return true;

            //using (var conexao = new SqlConnection(Global.Conexao))
            //{
            //    var qtdLinhas = await conexao.ExecuteAsync(sqlInsert, new
            //    {
            //        Equipamento = checklistOperador.Equipamento.IdEquipamento,
            //        Operador = checklistOperador.Operador.IdOperador,
            //        Checklist = checklistOperador.Checklist.IdEquipamentoChecklist,
            //        Resposta = checklistOperador.FgResposta,
            //        Data = checklistOperador.DtChecklist,
            //    });

            //    return qtdLinhas > 0;
            //}
        }
    }
}
