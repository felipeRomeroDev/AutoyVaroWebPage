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
    
    public partial class AjusteDocumental
    {
        public int id { get; set; }
        public decimal DemeritoDocumental { get; set; }
        public string Observaciones { get; set; }
        public int IdCredito { get; set; }
        public bool Revisado { get; set; }
    
        public virtual Credito Credito { get; set; }
    }
}
