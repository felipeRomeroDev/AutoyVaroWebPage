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
    
    public partial class Vehiculo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehiculo()
        {
            this.Credito = new HashSet<Credito>();
            this.ControlDeCambios = new HashSet<ControlDeCambios>();
        }
    
        public int id { get; set; }
        public int idCliente { get; set; }
        public int IdMarca { get; set; }
        public int IdSubmarca { get; set; }
        public int IdModelo { get; set; }
        public int Transmision { get; set; }
        public string Color { get; set; }
        public string NumeroDeSerie { get; set; }
        public string NumeroDeMotor { get; set; }
        public string Placas { get; set; }
        public int Puertas { get; set; }
        public int Asientos { get; set; }
        public int Cilindros { get; set; }
        public bool AireAcondicionado { get; set; }
        public string TarjetaDeCirculacion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<int> Kilometraje { get; set; }
        public string NumeroDeFactura { get; set; }
        public System.DateTime FechaFactura { get; set; }
        public Nullable<int> IdEstado { get; set; }
        public Nullable<System.DateTime> FechaAdjudicacion { get; set; }
        public string RFC { get; set; }
        public bool EsLegalizado { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Credito> Credito { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Marca Marca { get; set; }
        public virtual Modelo Modelo { get; set; }
        public virtual Submarca Submarca { get; set; }
        public virtual TipoTransmision TipoTransmision { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ControlDeCambios> ControlDeCambios { get; set; }
    }
}
