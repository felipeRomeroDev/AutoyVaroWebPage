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
    
    public partial class LA_Modelos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LA_Modelos()
        {
            this.LA_ControlPorcentajeAvaluo = new HashSet<LA_ControlPorcentajeAvaluo>();
            this.LA_HistotialConsulastas = new HashSet<LA_HistotialConsulastas>();
            this.LA_ModelosXMarcaAnio = new HashSet<LA_ModelosXMarcaAnio>();
            this.SolicitudCotizacion = new HashSet<SolicitudCotizacion>();
            this.LA_Version = new HashSet<LA_Version>();
            this.Fit_Vehiculo = new HashSet<Fit_Vehiculo>();
            this.ContactoWeb = new HashSet<ContactoWeb>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Active { get; set; }
        public Nullable<int> IdMarcasLA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LA_ControlPorcentajeAvaluo> LA_ControlPorcentajeAvaluo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LA_HistotialConsulastas> LA_HistotialConsulastas { get; set; }
        public virtual LA_Marcas LA_Marcas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LA_ModelosXMarcaAnio> LA_ModelosXMarcaAnio { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudCotizacion> SolicitudCotizacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LA_Version> LA_Version { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fit_Vehiculo> Fit_Vehiculo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactoWeb> ContactoWeb { get; set; }
    }
}
