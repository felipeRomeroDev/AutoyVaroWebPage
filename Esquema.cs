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
    
    public partial class Esquema
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Esquema()
        {
            this.Credito = new HashSet<Credito>();
            this.Fin_Solicitud = new HashSet<Fin_Solicitud>();
        }
    
        public int id { get; set; }
        public string Descripción { get; set; }
        public int Dias { get; set; }
        public bool Activo { get; set; }
        public decimal porcentajeInteres { get; set; }
        public decimal porcentajeInteresSinAmortizacion { get; set; }
        public Nullable<decimal> PorcentajeAperturaCredito { get; set; }
        public Nullable<decimal> PorcentajeAperturaCreditoClientePreferente { get; set; }
        public decimal GastosAdministrativosPostEntregaLiquida { get; set; }
        public decimal GastosAdministrativosPreEntregaLiquida { get; set; }
        public string DescripcionProducto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Credito> Credito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fin_Solicitud> Fin_Solicitud { get; set; }
    }
}
