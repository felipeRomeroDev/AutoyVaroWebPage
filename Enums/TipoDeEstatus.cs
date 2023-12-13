using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoyVaro.Enums
{
    public enum TipoDeEstatus : int
    {
        [Display(Name = "En Proceso")]
        EnProceso = 1,
        Rechazado = 2,
        Autorizado = 3,
        PreAutorizado = 13,
        [Display(Name = "Proceso de Dispersión")]
        ProcesoDeDispersión = 4,
        Predispersión = 5,
        Dispersado = 6,
        Valuado = 7,
        EnValuacion = 8,
        [Display(Name = "Contrato generado")]
        ContratoGenerado = 9,
        Liquidado = 10,
        Cancelado = 11,
        RechazoDefinitivo = 14
    };
}