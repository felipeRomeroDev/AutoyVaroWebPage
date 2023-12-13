using System.ComponentModel.DataAnnotations;

namespace AutoyVaro.Enums
{
    public enum TipoEstatusPago:int
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,
        [Display(Name = "Programado")]
        Programado = 2,
        [Display(Name = "Pagado")]
        Pagado = 3,
        [Display(Name = "Facturado")]
        Facturado = 4
    }
}