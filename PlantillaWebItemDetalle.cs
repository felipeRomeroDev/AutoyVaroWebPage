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
    
    public partial class PlantillaWebItemDetalle
    {
        public int Id { get; set; }
        public Nullable<int> PlantillaWebItemId { get; set; }
        public Nullable<int> PlantillaWebItemHijoId { get; set; }
    
        public virtual PlantillaWebItem PlantillaWebItem { get; set; }
        public virtual PlantillaWebItem PlantillaWebItem1 { get; set; }
    }
}
