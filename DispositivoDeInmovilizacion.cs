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
    
    public partial class DispositivoDeInmovilizacion
    {
        public int id { get; set; }
        public int idEmpresaInmovilizadora { get; set; }
        public int idCredito { get; set; }
        public string Folio { get; set; }
        public System.DateTime FechaDeActivacion { get; set; }
        public bool Activo { get; set; }
        public string Telefono { get; set; }
        public string idDispositivo { get; set; }
    
        public virtual Credito Credito { get; set; }
        public virtual EmpresaInmovilizadora EmpresaInmovilizadora { get; set; }
    }
}