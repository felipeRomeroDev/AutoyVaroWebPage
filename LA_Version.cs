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
    
    public partial class LA_Version
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LA_Version()
        {
            this.LA_HistotialConsulastas = new HashSet<LA_HistotialConsulastas>();
            this.SolicitudCotizacion = new HashSet<SolicitudCotizacion>();
            this.Fit_Vehiculo = new HashSet<Fit_Vehiculo>();
            this.ContactoWeb = new HashSet<ContactoWeb>();
        }
    
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int IdModeloLA { get; set; }
        public bool Active { get; set; }
        public Nullable<decimal> Compra { get; set; }
        public Nullable<decimal> Venta { get; set; }
        public Nullable<System.DateTime> FechaActualizacion { get; set; }
        public Nullable<int> IdAnioLA { get; set; }
        public Nullable<int> IdMarcaLA { get; set; }
    
        public virtual LA_Anios LA_Anios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LA_HistotialConsulastas> LA_HistotialConsulastas { get; set; }
        public virtual LA_Marcas LA_Marcas { get; set; }
        public virtual LA_Modelos LA_Modelos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCotizacion> SolicitudCotizacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fit_Vehiculo> Fit_Vehiculo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactoWeb> ContactoWeb { get; set; }
    }
}
