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
    
    public partial class AplicacionPago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AplicacionPago()
        {
            this.Cargo = new HashSet<Cargo>();
            this.DetalleDePago = new HashSet<DetalleDePago>();
            this.CrucePagoMensualidad = new HashSet<CrucePagoMensualidad>();
        }
    
        public int Id { get; set; }
        public int IdCredito { get; set; }
        public System.DateTime FechaPago { get; set; }
        public System.DateTime FechaCaptura { get; set; }
        public decimal Monto { get; set; }
        public decimal GPS { get; set; }
        public decimal Pension { get; set; }
        public decimal GastosAdmon { get; set; }
        public decimal Seguro { get; set; }
        public decimal Interes { get; set; }
        public decimal Moratorios { get; set; }
        public decimal Capital { get; set; }
        public bool Contabilizado { get; set; }
        public Nullable<int> IdCuentasPagos { get; set; }
        public string Concepto { get; set; }
        public decimal GastosCobrenza { get; set; }
        public Nullable<int> IdTipoPago { get; set; }
        public Nullable<decimal> Devolucion { get; set; }
        public Nullable<decimal> CapitalCondonado { get; set; }
        public Nullable<decimal> MontoNotaDeCredito { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cargo> Cargo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetalleDePago> DetalleDePago { get; set; }
        public virtual Credito Credito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CrucePagoMensualidad> CrucePagoMensualidad { get; set; }
        public virtual CuentasPagos CuentasPagos { get; set; }
        public virtual TipoPago TipoPago { get; set; }
    }
}
