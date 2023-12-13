using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models.Auxiliares
{
    public class PagoMora
    {
        
        public decimal monto { get; set; }
        public Int32 idMensualidad { get; set; }
        public String conceptoMora { get; set; }
        public DateTime fecha { get; set; }

        public List<PagoMora> toListPagos(string listaPagosMora) {
            var listMoras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExpandoObject>>(listaPagosMora);
            List<PagoMora> result = new List<PagoMora>(); 
            foreach (dynamic prod in listMoras)            {
                PagoMora p = new PagoMora();
                p.idMensualidad = int.Parse(prod.idMensualidad);
                p.monto = Decimal.Parse(prod.monto);
                p.conceptoMora = prod.conceptoMora;
                result.Add(p);                
            }
                return result;
        }
    }
}