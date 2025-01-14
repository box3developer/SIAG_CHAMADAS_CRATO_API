using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AtividadeController : ControllerBase
{
    private readonly IAtividadeService _atividadeService;


    public AtividadeController(IAtividadeService atividadeService)
    {
        _atividadeService = atividadeService;
    }

    [HttpPost("ConsultarTarefa")]
    public async Task<ActionResult> ConsultarTarefa([FromBody] SelecionarTarefaDTO selecionarTarefaDTO)
    {
        try
        {
            //return Ok(new List<ChamadaCompletaModel>
            //{
            //    new ChamadaCompletaModel {
            //        Codigo = new Guid("4d433057-a003-4efd-8f9c-af7a707f5de5"),
            //        PalletOrigem = new PalletModel(),
            //        PalletDestino = new PalletModel(),
            //        PalletLeitura = new PalletModel(),
            //        AreaArmazenagemOrigem = new AreaArmazenagemModel(),
            //        AreaArmazenagemDestino = new AreaArmazenagemModel(),
            //        AreaArmazenagemLeitura = new AreaArmazenagemModel(),
            //        Operador = new OperadorModel(),
            //        Equipamento = new EquipamentoModel(),
            //        AtividadeRejeicao = new AtividadeRejeicaoModel(),
            //        Atividade = new AtividadeModel{
            //            Codigo = 1,
            //            Descricao = "Chamada 1",
            //            EquipamentoModelo = new EquipamentoModeloModel(),
            //            PermiteRejeitar = RejeicaoTarefa.Permite,
            //            AtividadeAnterior = new AtividadeModel(),
            //            SetorTrabalho = new SetorModel(),
            //            TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //            AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //            EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //        },
            //        Status = StatusChamada.Aguardando,
            //        DataChamada = DateTime.Now,
            //        DataRecebida = DateTime.Now,
            //        DataAtendida = DateTime.Now,
            //        DataFinalizada = DateTime.Now,
            //        DataRejeitada = DateTime.Now,
            //        DataSuspensa = DateTime.Now,
            //        CodigoChamadaSuspensa = new Guid("e134159b-37eb-4021-9049-0b0850f0f77d"),

            //        Tarefas = new List<AtividadeTarefaModel>
            //        {
            //            new AtividadeTarefaModel
            //            {
            //                Codigo = 1,
            //                Descricao = "Tarefa 1",
            //                Mensagem = "Mensagem 1",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 1,
            //                    Descricao = "Atividade 1",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 1,
            //                Recursos = Recursos.ConfirmarOperacao,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //            new AtividadeTarefaModel
            //            {
            //                Codigo = 2,
            //                Descricao = "Tarefa 2",
            //                Mensagem = "Mensagem 2",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 1,
            //                    Descricao = "Atividade 2",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 2,
            //                Recursos = Recursos.LeituraEndereco,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //            new AtividadeTarefaModel
            //            {
            //                Codigo = 3,
            //                Descricao = "Tarefa 3",
            //                Mensagem = "Mensagem 3",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 1,
            //                    Descricao = "Atividade 3",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 3,
            //                Recursos = Recursos.LeituraEndereco,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //        },
            //        Concluida = false
            //    },
            //    new ChamadaCompletaModel {
            //        Codigo = new Guid("ee9a5039-28d2-4f3d-89be-8b53806c069e"),
            //        PalletOrigem = new PalletModel(),
            //        PalletDestino = new PalletModel(),
            //        PalletLeitura = new PalletModel(),
            //        AreaArmazenagemOrigem = new AreaArmazenagemModel(),
            //        AreaArmazenagemDestino = new AreaArmazenagemModel(),
            //        AreaArmazenagemLeitura = new AreaArmazenagemModel(),
            //        Operador = new OperadorModel(),
            //        Equipamento = new EquipamentoModel(),
            //        AtividadeRejeicao = new AtividadeRejeicaoModel(),
            //        Atividade = new AtividadeModel{
            //            Codigo = 2,
            //            Descricao = "Chamada 2",
            //            EquipamentoModelo = new EquipamentoModeloModel(),
            //            PermiteRejeitar = RejeicaoTarefa.Permite,
            //            AtividadeAnterior = new AtividadeModel(),
            //            SetorTrabalho = new SetorModel(),
            //            TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //            AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //            EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //        },
            //        Status = StatusChamada.Aguardando,
            //        DataChamada = DateTime.Now,
            //        DataRecebida = DateTime.Now,
            //        DataAtendida = DateTime.Now,
            //        DataFinalizada = DateTime.Now,
            //        DataRejeitada = DateTime.Now,
            //        DataSuspensa = DateTime.Now,
            //        CodigoChamadaSuspensa = new Guid("641e4b0b-d684-4b2b-8e39-07e7ebb2f187"),


            //        Tarefas = new List<AtividadeTarefaModel>
            //        {
            //           new AtividadeTarefaModel
            //            {
            //                Codigo = 4,
            //                Descricao = "Tarefa 4",
            //                Mensagem = "Mensagem 4",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 4,
            //                    Descricao = "Atividade 4",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 4,
            //                Recursos = Recursos.LeituraEndereco,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //            new AtividadeTarefaModel
            //            {
            //                Codigo = 5,
            //                Descricao = "Tarefa 5",
            //                Mensagem = "Mensagem 5",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 5,
            //                    Descricao = "Atividade 5",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 5,
            //                Recursos = Recursos.LeituraEndereco,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //            new AtividadeTarefaModel
            //            {
            //                Codigo = 6,
            //                Descricao = "Tarefa 6",
            //                Mensagem = "Mensagem 6",
            //                Atividade = new AtividadeModel{
            //                    Codigo = 6,
            //                    Descricao = "Atividade 6",
            //                    EquipamentoModelo = new EquipamentoModeloModel(),
            //                    PermiteRejeitar = RejeicaoTarefa.Permite,
            //                    AtividadeAnterior = new AtividadeModel(),
            //                    SetorTrabalho = new SetorModel(),
            //                    TipoAtribuicaoAutomatica = TipoAtribuicaoAutomatica.MesmoOperador,
            //                    AtividadeRotinaValidacao = new AtividadeRotinaModel(),
            //                    EvitarConflitoEndereco = ConflitoDeEnderecos.SemBloqueio
            //                },
            //                Sequencia = 6,
            //                Recursos = Recursos.LeituraEndereco,
            //                AtividadeRotina = new AtividadeRotinaModel(),
            //                PotenciaNormal = 0,
            //                PotenciaAumentada = 0,
            //            },
            //        },
            //        Concluida = false
            //    },
            //});

            selecionarTarefaDTO.Reconsultar = true;
            var response = await _atividadeService.SelecionaTarefa(selecionarTarefaDTO);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("IniciarTarefa/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> IniciarTarefa(string idChamada, int idTarefa)
    {
        try
        {
            //return Ok(true);
            var response = await _atividadeService.IniciarTarefa(idChamada, idTarefa);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("SelecionaTarefa")]
    public async Task<ActionResult> SelecionaTarefa([FromBody] SelecionarTarefaDTO selecionaTarefaDTO)
    {
        try
        {
            var response = await _atividadeService.SelecionaTarefa(selecionaTarefaDTO);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("RejeitarTarefa/{Cracha}/{idMotivo}/{idChamada}")]
    public async Task<ActionResult> RejeitarTarefa(long Cracha, int idMotivo, string? idChamada)
    {
        try
        {
            //return Ok(true);
            var response = await _atividadeService.RejeitarTarefa(Cracha, idMotivo, idChamada);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("EnviarLeitura/{identificadorPallet}/{identificadorAreaArmazenagem}/{idChamada}/{idTarefa}")]
    public async Task<ActionResult> EnviarLeitura(string? identificadorPallet, string? identificadorAreaArmazenagem, string idChamada, int idTarefa)
    {
        try
        {
            //return Ok(new MensagemWebservice
            //{
            //    Retorno = true,
            //    Mensagem = "Tarefa encerrada"
            //});

            var response = await _atividadeService.EfetivaLeitura(identificadorPallet, identificadorAreaArmazenagem, idChamada, idTarefa);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
