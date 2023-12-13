using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AutoyVaro.Enums
{
    public enum TipoDeIdentificacion : int
    {
        IFE = 1,
        Pasaporte = 2,
        Licencia = 3,
        [Display(Name = "Cédula Profesional")]
        CedulaProfesional = 4,
        [Display(Name = "Tarjeta de Migración")]
        TarjetaDeMigracion = 5
    };
}