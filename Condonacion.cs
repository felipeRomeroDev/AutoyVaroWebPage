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
    
    public partial class Condonacion
    {
        public int Id { get; set; }
        public int IdMensualidad { get; set; }
        public int Tipo { get; set; }
        public System.DateTime FechaCaptura { get; set; }
        public System.DateTime FechaLimite { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoAplicado { get; set; }
        public int Estatus { get; set; }
        public bool Activo { get; set; }
    
        public virtual TipoCondonacion TipoCondonacion { get; set; }
        public virtual EstatusCondonacion EstatusCondonacion { get; set; }
        public virtual Mensualidad Mensualidad { get; set; }
    }
}