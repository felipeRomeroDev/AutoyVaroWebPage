using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models
{
    public class ViewModelSolicitud
    {
        public int IdSolicitud { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string NumeroCasa { get; set; }
        public string EntreCalle { get; set; }
        public string YCalle { get; set; }
        public int CodigoPostal { get; set; }
        public string Ciudad { get; set; }
        public string Municipio { get; set; }
        public int IdEstado { get; set; }

        public int IdTipoDeIdentificacion { get; set; }
        
        public string TelefonoCasa { get; set; }
        public string TelefonoOficina { get; set; }
        public string Ext { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string NumeroDeSerie { get; set; }
        public string NumeroDeMotor { get; set; }
        public string Placas { get; set; }
        public string TarjetaDeCirculacion { get; set; }
        public Nullable<int> IdEstadoPlacas { get; set; }        
        public System.DateTime FechaSolicitud { get; set; }
        public decimal Monto { get; set; }

        public string ActividadPreponderante { get; set; }

        public int IdEstadoCivil { get; set; }

        public string Color { get; set; }
        public string RFCFactura { get; set; }

        public string NumeroFactura { get; set; }
        public Nullable<System.DateTime> FechaFactura { get; set; }

        public string NumeroDeTarjetaDeCirculacion { get; set; }

        public int NumeroDePuertas { get; set; }
        public int NumeroDeAsientos { get; set; }
        public int NumeroDeCilindros { get; set; }
        public int IdTipoTransmision { get; set; }

        public string Nombre1 { get; set; }
        public string ApellidoPaterno1 { get; set; }
        public string ApellidoMaterno1 { get; set; }
        public string ParentescoR1 { get; set; }
        public string DireccionR1 { get; set; }
        public string TelefonoR1 { get; set; }
        public string Nombre2 { get; set; }
        public string ApellidoPaterno2 { get; set; }
        public string ApellidoMaterno2 { get; set; }
        public string TelefonoR2 { get; set; }

        public string Nombre3 { get; set; }
        public string ApellidoPaterno3 { get; set; }
        public string ApellidoMaterno3 { get; set; }
        public string TelefonoR3 { get; set; }
        public int? tipoDeSeguiniento { get; set; }       

    }

    public class EventViewModel
    {
        public Int64 id { get; set; }

        public String title { get; set; }

        public String start { get; set; }

        public String end { get; set; }

        public bool allDay { get; set; }

        public bool editable { get; set; }

        public String description { get; set; }

        public bool inUse { get; set; }

        public bool holiday { get; set; }

        public bool overtime { get; set; }


    }
}