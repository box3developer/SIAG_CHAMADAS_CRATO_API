using Dapper;
using grendene_caracois_api_csharp;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;
using System.Dynamic;

namespace PATINHAS_RFID_API.Repositories.Implementations
{
    public class EquipamentoRepository : IEquipamentoRepository
    {
        const string sqlSelect = "SELECT id_equipamento, id_setortrabalho, id_operador, nm_equipamento, id_equipamentomodelo, fg_status, dt_inclusao, dt_manutencao, nm_identificador, dt_ultimaleitura, id_endereco, nm_ip, fg_statustrocacaracol, nm_abreviado_equipamento FROM equipamento with(nolock) WHERE 1 = 1 ";
        const string sqlInsert = "INSERT INTO equipamento (id_setortrabalho, nm_equipamento, id_equipamentomodelo, fg_status, dt_inclusao, dt_manutencao, nm_identificador, nm_abreviado_equipamento) VALUES (@Setor, @Descricao, @Modelo, @Status, @DataInclusao, @DataManutencao, @Identificador, @DescricaoAbreviada) ";
        const string sqlUpdate = "UPDATE equipamento SET id_setortrabalho = @Setor, nm_equipamento = @Descricao, id_equipamentomodelo = @Modelo, fg_status = @Status, dt_inclusao = @DataInclusao, dt_manutencao = @DataManutencao, nm_identificador = @Identificador, nm_ip = @IP, nm_abreviado_equipamento = @DescricaoAbreviada WHERE id_equipamento = @Codigo";
        const string sqlDelete = "DELETE FROM equipamento WHERE id_equipamento = @Codigo";
        const string sqlLogoff = "UPDATE equipamento SET id_operador = NULL WHERE 1 = 1";
        const string sqlSelectUtilizacao = "SELECT count(id_equipamento) as total, count(CASE WHEN id_operador IS NOT NULL THEN id_equipamento END) as logado, count(CASE WHEN id_operador IS NULL THEN id_equipamento END) as disponivel FROM equipamento  with(NOLOCK) WHERE 1 = 1 ";
        private const string sqlSelectPorSetorModelo = "SELECT id_equipamento, id_setortrabalho, operador.id_operador, nm_equipamento, id_equipamentomodelo, fg_status, dt_inclusao, dt_manutencao, nm_identificador, dt_ultimaleitura, id_endereco, nm_ip, fg_statustrocacaracol, nm_observacao, cd_ultimaleitura, nm_abreviado_equipamento FROM equipamento with(nolock) JOIN operador on(operador.id_operador = equipamento.id_operador) WHERE id_setortrabalho = @setor AND id_equipamentomodelo = @modelo";

        public async Task<EquipamentoModel> Consultar(string identificador, int? codigo = 0)
        {

            string sql = sqlSelect;
            sql += " AND nm_identificador = @Identificador " +
                   " AND id_equipamento <> @Codigo ";

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamento = await conexao.QueryFirstOrDefaultAsync<EquipamentoQuery>(sql, new
                {
                    Identificador = identificador,
                    Codigo = codigo
                });

                if (equipamento == null) return null;

                return new EquipamentoModel
                {
                    Codigo = equipamento.id_equipamento,
                    Modelo = new EquipamentoModeloModel {
                        Codigo = equipamento.id_equipamentomodelo,
                    },
                    Setor = new SetorModel
                    {
                        Codigo = equipamento.id_setortrabalho
                    },
                    Operador = new OperadorModel
                    {
                        Codigo = equipamento.id_operador
                    },
                    Descricao = equipamento.nm_equipamento,
                    DescricaoAbreviada = equipamento.nm_abreviado_equipamento,
                    Identificador = equipamento.nm_identificador,
                    Status = (StatusEquipamento)equipamento.fg_status,
                    DataInclusao = equipamento.dt_inclusao,
                    DataManutencao = equipamento.dt_manutencao,
                    DataUltimaLeitura = equipamento.dt_ultimaleitura,
                    EnderecoTrabalho = new EnderecoModel
                    {
                        Codigo = equipamento.id_endereco,
                    },
                    IP = equipamento.nm_ip,
                    StatusTrocaCaracol = (Ativo)equipamento.fg_statustrocacaracol,
                };
            }
        }

        public async Task AtualizaMovimentacao(EquipamentoModel equipamento, EnderecoModel? endereco = null)
        {
            string sql = "exec sp_siag_atualizaequipamento @Equipamento, @Endereco";

            int? codigoEndereco = null;

            if ((endereco != null) && (endereco.Codigo > 0))
                codigoEndereco = endereco.Codigo;

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.QueryAsync(sql, new
                {
                    Endereco = equipamento.Codigo,
                    Equipamento = codigoEndereco,
                });
            }
        }

        public async Task<List<EquipamentoModel>> ConsultarLista(EquipamentoModel? equipamento = null, int codInicial = 0, int codFinal = 0)
        {
            if ((codInicial != 0 & codFinal != 0) & (codInicial > codFinal))
                throw new Exception("O código inicial não pode ser maior que o código final.");

            string sql = sqlSelect;

            var filtros = new Dictionary<string, object>();

            if (equipamento != null)
            {
                if ((equipamento.Setor != null) && (equipamento.Setor.Codigo != 0))
                {
                    sql += " AND id_setortrabalho = @Setor ";
                    filtros.Add("@Setor", equipamento.Setor.Codigo);
                }
                if ((equipamento.Operador != null) && (equipamento.Operador.Codigo != 0))
                {
                    sql += " AND id_operador = @Operador ";
                    filtros.Add("@Operador", equipamento.Operador.Codigo);
                }
                if (!String.IsNullOrEmpty(equipamento.Descricao))
                {
                    sql += " AND nm_equipamento like @Descricao ";
                    filtros.Add("@Descricao", "%" + equipamento.Descricao + "%");
                }
                if (!String.IsNullOrEmpty(equipamento.DescricaoAbreviada))
                {
                    sql += " AND nm_abreviado_equipamento like @DescricaoAbreviada ";
                    filtros.Add("@DescricaoAbreviada", "%" + equipamento.DescricaoAbreviada + "%");
                }
                if ((equipamento.Modelo != null) && (equipamento.Modelo.Codigo != 0))
                {
                    sql += " AND id_equipamentomodelo = @Modelo ";
                    filtros.Add("@Modelo", equipamento.Modelo.Codigo);
                }
                if (equipamento.Status != StatusEquipamento.Indefinido)
                {
                    sql += " AND fg_status = @Status ";
                    filtros.Add("@Status", (int)equipamento.Status);
                }
                if (!String.IsNullOrEmpty(equipamento.Identificador))
                {
                    sql += " AND nm_identificador = @Identificador ";
                    filtros.Add("@Identificador", equipamento.Identificador);
                }
                if ((equipamento.EnderecoTrabalho != null) && (equipamento.EnderecoTrabalho.Codigo != 0))
                {
                    sql += " AND id_endereco = @Endereco ";
                    filtros.Add("@Endereco", equipamento.EnderecoTrabalho.Codigo);
                }
            }
            if (codInicial != 0)
            {
                sql += " AND id_equipamento >= @CodInicial ";
                filtros.Add("@CodInicial", codInicial);
            }
            if (codFinal != 0)
            {
                sql += " AND id_equipamento <= @CodFinal ";
                filtros.Add("@CodFinal", codFinal);
            }

            var parametros = new DynamicParameters(filtros);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentosAtualizado = (await conexao.QueryAsync<EquipamentoQuery>(sql, parametros)).Select(equipamento => new EquipamentoModel
                {
                    Codigo = equipamento.id_equipamento,
                    Modelo = new EquipamentoModeloModel
                    {
                        Codigo = equipamento.id_equipamentomodelo,
                    },
                    Setor = new SetorModel
                    {
                        Codigo = equipamento.id_setortrabalho
                    },
                    Operador = new OperadorModel
                    {
                        Codigo = equipamento.id_operador
                    },
                    Descricao = equipamento.nm_equipamento,
                    DescricaoAbreviada = equipamento.nm_abreviado_equipamento,
                    Identificador = equipamento.nm_identificador,
                    Status = (StatusEquipamento)equipamento.fg_status,
                    DataInclusao = equipamento.dt_inclusao,
                    DataManutencao = equipamento.dt_manutencao,
                    DataUltimaLeitura = equipamento.dt_ultimaleitura,
                    EnderecoTrabalho = new EnderecoModel
                    {
                        Codigo = equipamento.id_endereco,
                    },
                    IP = equipamento.nm_ip,
                    StatusTrocaCaracol = (Ativo)equipamento.fg_statustrocacaracol,
                }).ToList();

                return equipamentosAtualizado;
            }
        }

        private static void validaCampos(EquipamentoModel equipamento, bool emEdicao = false)
        {
            if (equipamento.Codigo <= 0 && emEdicao)
                throw new Exception("Para inserir/alterar um equipamento o código deve ser maior que zero (0).");

            if (string.IsNullOrEmpty(equipamento.Descricao))
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar uma descrição.");

            if (equipamento.Status == StatusEquipamento.Indefinido)
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Status válido.");

            if ((equipamento.Modelo == null) || (equipamento.Modelo.Codigo <= 0))
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Modelo válido.");

            if ((equipamento.Setor == null) || (equipamento.Setor.Codigo <= 0))
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Setor cadastrado.");

            if (string.IsNullOrEmpty(equipamento.Identificador))
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um identificador.");
        }

        private void validaIdentificador(string identificador, int codigo)
        {
            if (!String.IsNullOrEmpty(identificador))
            {
                if (Consultar(identificador, codigo) != null)
                {
                    throw new Exception("Identificador já cadastrado para outro Equipamento.");
                }
            }
        }

        public async void Editar(EquipamentoModel equipamento)
        {
            validaCampos(equipamento, true);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.QueryAsync(sqlUpdate, new
                {
                    Codigo = equipamento.Codigo,
                    Setor = equipamento.Setor.Codigo,
                    Descricao = equipamento.Descricao,
                    DescricaoAbreviada = equipamento.DescricaoAbreviada,
                    Modelo = equipamento.Modelo.Codigo,
                    Status = (int)equipamento.Status,
                    Identificador = equipamento.Identificador,
                    IP = equipamento.IP,
                    DataInclusao = equipamento.DataInclusao,
                    DataManutencao = equipamento.DataManutencao
                });
            }
        }

        public async void LogoffOperador(EquipamentoModel equipamento, OperadorModel operador)
        {
            var filtros = new Dictionary<string, object>();

            if ((equipamento != null) && (equipamento.Codigo > 0)) filtros.Add("@id_equipamento", equipamento.Codigo);
            if ((operador != null) && (operador.Codigo > 0)) filtros.Add("@id_operador", operador.Codigo);

            var query = "EXEC sp_siag_logoffoperador @id_operador, @id_equipamento";

            var parametros = new DynamicParameters(filtros);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = await conexao.ExecuteAsync(query, parametros);
            }
        }

        public async void LoginOperador(EquipamentoModel equipamento, OperadorModel? operador)
        {
            var filtros = new Dictionary<string, object>();

            if ((equipamento != null) && (equipamento.Codigo > 0)) filtros.Add("@id_equipamento", equipamento.Codigo);

            if ((operador != null) && (operador.Codigo > 0)) filtros.Add("@id_operador", operador.Codigo);



            var query = "EXEC sp_siag_loginoperador @id_operador, @id_equipamento";

            var parameters = new DynamicParameters(filtros);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = await conexao.ExecuteAsync(query, parameters);
            }
        }

    }
}
