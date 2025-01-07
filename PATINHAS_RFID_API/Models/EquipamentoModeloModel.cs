﻿using PATINHAS_RFID_API.Data;

namespace PATINHAS_RFID_API.Models;

public class EquipamentoModeloModel
{
    public int Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public StatusModeloEquipamento Status { get; set; }
}
