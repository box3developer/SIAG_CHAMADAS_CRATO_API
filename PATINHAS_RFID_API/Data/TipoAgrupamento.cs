using System.ComponentModel.DataAnnotations;

namespace PATINHAS_RFID_API.Data
{

    public enum TipoAgrupamento
    {
        [Display(Name = "Indefinido")]
        Indefinido = 0,

        [Display(Name = "Box")]
        Box = 1,

        [Display(Name = "Saldo")]
        ProdutoCorGrade = 2,

        [Display(Name = "Estoque Estratégico")]
        ProdutoCorTamanho = 3,

        [Display(Name = "Ordem Exportação")]
        OrdemExportacao = 4,

        [Display(Name = "Box SKU")]
        BoxSku = 5,
    }
}
