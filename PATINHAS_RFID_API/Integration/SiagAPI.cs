using grendene_caracois_api_csharp;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.AtividadeRotina;
using PATINHAS_RFID_API.Models.AtividadeTarefa;
using PATINHAS_RFID_API.Models.Chamada;
using PATINHAS_RFID_API.Models.ChamadaTarefa;
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
                    Codigo = filtro.Codigo,
                    Descricao = filtro.Descricao,
                    Mensagem = filtro.Mensagem,
                    AtividadeId = filtro.Atividade.Codigo,
                    Sequencia = filtro.Sequencia,
                    Recursos = (int)(filtro.Recursos ?? 0),
                    AtividadeRotinaId = filtro.AtividadeRotina.Codigo,
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
}
