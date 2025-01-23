using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.DTOs.Chamada;
using PATINHAS_RFID_API.DTOs.Equipamento;
using PATINHAS_RFID_API.DTOs.Luzes;
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

    /** AreaArmazenagem **/
    public static async Task<AreaArmazenagemModel?> GetAreaArmazenagemByIdAsync(long id)
    {
        try
        {
            string url = $"{siagURL}/AreaArmazenagem/{id}/chamada";

            var response = await client.GetFromJsonAsync<APIResultDTO<AreaArmazenagemModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<AreaArmazenagemModel?> GetAreaArmazenagemByIdentificadorAsync(string identificador)
    {
        try
        {
            string url = $"{siagURL}/AreaArmazenagem/Identificador/{identificador}/chamada";

            var response = await client.GetFromJsonAsync<APIResultDTO<AreaArmazenagemModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** AtividadeRejeicao **/
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


    /** Atividade **/
    public static async Task<AtividadeModel?> GetAtividadeByIdAsync(int id)
    {
        try
        {
            string url = $"{siagURL}/Atividade/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<AtividadeModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** AtividadeRotina **/
    public static async Task<AtividadeRotinaModel> GetAtividadeRotinaByIdAsync(int id)
    {
        try
        {
            string url = $"{siagURL}/AtividadeRotina/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<AtividadeRotinaModel>>(url);

            if (response == null || response.Dados == null)
            {
                return new();
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** AtividadeTarefa **/
    public static async Task<AtividadeTarefaModel> GetAtividadeTarefaByIdAsync(int id)
    {
        try
        {
            string url = $"{siagURL}/AtividadeTarefa/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<AtividadeTarefaModel>>(url);

            if (response == null || response.Dados == null)
            {
                return new();
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<List<AtividadeTarefaModel>> GetListaAtividadeTarefaAsync(AtividadeTarefaModel? filtro)
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


    /** Chamada **/
    public static async Task<ChamadaModel?> GetChamadaByIdAsync(Guid id)
    {
        try
        {
            string url = $"{siagURL}/Chamada/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<ChamadaModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AtribuirChamadaAsync(ChamadaModel chamada)
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

    public static async Task<bool> RejeitarChamadaAsync(Guid id)
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

    public static async Task<bool> AlterarStatusChamadaAsync(Guid id, StatusChamada status)
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

    public static async Task<bool> FinalizarChamadaAsync(Guid id)
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

    public static async Task<bool> ReiniciarChamadaAsync(Guid id)
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

    public static async Task<Guid> SelecionarChamadaAsync(ChamadaModel chamada)
    {
        try
        {
            string url = $"{siagURL}/Chamada/selecionar";

            var response = await client.PostAsJsonAsync(url, chamada);
            var responseJson = await response.Content.ReadFromJsonAsync<APIResultDTO<Guid>>();

            if (responseJson == null)
            {
                return Guid.Empty;
            }

            return responseJson.Dados;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    public static async Task<List<ChamadaModel>> ConsultarChamadaAsync(ChamadaFiltroDTO chamada)
    {
        try
        {
            string url = $"{siagURL}/Chamada/listar";

            var response = await client.PostAsJsonAsync(url, chamada);
            var chamadas = await response.Content.ReadFromJsonAsync<List<ChamadaModel>>();

            return chamadas ?? new();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AtualizarLeituraChamadaAsync(ChamadaLeituraDTO leitura)
    {
        try
        {
            string url = $"{siagURL}/Chamada/atualizar-leitura";

            await client.PostAsJsonAsync(url, leitura);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<ValidaLeituraChamadaRetornoDTO?> ValidaChamadaAsync(ChamadaModel chamada, AtividadeRotinaModel atividadeRotina)
    {
        try
        {
            string url = $"{siagURL}/Chamada/valida-leitura";

            var response = await client.PostAsJsonAsync<ValidaLeituraChamadaDTO>(url, new()
            {
                AtividadeRotina = atividadeRotina,
                Chamada = chamada
            });

            var responseJson = await response.Content.ReadFromJsonAsync<APIResultDTO<ValidaLeituraChamadaRetornoDTO>>();

            if (responseJson == null || responseJson.Dados == null)
            {
                return null;
            }

            return responseJson.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** Pallet **/
    public static async Task<PalletModel?> GetPalletByIdAsync(int id)
    {
        try
        {
            string url = $"{siagURL}/Pallet/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<PalletModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> InsertPalletAsync(PalletModel pallet)
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

    public static async Task<PalletModel?> GetPalletByIdenfificadorAsync(string identificador, int id = 0)
    {
        try
        {
            string url = $"{siagURL}/pallet/identificador";

            var response = await client.PostAsJsonAsync<PalletFiltroDTO>(url, new()
            {
                CdIdentificador = identificador,
                IdPallet = id,
            });

            var json = await response.Content.ReadFromJsonAsync<APIResultDTO<PalletModel>>();

            if (json == null || json.Dados == null)
            {
                return null;
            }

            return json.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** ChamadaTarefas **/
    public static async Task<List<ChamadaTarefaModel>> GetChamadasTarefasByIdAsync(Guid idChamada, int idTarefa)
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

    public static async Task<List<ChamadaTarefaModel>> GetListaChamadasTarefasAsync()
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

    public static async Task<bool> UpdateChamadaTarefaAsync(ChamadaTarefaModel chamadaTarefa)
    {
        try
        {
            string url = $"{siagURL}/ChamadaTarefa";

            var response = await client.PutAsJsonAsync(url, chamadaTarefa);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** ChecklistOperador **/
    public static async Task<bool> InsertChecklistOperadorAsync(EquipamentoChecklistOperadorModel checklist)
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

    public static async Task<List<EquipamentoChecklistModel>> GetEquipamentosChecklistAsync(string identificador)
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


    /** Endereco **/
    public static async Task<EnderecoModel?> GetEnderecoByIdAsync(int idEndereco)
    {
        try
        {
            string url = $"{siagURL}/Endereco/{idEndereco}";

            var response = await client.GetFromJsonAsync<APIResultDTO<EnderecoModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** Equipamento **/
    public static async Task<EquipamentoModel?> GetEquipamentoByIdentificadorAsync(string identificador)
    {
        try
        {
            string url = $"{siagURL}/Equipamento/identificador/{identificador}";

            var response = await client.GetFromJsonAsync<APIResultDTO<EquipamentoModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> AtualizarEnderecoEquipamentoAsync(int idEquipamento, int? idEndereco = null)
    {
        try
        {
            string url = $"{siagURL}/Equipamento/endereco";

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

    public static async Task<bool> AtualizarEquipamentoAsync(EquipamentoModel equipamento)
    {
        try
        {
            string url = $"{siagURL}/Equipamento";

            await client.PutAsJsonAsync(url, equipamento);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /** Log **/
    public static async Task<bool> InsertLogAsync(string mensagem)
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


    /** Operador **/
    public static async Task<OperadorModel?> GetOperadorByCrachaAsync(long cracha)
    {
        try
        {
            string url = $"{siagURL}/Operador/{cracha}";

            var result = await client.GetFromJsonAsync<APIResultDTO<OperadorModel>>(url);

            if (result == null || result.Dados == null)
            {
                return null;
            }

            return result.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<OperadorModel?> GetOperadorByNFCAsync(string nfc)
    {
        try
        {
            string url = $"{siagURL}/Operador/nfc/{nfc}";

            var result = await client.GetFromJsonAsync<APIResultDTO<OperadorModel>>(url);

            if (result == null || result.Dados == null)
            {
                return null;
            }

            return result.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static async Task<bool> LoginOperadorAsync(long idOperador, int idEquipamento)
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

    public static async Task<bool> LogoffOperadorAsync(long idOperador, int idEquipamento)
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

    public static async Task<int> GetOperadorPerformance(long idOperador)
    {
        try
        {
            string url = $"{siagURL}/Operador/{idOperador}/performance";

            var result = await client.GetFromJsonAsync<APIResultDTO<int>>(url);

            if (result == null)
            {
                return 0;
            }

            return result.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** Setor **/
    public static async Task<SetorModel?> GetSetorByIdAsync(int id)
    {
        try
        {
            string url = $"{siagURL}/Setor/{id}";

            var response = await client.GetFromJsonAsync<APIResultDTO<SetorModel>>(url);

            if (response == null || response.Dados == null)
            {
                return null;
            }

            return response.Dados;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /** Luzes **/
    public static async Task<bool> ApagarLuzVermelha(int caracol, int gaiola)
    {
        try
        {
            string url = $"{siagURL}/Luzes/luzVermelha/desligar";

            var response = await client.PostAsJsonAsync<LuzesFiltroDTO>(url, new()
            {
                Caracol = caracol,
                Gaiola = gaiola
            });

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
