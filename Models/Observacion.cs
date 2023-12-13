using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoyVaro
{
    [MetadataType(typeof(MetadataObservacion))]

    public partial class Observacion
    {
    }
    public class MetadataObservacion
    {
        [Display(Name="Identificador")]
        public int IdObservacion;


        [DataType(DataType.MultilineText)]
        [Display(Name="Observación")]
        public string Observacion1;

       
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        [Display(Name = "Fecha")]
        public System.DateTime FechaObservacion;

        public bool Publicar;

       
    }
}