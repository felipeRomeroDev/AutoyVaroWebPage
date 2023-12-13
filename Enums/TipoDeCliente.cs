using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoyVaro.Enums
{
    public enum TipoDeCliente : int
    {
        [Display(Name = "Persona Física")]
        fisica = 1,
        [Display(Name = "Persona Moral")]
        moral = 2,
     
        
    };
}