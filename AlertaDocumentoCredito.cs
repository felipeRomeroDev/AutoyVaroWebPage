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
    
    public partial class AlertaDocumentoCredito
    {
        public int Id { get; set; }
        public int IdDocumentoCredito { get; set; }
        public string Alerta { get; set; }
        public int IdEstadoAlertaDocumento { get; set; }
        public int IdUsuario { get; set; }
        public System.DateTime FechaObservacion { get; set; }
    
        public virtual DocumentacionCredito DocumentacionCredito { get; set; }
        public virtual EstadoAlertaDocumento EstadoAlertaDocumento { get; set; }
        public virtual User User { get; set; }
    }
}