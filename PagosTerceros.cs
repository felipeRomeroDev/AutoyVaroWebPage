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
    
    public partial class PagosTerceros
    {
        public int Id { get; set; }
        public int IdCredito { get; set; }
        public int IdCuentaTerceros { get; set; }
        public decimal Monto { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public string Descripción { get; set; }
    
        public virtual Credito Credito { get; set; }
        public virtual CuentasTerceros CuentasTerceros { get; set; }
    }
}