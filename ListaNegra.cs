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
    
    public partial class ListaNegra
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string APaterno { get; set; }
        public string Amaterno { get; set; }
        public string VIN { get; set; }
        public string Placas { get; set; }
        public string CURP { get; set; }
        public Nullable<int> IdTipoRegistroListaNegra { get; set; }
        public Nullable<int> Idcredito { get; set; }
        public string Comentario { get; set; }
        public Nullable<int> IdUser { get; set; }
        public string NombreCompleto { get; set; }
        public Nullable<int> IdSucursal { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public Nullable<int> IdTipoCliente { get; set; }
        public Nullable<int> IdTipoReporteListaNegra { get; set; }
        public string Vehiculo { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Credito Credito { get; set; }
        public virtual TipoRegitroListaNegra TipoRegitroListaNegra { get; set; }
        public virtual User User { get; set; }
        public virtual Sucursal Sucursal { get; set; }
        public virtual TipoDeCliente TipoDeCliente { get; set; }
        public virtual TipoReporteListaNegra TipoReporteListaNegra { get; set; }
    }
}
