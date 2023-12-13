using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace AutoyVaro
{
    [MetadataType(typeof(MetadataContactoWeb))]
    public partial class ContactoWeb
    {
    }

    public class MetadataContactoWeb {

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "La fecha es requerida")]
        public System.DateTime FechaSolicitud;

        [Display(Name = "Año")]
        [Required(ErrorMessage = "El año es requerido")]
        public int IdAnioLA;

        [Display(Name = "Marca")]
        [Required(ErrorMessage = "La marca es requerida")]
        public int IdMarcaLA;

        [Display(Name = "Modelo")]
        [Required(ErrorMessage = "El modelo es requerido")]
        public int IdModeloLA;

        [Display(Name = "Versión")]
        [Required(ErrorMessage = "La versión es requerida")]
        public string ClaveVercion;

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string NombreCopleto;

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El telefono es requerido")]
        public string Telefono;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El email es requerido")]
        public string Email;        
        
        public string IdUsuarioSolicitud;

        [Display(Name = "Comentario")]        
        public string Comentario;

    }
}