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
    
    public partial class Pensionado
    {
        public int id { get; set; }
        public int idCredito { get; set; }
        public int idPension { get; set; }
        public System.DateTime FechaEntrada { get; set; }
        public Nullable<System.DateTime> FechaSalida { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
    
        public virtual Credito Credito { get; set; }
        public virtual Pension Pension { get; set; }
    }
}
