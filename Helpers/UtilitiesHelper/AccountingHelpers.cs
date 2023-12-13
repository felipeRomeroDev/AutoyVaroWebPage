using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Helpers.UtilitiesHelper
{
    public static class AccountingHelpers
    {
        private static Entities db = new Entities();
        private static List<ISR> isr = db.ISR.ToList();

        public static decimal getLimiteInferior(decimal Monto)
        {

            return isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault() != null? isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault().LimiteInferior : 0 ;
        }

        public static decimal getLimiteSuperior(decimal Monto)
        {
            return isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault() != null? isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault().LimiteSuperior : 0 ;
        }
        public static decimal getCuotaFija(decimal Monto)
        {
            return isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault() != null ? isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault().CuotaFija : 0;
        }

        public static decimal getTasa(decimal Monto)
        {
            return isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault() != null ? isr.Where(c => c.LimiteInferior <= Monto && c.LimiteSuperior >= Monto).FirstOrDefault().Tasa / 100 : 0;
        }
    }
}