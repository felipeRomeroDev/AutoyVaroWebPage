using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoyVaro.Helpers.UtilitiesHelper
{
    public static class Utilities
    {
        public static decimal MontoMasIVA(decimal monto)
        {
            monto = monto * (1 + SettingsHelper.getIVA());
            return monto;
        }
        public static decimal MontoMasIVA(decimal monto, decimal iva)
        {
            monto = monto * (1 + iva);
            return monto;
        }
        public static decimal TruncateDecimal(decimal monto)
        {
            return Math.Truncate(100 * monto) / 100;
        }

        public static decimal RepartirMonto(decimal montoAPagar, ref decimal montoARepartir)
        {
            if (montoAPagar > 0)
            {
                if (montoAPagar <= montoARepartir)
                {
                    montoARepartir -= montoAPagar;
                }

                else
                {
                    montoAPagar = montoARepartir;
                    montoARepartir = 0;
                }
            }

            return montoAPagar;
        }
    }
}