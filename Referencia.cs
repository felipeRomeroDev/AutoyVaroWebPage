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
    
    public partial class Referencia
    {
        public int idReferencia { get; set; }
        public string Nombre1 { get; set; }
        public string ApellidoPaterno1 { get; set; }
        public string ApellidoMaterno1 { get; set; }
        public string TelefonoR1_1 { get; set; }
        public string TelefonoR1_2 { get; set; }
        public int idCredito { get; set; }
        public string Nombre2 { get; set; }
        public string ApellidoPaterno2 { get; set; }
        public string ApellidoMaterno2 { get; set; }
        public string TelefonoR2_1 { get; set; }
        public string TelefonoR2_2 { get; set; }
        public string Nombre3 { get; set; }
        public string ApellidoPaterno3 { get; set; }
        public string ApellidoMaterno3 { get; set; }
        public string TelefonoR3_1 { get; set; }
        public string TelefonoR3_2 { get; set; }
        public string ParentescoR1 { get; set; }
        public string DireccionR1 { get; set; }
    
        public virtual Credito Credito { get; set; }
    }
}