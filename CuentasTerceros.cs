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
    
    public partial class CuentasTerceros
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CuentasTerceros()
        {
            this.PagosTerceros = new HashSet<PagosTerceros>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public bool Activo { get; set; }
    
        public virtual Banco Banco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagosTerceros> PagosTerceros { get; set; }
    }
}
