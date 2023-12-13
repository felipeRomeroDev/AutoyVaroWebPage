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
    
    public partial class ControlDeCambios
    {
        public int Id { get; set; }
        public string DataJSON { get; set; }
        public Nullable<System.DateTime> FechaDeCambio { get; set; }
        public string Comentario { get; set; }
        public Nullable<int> IdUsuario { get; set; }
        public Nullable<int> IdTipoCambio { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public Nullable<int> IdCredito { get; set; }
        public Nullable<int> IdVehiculo { get; set; }
        public Nullable<int> Version { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Credito Credito { get; set; }
        public virtual TipoCambio TipoCambio { get; set; }
        public virtual User User { get; set; }
        public virtual Vehiculo Vehiculo { get; set; }
    }
}
