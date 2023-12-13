using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AutoyVaro.Models
{
    public class ClientePeticionModel
    {
        [Required]
        [Display(Name = "Número de Crédito")]
        public int NumeroCredito { get; set; }
        [Required]
        [Display(Name = "RFC")]
        public string RFC { get; set; }
        [Required]
        [Display(Name = "Clave de Seguridad")]
        public string ClaveSeguridad { get; set; }

    }
}