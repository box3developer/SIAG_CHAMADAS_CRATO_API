using grendene_caracois_api_csharp;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.DTOs.Equipamento;
using PATINHAS_RFID_API.DTOs.Operador;
using PATINHAS_RFID_API.DTOs.Pallet;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;

namespace PATINHAS_RFID_API.Integration;

public class SiagAPI
{
    private static readonly HttpClient client = new();
    private static readonly string siagURL = Global.SiagAPI;

    public static async Task<AreaArmazenagemModel> GetAreaArmazenagemByIdAsync(long id)
    {
        try
        {
            string url = $"{siagURL}/AreaArmazenagem/{id}";

            var areaArmazenagem = await client.GetFromJsonAsync<AreaArmazenagemModel>(url);

            return areaArmazenagem ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<AreaArmazenagemModel> GetAreaArmazenagemByIdentificadorAsync(string identificador)
    {
        try
        {
            string url = $"{siagURL}/AreaArmazenagem/Identificador/{identificador}";

            var areaArmazenagem = await client.GetFromJsonAsync<AreaArmazenagemModel>(url);

            return areaArmazenagem ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<List<AtividadeRejeicaoModel>> GetListaAtividadeRejeicaoAsync()
    {
        try
        {
            string url = $"{siagURL}/AtividadeRejeicao";

            var atividade = await client.GetFromJsonAsync<List<AtividadeRejeicaoModel>>(url);

            return atividade ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<AtividadeModel> GetAtividadeById(int id)
    {
        try
        {
            string url = $"{siagURL}/Atividade/{id}";

            var atividade = await client.GetFromJsonAsync<AtividadeModel>(url);

            return atividade ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<AtividadeRotinaModel> GetAtividadeRotinaById(int id)
    {
        try
        {
            string url = $"{siagURL}/AtividadeRotina/{id}";

            var atividade = await client.GetFromJsonAsync<AtividadeRotinaModel>(url);

            return atividade ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<AtividadeTarefaModel> GetAtividadeTarefaById(int id)
    {
        try
        {
            string url = $"{siagURL}/AtividadeTarefa/{id}";

            var atividade = await client.GetFromJsonAsync<AtividadeTarefaModel>(url);

            return atividade ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<List<AtividadeTarefaModel>> GetListaAtividadeTarefa(AtividadeTarefaModel? filtro)
    {
        try
        {
            string url = $"{siagURL}/AtividadeTarefa";
            var atividades = new List<AtividadeTarefaModel>();

            if (filtro == null)
            {
                atividades = await client.GetFromJsonAsync<List<AtividadeTarefaModel>>(url);
            }
            else
            {
                url = $"{url}/filtro";

                var parametros = new AtividadeTarefaFiltroDTO()
                {
                    Codigo = filtro.IdTarefa,
                    Descricao = filtro.NmTarefa,
                    Mensagem = filtro.NmMensagem,
                    AtividadeId = filtro?.Atividade?.IdAtividade ?? 0,
                    Sequencia = filtro?.CdSequencia ?? 0,
                    Recursos = (int)(filtro?.FgRecurso ?? 0),
                    AtividadeRotinaId = filtro?.AtividadeRotina?.IdAtividadeRotina ?? 0,
                };

                var response = await client.PostAsJsonAsync(url, parametros);
                atividades = await response.Content.ReadFromJsonAsync<List<AtividadeTarefaModel>>();
            }

            return atividades ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<ChamadaModel?> GetChamadaById(Guid id)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}";

            var chamada = await client.GetFromJsonAsync<ChamadaModel>(url);

            return chamada;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AtribuirChamada(ChamadaModel chamada)
    {
        try
        {
            string url = $"{siagURL}/Chamada/atribuir";

            await client.PostAsJsonAsync(url, chamada);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> RejeitarChamada(Guid id)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}/rejeitar";

            var response = await client.PostAsync(url, null);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AlterarStatusChamada(Guid id, StatusChamada status)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}/status/{status}";

            var response = await client.GetAsync(url);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> FinalizarChamada(Guid id)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}/finalizar";

            var response = await client.PostAsync(url, null);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> ReiniciarChamada(Guid id)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}/reiniciar";

            var response = await client.PostAsync(url, null);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<Guid> SelecionarChamada(ChamadaModel chamada)
    {
        try
        {
            string url = $"{siagURL}/Chamada/selecionar";

            var response = await client.PostAsJsonAsync(url, chamada);
            var id = await response.Content.ReadFromJsonAsync<Guid>();

            return id;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<PalletModel?> GetPalletById(int id)
    {
        try
        {
            string url = $"{siagURL}/Pallet/{id}";

            var pallet = await client.GetFromJsonAsync<PalletModel>(url);

            return pallet;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<List<ChamadaTarefaModel>> GetChamadasTarefasById(Guid idChamada, int idTarefa)
    {
        try
        {
            string url = $"{siagURL}/ChamadaTarefa/{idChamada}/{idTarefa}";

            var chamadaTarefa = await client.GetFromJsonAsync<List<ChamadaTarefaModel>>(url);

            return chamadaTarefa ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<List<ChamadaTarefaModel>> GetListaChamadasTarefas()
    {
        try
        {
            string url = $"{siagURL}/ChamadaTarefa";

            var chamadaTarefa = await client.GetFromJsonAsync<List<ChamadaTarefaModel>>(url);

            return chamadaTarefa ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> UpdateChamadaTarefa(ChamadaTarefaModel chamadaTareda)
    {
        try
        {
            string url = $"{siagURL}/ChamadaTarefa";

            var response = await client.PutAsJsonAsync(url, chamadaTareda);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<bool> InsertChecklistOperador(EquipamentoChecklistOperadorModel checklist)
    {
        try
        {
            string url = $"{siagURL}/EquipamentoChecklistOperador";

            var response = await client.PostAsJsonAsync(url, checklist);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<List<EquipamentoChecklistModel>> GetEquipamentosChecklist(string identificador)
    {
        try
        {
            string url = $"{siagURL}/EquipamentoChecklist/identificador/{identificador}";

            var equipamentoChecklist = await client.GetFromJsonAsync<List<EquipamentoChecklistModel>>(url);

            return equipamentoChecklist ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<EnderecoModel> GetEndereco(int idEndereco)
    {
        try
        {
            string url = $"{siagURL}/Endereco/{idEndereco}";

            var endereco = await client.GetFromJsonAsync<EnderecoModel>(url);

            if (endereco == null)
            {
                return new();
            }

            endereco.RegiaoTrabalho = new() { Codigo = endereco.IdRegiaoTrabalho };
            endereco.SetorTrabalho = new() { IdSetorTrabalho = endereco.IdSetorTrabalho };
            endereco.TipoEndereco = new() { Codigo = endereco.IdTipoEndereco };

            return endereco;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<EquipamentoModel> GetEquipamentoByIdentificador(string identificador)
    {
        try
        {
            string url = $"{siagURL}/Equipamento/identificador/{identificador}";

            var equipamento = await client.GetFromJsonAsync<EquipamentoModel>(url);

            if (equipamento == null)
            {
                return new();
            }

            equipamento.EquipamentoModelo = new() { Codigo = equipamento.IdEquipamento };
            equipamento.SetorTrabalho = new() { IdSetorTrabalho = equipamento.IdSetorTrabalho ?? 0 };
            equipamento.Operador = new() { IdOperador = equipamento.IdOperador };
            equipamento.Endereco = new() { IdEndereco = equipamento.IdEndereco ?? 0 };

            return equipamento;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AtualizarEquipamento(int idEquipamento, int? idEndereco = null)
    {
        try
        {
            string url = $"{siagURL}/Equipamento";

            await client.PutAsJsonAsync<EquipamentoUpdateDTO>(url, new()
            {
                IdEndereco = idEndereco,
                IdEquipamento = idEquipamento
            });

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<bool> LoginOperador(long idOperador, int idEquipamento)
    {
        try
        {
            string url = $"{siagURL}/Operador/login";

            await client.PostAsJsonAsync<OperadorLoginDTO>(url, new()
            {
                IdOperador = idOperador,
                IdEquipamento = idEquipamento,
            });

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> LogoffOperador(long idOperador, int idEquipamento)
    {
        try
        {
            string url = $"{siagURL}/Operador/logoff";

            await client.PostAsJsonAsync<OperadorLoginDTO>(url, new()
            {
                IdOperador = idOperador,
                IdEquipamento = idEquipamento,
            });

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<bool> InsertLog(string mensagem)
    {
        try
        {
            string url = $"{siagURL}/Log/siag";

            await client.PostAsJsonAsync(url, mensagem);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<OperadorModel?> GetOperador(long cracha, string nfc = "")
    {
        try
        {
            string url = $"{siagURL}/Operador";

            if (!string.IsNullOrEmpty(nfc))
            {
                url = $"{url}/{cracha}";
            }
            else
            {
                url = $"{url}/nfc/{nfc}";
            }

            var operador = await client.GetFromJsonAsync<OperadorModel>(url);

            return operador;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<bool> InsertPallet(PalletModel pallet)
    {
        try
        {
            string url = $"{siagURL}/Pallet";

            await client.PostAsJsonAsync(url, pallet);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<PalletModel?> GetPalletByIdenfificador(string identificador, int id = 0)
    {
        try
        {
            string url = $"{siagURL}/identificador";

            var response = await client.PostAsJsonAsync<PalletFiltroDTO>(url, new()
            {
                CdIdentificador = identificador,
                IdPallet = id,
            });

            var pallet = await response.Content.ReadFromJsonAsync<PalletModel>();

            return pallet;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public static async Task<SetorModel?> GetSetorById(int id)
    {
        try
        {
            string url = $"{siagURL}/Setor/{id}";

            var setor = await client.GetFromJsonAsync<SetorModel>(url);

            return setor;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
