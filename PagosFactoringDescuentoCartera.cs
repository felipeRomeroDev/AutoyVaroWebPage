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
    
    public partial class PagosFactoringDescuentoCartera
    {
        public int Id { get; set; }
        public int IdCredito { get; set; }
        public decimal Monto { get; set; }
        public System.DateTime FechaSolicitud { get; set; }
        public int IdUsuario { get; set; }
        public Nullable<int> IdCargaPlantilla { get; set; }
        public Nullable<bool> Activo { get; set; }
        public decimal Importemensual { get; set; }
    
        public virtual Credito Credito { get; set; }
        public virtual User User { get; set; }
    }
}
