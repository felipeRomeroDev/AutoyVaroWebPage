using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models
{
    public class ViewModelCreditos
    {     
        public SolicitudCotizacion solicitudCotizacion { get; set; }
        public List<Credito> creditos { get; set; }

    }

    public class ViewModelSolicitudCredito
    {
        public Fin_Solicitud solicitud { get; set; }
        public Credito creditos { get; set; }
    }
}