using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AutoyVaro.Enums
{
    public enum TipoDeArchivo : int
    {
        [Display(Name = "Expediente")]
        Expediente = 1,
        [Display(Name = "Contrato")]
        Contrato = 2,
        [Display(Name = "Comprobantes")]
        Comprobantes = 3,
        [Display(Name = "Tenencia")]
        Tenencia = 4,


    };
}