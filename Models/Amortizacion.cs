using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoyVaro.Models
{
    public class Amortizacion
    {
        public Amortizacion(decimal capital, decimal interes, decimal total, decimal amortizacion, decimal gastoAdministrativo, decimal pagoTotalMensual, decimal seguro, DateTime fechaDePago, string consecutivo)
        {
            Capital = capital;
            Interes = interes;
            Total = total;
            valor = amortizacion;
            GastoAdministrativo = gastoAdministrativo;
            Seguro = seguro;
            FechaDePago = fechaDePago;
            Consecutivo = consecutivo;
            PagoTotalMensual = pagoTotalMensual;
            MontoPagadoACapital = amortizacion - interes;
            MontoCapitalMasGastos = MontoPagadoACapital + seguro + gastoAdministrativo;

        }
        public Amortizacion(decimal capital, decimal interes, decimal total, decimal amortizacion, decimal gastoAdministrativo, decimal pagoTotalMensual, decimal seguro, DateTime fechaDePago, string consecutivo, decimal amortizacionMes)
        {
            Capital = capital;
            Interes = interes;
            Total = total;
            valor = amortizacion;
            GastoAdministrativo = gastoAdministrativo;
            Seguro = seguro;
            FechaDePago = fechaDePago;
            Consecutivo = consecutivo;
            PagoTotalMensual = pagoTotalMensual;
            MontoPagadoACapital = amortizacionMes;
            MontoCapitalMasGastos = MontoPagadoACapital + seguro + gastoAdministrativo;

        }

        public String Consecutivo { get; set; }
        public decimal Capital { get; set; }
        public decimal Interes { get; set; }
        public decimal Total { get; set; }
        public decimal valor { get; set; }
        public decimal GastoAdministrativo { get; set; }
        public decimal PagoTotalMensual { get; set; }
        public decimal Seguro { get; set; }
        public decimal MontoPagadoACapital { get; set; }
        public decimal MontoCapitalMasGastos { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaDePago { get; set; }

        public decimal getInteresMensualidadMenosPagoTotal()
        {
            return PagoTotalMensual - MontoPagadoACapital;
        }
        public decimal GetAmortizacion()
        {
            return valor - Interes;
        }
    }
}