using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Helpers.UtilitiesHelper
{

    public static class SettingsHelper
    {
        private static Entities db = new Entities();
        private static Settings settings = db.Settings.Single();
        private static List<Esquema> listaPorcentaje = db.Esquema.Where(e => e.Activo == true).ToList();

        public static decimal getPension()
        {

            return settings.Pension;
        }

        public static decimal getIVA()
        {

            return settings.IVA / 100;
        }

        public static decimal getPorcentajeApertura(int idEsquema, bool clientePreferente)
        {
            Esquema esquema = listaPorcentaje.Find(p => p.id == idEsquema);
            if (clientePreferente) {
                return (esquema.PorcentajeAperturaCreditoClientePreferente ?? 0) / 100;
            }
            return (esquema.PorcentajeAperturaCredito ?? 0) / 100;
        }

        public static decimal getPorcentajeAperturaParche(int idEsquema, bool clientePreferente)
        {
            Esquema esquema = listaPorcentaje.Find(p => p.id == idEsquema);
            if (clientePreferente)
            {
                return (esquema.PorcentajeAperturaCreditoClientePreferente ?? 0) / 100;
            }
            return ((esquema.PorcentajeAperturaCredito ?? 0)-1) / 100;
        }


        public static decimal getPorcentajeAvaluoProcasa()
        {
            return settings.PorcentajeAvaluoProcasa / 100;
        }

        public static decimal getGastoAdministrativo()
        {
            return settings.GastosaAdministrativos;
        }

        public static decimal getPorcentajeCastigo() {
            return settings.PorcentajeCastigo;
        }
        public static decimal getPorcentajeCreditoLimpio()
        {
            return settings.PorcentajeCreditoLimpio;
        }



        public static decimal getPorcentajeIntereses(int idPlazo)
        {
            decimal porcentage = listaPorcentaje.Find(p => p.id == idPlazo).porcentajeInteres;
            return porcentage / 100;
        }

        public static decimal getPorcentajeInteresesSinAmortizacion(int idPlazo)
        {
            decimal porcentage = listaPorcentaje.Find(p => p.id == idPlazo).porcentajeInteresSinAmortizacion;
            return porcentage / 100;
        }

        public static string getRutaDirectorio()
        {
            return settings.RutaDeArchivos;
        }

        public static string getRutaDirectorioWeb()
        {
            return settings.RutaDeArchivosWeb;
        }

        public static decimal getGPS()
        {
            return settings.GPS;
        }
        public static int getModeloMinimo()
        {
            return settings.ModeloMinimo ?? 0;
        }
        public static int getDiasToleranciaMora()
        {
            return settings.diasToleranciaMora;
        }
    }
}