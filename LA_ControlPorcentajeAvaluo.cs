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
    
    public partial class LA_ControlPorcentajeAvaluo
    {
        public int Id { get; set; }
        public int IdAnioLA { get; set; }
        public int IdMarcaLA { get; set; }
        public Nullable<int> IdModeloLA { get; set; }
        public Nullable<decimal> PorcentajeAvaluo { get; set; }
        public Nullable<bool> Activo { get; set; }
    
        public virtual LA_Anios LA_Anios { get; set; }
        public virtual LA_Marcas LA_Marcas { get; set; }
        public virtual LA_Modelos LA_Modelos { get; set; }
    }
}
