using AutoyVaro.Helpers.UtilitiesHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace AutoyVaro.Extenciones
{
    public static class StringExtensions
    {
       
        public static string TruncarCadena(this string cadena, int longitud)
        {
            if(!String.IsNullOrEmpty( cadena))
            { cadena = cadena.Length <= longitud ? cadena : cadena.Substring(0, longitud) + "...";}
            else
            {
                cadena = "";
            }
            return cadena;
        }

        public static string MontoALetra(this string monto)
        {
            Numalet let;
            let = new Numalet();
            //al uso en México (creo):
            let.MascaraSalidaDecimal = "00/100 M.N.";
            let.SeparadorDecimalSalida = "pesos";
            //observar que sin esta propiedad queda "veintiuno pesos" en vez de "veintiún pesos":
            let.ApocoparUnoParteEntera = true;
            return let.ToCustomCardinal(monto);

        }
        public static string MontoALetraPorcentajePorcentaje(this string monto)
        {
            Numalet let;
            let = new Numalet();
            let.porcentaje = true;
            let.MascaraSalidaDecimal = " ";
            let.SeparadorDecimalSalida = " ";
            let.ApocoparUnoParteEntera = true;
            return let.ToCustomCardinal(monto);

        }

        public static string FechaExtendida(this DateTime fecha)
        {

            
            string dia = fecha.Day.ToString();
            string mes = fecha.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));
            string año = fecha.Year.ToString();
            return  "día " + dia + " del mes " + mes + " del año " + año;
        }

        public static string FechaExtendida(this DateTime? fecha)
        {

            if (fecha != null)
            {
                string dia = fecha.Value.Day.ToString();
                string mes = fecha.Value.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-MX"));
                string año = fecha.Value.Year.ToString();
                return "día " + dia + " del mes " + mes + " del año " + año;

            }
            else
                return "";

        }

        public static string ToCurrencyFormat(this decimal monto)
        {
            return String.Format("{0:C}", monto);
        }

        public static string ToCurrencyFormat(this double monto)
        {
            return String.Format("{0:C}", monto);
        }

        public static string ToTranslatedBooleanValue(this bool valor)
        {
            return valor == false ? "No" : "Si";
        }

        public static decimal PorcentajeToFormat100(this decimal porcentaje)
        {
            return porcentaje * 100;
        }
    }

}