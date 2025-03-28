﻿using Dapper;
using Microsoft.Data.SqlClient;
using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Endereco;
using PATINHAS_RFID_API.Repositories.Interfaces;

namespace PATINHAS_RFID_API.Repositories.Implementations;

public class AreaArmazenagemRepository : IAreaArmazenagemRepository
{
    const string SELECT = "SELECT id_areaarmazenagem, id_tipoarea, id_endereco, id_agrupador, nr_posicaox, nr_posicaoy, nr_lado, fg_status, cd_identificacao FROM areaarmazenagem with (nolock)";

    public async Task<AreaArmazenagemModel> Consultar(AreaArmazenagemModel areaArmazenagem)
    {
        string sql = $"{SELECT} WHERE id_areaarmazenagem = @Codigo";

        using var conexao = new SqlConnection(Global.Conexao);
        var areaArmazenagemEncontrada = await conexao.QueryFirstOrDefaultAsync<AreaArmazenagemQuery>(sql, new
        {
            Codigo = areaArmazenagem.IdAreaArmazenagem
        });

        return new AreaArmazenagemModel
        {
            IdAreaArmazenagem = areaArmazenagemEncontrada.id_areaarmazenagem,
            TipoArea = new TipoAreaModel { Codigo = areaArmazenagemEncontrada.id_tipoarea },
            Endereco = new EnderecoModel { IdEndereco = areaArmazenagemEncontrada.id_endereco },
            Agrupador = new AgrupadorAtivoModel { Codigo = areaArmazenagemEncontrada.id_agrupador },
            NrPosicaoX = areaArmazenagemEncontrada.nr_posicaox,
            NrPosicaoY = areaArmazenagemEncontrada.nr_posicaoy,
            NrLado = areaArmazenagemEncontrada.nr_lado,
            FgStatus = (StatusAreaArmazenagem)areaArmazenagemEncontrada.fg_status,
            CdIdentificacao = areaArmazenagemEncontrada.cd_identificacao
        };
    }
}
