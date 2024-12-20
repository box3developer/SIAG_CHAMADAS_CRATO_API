﻿using PATINHAS_RFID_API.Data;
using PATINHAS_RFID_API.Models.AreaArmazenagem;
using PATINHAS_RFID_API.Models.Atividade;
using PATINHAS_RFID_API.Models.AtividadeRejeicao;
using PATINHAS_RFID_API.Models.Equipamento;
using PATINHAS_RFID_API.Models.Operador;
using PATINHAS_RFID_API.Models.Pallet;

namespace PATINHAS_RFID_API.Models.Chamada
{
    public class ChamadaModel
    {
        public Guid Codigo { get; set; }
        public PalletModel PalletOrigem { get; set; }
        public PalletModel PalletDestino { get; set; }
        public PalletModel PalletLeitura { get; set; }
        public AreaArmazenagemModel AreaArmazenagemOrigem { get; set; }
        public AreaArmazenagemModel AreaArmazenagemDestino { get; set; }
        public AreaArmazenagemModel AreaArmazenagemLeitura { get; set; }
        public OperadorModel Operador { get; set; }
        public EquipamentoModel Equipamento { get; set; }
        public AtividadeRejeicaoModel AtividadeRejeicao { get; set; }
        public AtividadeModel Atividade { get; set; }
        public StatusChamada Status { get; set; }
        public DateTime? DataChamada { get; set; }
        public DateTime? DataRecebida { get; set; }
        public DateTime? DataAtendida { get; set; }
        public DateTime? DataFinalizada { get; set; }
        public DateTime? DataRejeitada { get; set; }
        public DateTime? DataSuspensa { get; set; }
        public Guid CodigoChamadaSuspensa { get; set; }
    }
}
