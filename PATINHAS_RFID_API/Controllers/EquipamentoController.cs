﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.DTOs;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.EquipamentoCheckList;
using PATINHAS_RFID_API.Services.Interfaces;

namespace PATINHAS_RFID_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipamentoController : ControllerBase
    {
        private readonly IEquipamentoService _equipamentoService;

        public EquipamentoController(IEquipamentoService equipamentoService)
        {
            _equipamentoService = equipamentoService;
        }

        [HttpGet("ConsultarConfiguracoes/{cracha}/{identificadorEquipamento}")]
        public async Task<ActionResult> ConsultarConfiguracoes(long cracha, string identificadorEquipamento)
        {
            try
            {
                //return Ok(new ConfiguracaoModel
                //{
                //    TempoAquisicaoTarefas = 5000,
                //    TempoEstabilizacaoLeitura = 1000,
                //    ConfirmarAceiteTarefa = false,
                //    TamanhoCodigoArmazenagem = 10,
                //    TamanhoCodigoPallet = 6,
                //    OrdemLeitura = 0,
                //    InformarMotivoRejeicao = true,
                //    MotivosRejeicao = new List<AtividadeRejeicaoModel>
                //    {
                //        new AtividadeRejeicaoModel
                //        {
                //            Codigo = 1,
                //            Descricao = "Teste 1",
                //            Email = "emailteste1@grendene.com"
                //        },
                //        new AtividadeRejeicaoModel
                //        {
                //            Codigo = 1,
                //            Descricao = "Teste 1",
                //            Email = "emailteste1@grendene.com"
                //        },
                //        new AtividadeRejeicaoModel
                //        {
                //            Codigo = 1,
                //            Descricao = "Teste 1",
                //            Email = "emailteste1@grendene.com"
                //        }
                //    },
                //    ModeloEquipamento = new EquipamentoModeloModel
                //    {
                //        Codigo = 2,
                //        Descricao = "Patinha", 
                //        Status = StatusModeloEquipamento.Ativo
                //    }

                //});

                var result = await _equipamentoService.ConsultarConfiguracoes(cracha, identificadorEquipamento);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConsultarPerformance/{codOperador}")]
        public async Task<ActionResult> ConsultarPerformance(long cracha)
        {
            try
            {
                //return Ok(1);

                var result = await _equipamentoService.ConsultarPerformance(cracha);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnviaLocalizacaoEquipamento/{macEquipamento}/{retornoEquipamento}")]
        public async Task<ActionResult> EnviaLocalizacaoEquipamento(string macEquipamento, string retornoEquipamento)
        {
            try
            {
                var result = await _equipamentoService.EnviaLocalizacaoEquipamento(macEquipamento, retornoEquipamento);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCheckList/{identificadorEquipamento}")]
        public async Task<ActionResult> GetCheckList(string identificadorEquipamento)
        {
            try
            {
                var lista = new List<EquipamentoChecklistModel> {
                    new EquipamentoChecklistModel
                    {
                        Codigo = 1,
                        Modelo = new EquipamentoModeloModel(),
                        Descricao = "Verifique a carga da bateria",
                        Critico = false,
                        Status = Status.Ativo
                    },
                    new EquipamentoChecklistModel
                    {
                        Codigo = 2,
                        Modelo = new EquipamentoModeloModel(),
                        Descricao = "Teste Número 2",
                        Critico = false,
                        Status = Status.Ativo
                    },
                    new EquipamentoChecklistModel
                    {
                        Codigo = 3,
                        Modelo = new EquipamentoModeloModel(),
                        Descricao = "Teste Número 3",
                        Critico = false,
                        Status = Status.Ativo
                    },
                };

                //return Ok(lista);
                
                var result = await _equipamentoService.GetCheckList(identificadorEquipamento);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SetCheckList")]
        public async Task<ActionResult> SetCheckList([FromBody] SetCheckListDTO setCheckListDTO)
        {
            try
            {
                //return Ok(true);

                var result = await _equipamentoService.SetCheckList(setCheckListDTO);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("KeepAlive")]
        public ActionResult KeepAlive()
        {
            try
            {
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
