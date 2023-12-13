using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models
{
    public class UsuarioHistorialLA
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Sucursal { get; set; }
        public int Consultas { get; set; }        
    }
}