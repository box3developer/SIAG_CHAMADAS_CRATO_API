using Dapper;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Integration;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Repositories.Interfaces;

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

                if (equipamento == null)
                {
                    return null;
                }

                return new EquipamentoModel
                {
                    IdEquipamento = equipamento.id_equipamento,
                    EquipamentoModelo = new EquipamentoModeloModel
                    {
                        Codigo = equipamento.id_equipamentomodelo,
                    },
                    SetorTrabalho = new SetorModel
                    {
                        IdSetorTrabalho = equipamento.id_setortrabalho
                    },
                    Operador = new OperadorModel
                    {
                        IdOperador = equipamento.id_operador
                    },
                    Endereco = new EnderecoModel
                    {
                        IdEndereco = equipamento.id_endereco,
                    },
                    NmEquipamento = equipamento.nm_equipamento,
                    NmAbreviadoEquipamento = equipamento.nm_abreviado_equipamento,
                    NmIdentificador = equipamento.nm_identificador,
                    FgStatus = (StatusEquipamento)equipamento.fg_status,
                    DtInclusao = equipamento.dt_inclusao,
                    DtManutencao = equipamento.dt_manutencao,
                    DtUltimaLeitura = equipamento.dt_ultimaleitura,
                    NmIP = equipamento.nm_ip,
                    FgStatusTrocaCaracol = (Ativo)equipamento.fg_statustrocacaracol,
                };
            }
        }

        public async Task AtualizaMovimentacao(EquipamentoModel equipamento, EnderecoModel? endereco = null)
        {
            string sql = "exec sp_siag_atualizaequipamento @Equipamento, @Endereco";

            int? codigoEndereco = null;

            if ((endereco != null) && (endereco.IdEndereco > 0))
            {
                codigoEndereco = endereco.IdEndereco;
            }

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.QueryAsync(sql, new
                {
                    Endereco = equipamento.IdEquipamento,
                    Equipamento = codigoEndereco,
                });
            }
        }

        public async Task<List<EquipamentoModel>> ConsultarLista(EquipamentoModel? equipamento = null, int codInicial = 0, int codFinal = 0)
        {
            if ((codInicial != 0 & codFinal != 0) & (codInicial > codFinal))
            {
                throw new Exception("O código inicial não pode ser maior que o código final.");
            }

            string sql = sqlSelect;

            var filtros = new Dictionary<string, object>();

            if (equipamento != null)
            {
                if ((equipamento.SetorTrabalho != null) && (equipamento.SetorTrabalho.IdSetorTrabalho != 0))
                {
                    sql += " AND id_setortrabalho = @Setor ";
                    filtros.Add("@Setor", equipamento.SetorTrabalho.IdSetorTrabalho);
                }
                if ((equipamento.Operador != null) && (equipamento.Operador.IdOperador != 0))
                {
                    sql += " AND id_operador = @Operador ";
                    filtros.Add("@Operador", equipamento.Operador.IdOperador);
                }
                if (!String.IsNullOrEmpty(equipamento.NmEquipamento))
                {
                    sql += " AND nm_equipamento like @Descricao ";
                    filtros.Add("@Descricao", "%" + equipamento.NmEquipamento + "%");
                }
                if (!String.IsNullOrEmpty(equipamento.NmAbreviadoEquipamento))
                {
                    sql += " AND nm_abreviado_equipamento like @DescricaoAbreviada ";
                    filtros.Add("@DescricaoAbreviada", "%" + equipamento.NmAbreviadoEquipamento + "%");
                }
                if ((equipamento.EquipamentoModelo != null) && (equipamento.EquipamentoModelo.Codigo != 0))
                {
                    sql += " AND id_equipamentomodelo = @Modelo ";
                    filtros.Add("@Modelo", equipamento.EquipamentoModelo.Codigo);
                }
                if (equipamento.FgStatus != StatusEquipamento.Indefinido)
                {
                    sql += " AND fg_status = @Status ";
                    filtros.Add("@Status", (int)equipamento.FgStatus);
                }
                if (!String.IsNullOrEmpty(equipamento.NmIdentificador))
                {
                    sql += " AND nm_identificador = @Identificador ";
                    filtros.Add("@Identificador", equipamento.NmIdentificador);
                }
                if ((equipamento.Endereco != null) && (equipamento.Endereco.IdEndereco != 0))
                {
                    sql += " AND id_endereco = @Endereco ";
                    filtros.Add("@Endereco", equipamento.Endereco.IdEndereco);
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
                    IdEquipamento = equipamento.id_equipamento,
                    EquipamentoModelo = new EquipamentoModeloModel
                    {
                        Codigo = equipamento.id_equipamentomodelo,
                    },
                    SetorTrabalho = new SetorModel
                    {
                        IdSetorTrabalho = equipamento.id_setortrabalho
                    },
                    Operador = new OperadorModel
                    {
                        IdOperador = equipamento.id_operador
                    },
                    NmEquipamento = equipamento.nm_equipamento,
                    NmAbreviadoEquipamento = equipamento.nm_abreviado_equipamento,
                    NmIdentificador = equipamento.nm_identificador,
                    FgStatus = (StatusEquipamento)equipamento.fg_status,
                    DtInclusao = equipamento.dt_inclusao,
                    DtManutencao = equipamento.dt_manutencao,
                    DtUltimaLeitura = equipamento.dt_ultimaleitura,
                    Endereco = new EnderecoModel
                    {
                        IdEndereco = equipamento.id_endereco,
                    },
                    NmIP = equipamento.nm_ip,
                    FgStatusTrocaCaracol = (Ativo)equipamento.fg_statustrocacaracol,
                }).ToList();

                return equipamentosAtualizado;
            }
        }

        private static void ValidaCampos(EquipamentoModel equipamento, bool emEdicao = false)
        {
            if (equipamento.IdEquipamento <= 0 && emEdicao)
            {
                throw new Exception("Para inserir/alterar um equipamento o código deve ser maior que zero (0).");
            }

            if (string.IsNullOrEmpty(equipamento.NmEquipamento))
            {
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar uma descrição.");
            }

            if (equipamento.FgStatus == StatusEquipamento.Indefinido)
            {
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Status válido.");
            }

            if ((equipamento.EquipamentoModelo == null) || (equipamento.EquipamentoModelo.Codigo <= 0))
            {
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Modelo válido.");
            }

            if ((equipamento.SetorTrabalho == null) || (equipamento.SetorTrabalho.IdSetorTrabalho <= 0))
            {
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um Setor cadastrado.");
            }

            if (string.IsNullOrEmpty(equipamento.NmIdentificador))
            {
                throw new Exception("Para inserir/alterar um equipamento é obrigatório informar um identificador.");
            }
        }

        private void validaIdentificador(string identificador, int codigo)
        {
            if (!String.IsNullOrEmpty(identificador))
            {
                if (SiagAPI.GetEquipamentoByIdentificadorAsync(identificador) != null)
                {
                    throw new Exception("Identificador já cadastrado para outro Equipamento.");
                }
            }
        }

        public async void Editar(EquipamentoModel equipamento)
        {
            ValidaCampos(equipamento, true);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var equipamentoAtualizado = await conexao.QueryAsync(sqlUpdate, new
                {
                    Codigo = equipamento.IdEquipamento,
                    Setor = equipamento.SetorTrabalho.IdSetorTrabalho,
                    Descricao = equipamento.NmEquipamento,
                    DescricaoAbreviada = equipamento.NmAbreviadoEquipamento,
                    Modelo = equipamento.EquipamentoModelo.Codigo,
                    Status = (int)equipamento.FgStatus,
                    Identificador = equipamento.NmIdentificador,
                    IP = equipamento.NmIP,
                    DataInclusao = equipamento.DtInclusao,
                    DataManutencao = equipamento.DtManutencao
                });
            }
        }

        public async void LogoffOperador(EquipamentoModel equipamento, OperadorModel operador)
        {
            var filtros = new Dictionary<string, object>();

            if ((equipamento != null) && (equipamento.IdEquipamento > 0))
            {
                filtros.Add("@id_equipamento", equipamento.IdEquipamento);
            }

            if ((operador != null) && (operador.IdOperador > 0))
            {
                filtros.Add("@id_operador", operador.IdOperador);
            }

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

            if ((equipamento != null) && (equipamento.IdEquipamento > 0))
            {
                filtros.Add("@id_equipamento", equipamento.IdEquipamento);
            }

            if ((operador != null) && (operador.IdOperador > 0))
            {
                filtros.Add("@id_operador", operador.IdOperador);
            }

            var query = "EXEC sp_siag_loginoperador @id_operador, @id_equipamento";

            var parameters = new DynamicParameters(filtros);

            using (var conexao = new SqlConnection(Global.Conexao))
            {
                var linhas = await conexao.ExecuteAsync(query, parameters);
            }
        }

    }
}
