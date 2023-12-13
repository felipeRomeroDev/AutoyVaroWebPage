using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models.Auxiliares
{
    public class AmortizacionSimulacion
    {
        public int Mensualidad { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal CapitalDeudor { get; set; }
        public decimal Interes { get; set; }
        public decimal Amortizacion { get; set; }
        public decimal Mora { get; set; }
        public decimal Cargos { get; set; }


        public decimal InteresPago { get; set; }
        public decimal MoraPago { get; set; }
        public decimal CargosPago { get; set; }
        public decimal AmortizacionPago { get; set; }
        public decimal capitalPago { get; set; }

        public decimal MontoPago { get; set; }
        public decimal TotalCubierto { get; set; }
        public decimal Faltante { get; set; }



   }


    public class CreditoAmortizacion {

        public Credito credito { get; set; }
        public List<AmortizacionSimulacion> amortizacionSimulacion { get; set; }
        public List<AmortizacionSimulacion> amortizacionSimulacionSinAmortizar { get; set; }

        public int ultimaMensualidad { get; set; }
        public decimal MontoIntereses { get; set; }
        public int plazo { get; set; }
        public decimal ComicionPorAperturaCal { get; set; }
        public decimal intereses { get; set; }
        public decimal moras { get; set; }
        public decimal cargos { get; set; }
        public decimal apertuta { get; set; }
        public decimal total { get; set; }
        public decimal pagoAPertura { get; set; }
        public decimal porcentajeInteres { get; set; }


    }

}