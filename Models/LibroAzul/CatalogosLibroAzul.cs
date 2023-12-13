using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using AutoyVaro.Models;
using System.ComponentModel;

namespace AutoyVaro.Models
{
    public class DetalleVersion
    {
        
        public String Clave { get; set; }
        public String Nombre { get; set; }
        public String Marca { get; set; }
        public int IdMarca { get; set; }
        public String Modelo { get; set; }
        public int IdModelo { set; get; }
        public int Anio { get; set; }
        public String Version { get; set; }
        public String Puertas { get; set; }
        public String URLFRENTE { get; set; }
        public String URLTRAS { get; set; }
    }

}