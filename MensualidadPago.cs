//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoyVaro
{
    using System;
    using System.Collections.Generic;
    
    public partial class MensualidadPago
    {
        public int IdAmortizacionRespaldo { get; set; }
        public int Id_AplicacionPago { get; set; }
        public int Id { get; set; }
        public int IdCredito { get; set; }
        public int Mensualidad { get; set; }
        public System.DateTime FechaPago { get; set; }
        public decimal CapitalDeudor { get; set; }
        public decimal GPS { get; set; }
        public decimal Pension { get; set; }
        public decimal Seguro { get; set; }
        public decimal Interes { get; set; }
        public decimal Amortizacion { get; set; }
        public decimal PagoAcumuladoGastoAdmon { get; set; }
        public decimal PagoAcumuladoGPS { get; set; }
        public decimal PagoAcumuladoPension { get; set; }
        public decimal PagoAcumuladoSeguro { get; set; }
        public decimal PagoAcumuladoInteres { get; set; }
        public decimal PagoAcumuladoInteresMoratorio { get; set; }
        public decimal PagoAcumuladoAmortizacion { get; set; }
        public decimal PagoCapitalDeudor { get; set; }
        public int Mora { get; set; }
        public bool Contabilizado { get; set; }
        public bool Activo { get; set; }
        public Nullable<bool> adjudicado { get; set; }
        public decimal PagoAcumuladoGastosCobrenza { get; set; }
    }
}
