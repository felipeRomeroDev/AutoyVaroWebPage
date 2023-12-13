using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models
{
    public class ListaCounsultasLA
    {
        public int? Anio { set; get;}
        public string Marca { set; get; }
        public string Modelo { set; get; }
        public string Version { set; get; }
        public decimal? Compra { set; get; }
        public decimal? Venta { set; get; }
        public string Moneda { set; get; }
    }
}