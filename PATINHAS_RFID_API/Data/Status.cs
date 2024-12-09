namespace PATINHAS_RFID_API.Data
{
    public enum Status
    {
        //Referente a Todos
        Indefinido = 0,
        Inativo = 1,
        Ativo = 2,
        Manutencao = 3,
        Ocupado = 4,
        //Referente a Caixa
        Sorter = 20,
        Armazenado = 21,
        Expedido = 22,
        //Referente a Pedido
        Cancelado = 40,
        Faturado = 41,
        CanceladoInadimplencia = 42,
        //Referente a Endereço
        Reservado = 50,
    }
}
