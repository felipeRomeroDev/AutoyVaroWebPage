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
    
    public partial class ReporteEsperanzaDeCobro
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> FechaReporte { get; set; }
        public Nullable<int> IdCredito { get; set; }
        public string Estatus { get; set; }
        public Nullable<int> IdEstatus { get; set; }
        public Nullable<int> IdSucursal { get; set; }
        public Nullable<int> CIE { get; set; }
        public string Cliente { get; set; }
        public Nullable<decimal> GPS { get; set; }
        public Nullable<decimal> Pension { get; set; }
        public Nullable<decimal> Seguro { get; set; }
        public Nullable<decimal> Interes { get; set; }
        public Nullable<decimal> Amortizacion { get; set; }
        public Nullable<int> Mora { get; set; }
        public Nullable<decimal> Pago { get; set; }
        public Nullable<decimal> UltimoPago { get; set; }
        public Nullable<decimal> PagoEsperanzaReal { get; set; }
        public Nullable<decimal> UnidadVenta { get; set; }
        public Nullable<decimal> PagoCliente { get; set; }
    
        public virtual Credito Credito { get; set; }
        public virtual Sucursal Sucursal { get; set; }
    }
}
