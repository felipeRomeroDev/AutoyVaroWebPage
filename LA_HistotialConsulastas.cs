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
    
    public partial class LA_HistotialConsulastas
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdSucursal { get; set; }
        public System.DateTime FechaConsulta { get; set; }
        public Nullable<int> IdAnioLA { get; set; }
        public Nullable<int> IdMarcaLA { get; set; }
        public Nullable<int> IdModeloLA { get; set; }
        public string ClaveVercion { get; set; }
        public Nullable<decimal> Compra { get; set; }
        public Nullable<decimal> Venta { get; set; }
        public string Moneda { get; set; }
        public int IdPaqueteConsulas { get; set; }
    
        public virtual LA_Anios LA_Anios { get; set; }
        public virtual User User { get; set; }
        public virtual LA_Marcas LA_Marcas { get; set; }
        public virtual LA_Modelos LA_Modelos { get; set; }
        public virtual LA_Version LA_Version { get; set; }
        public virtual LA_PaqueteConsultas LA_PaqueteConsultas { get; set; }
    }
}
