using System.ComponentModel.DataAnnotations;

namespace PATINHAS_RFID_API.Data
{
    public enum Ativo
    {
        [Display(Name = "Inativo")]
        Inativo = 0,
        [Display(Name = "Ativo")]
        Ativo = 1
    }
}
