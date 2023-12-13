using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoyVaro.Extenciones;
using System.ComponentModel;

namespace AutoyVaro
{
    [MetadataType(typeof(MetadataCredito))]
    public partial class Credito
    {

        //decimal gastoAdministrativo = Utilities.MontoMasIVA(SettingsHelper.getGastoAdministrativo());

        [Display(Name = "Última observación")]
        public Observacion UltimaObservacion
        {
            get
            {

                return Observacion.OrderByDescending(i => i.IdCredito).LastOrDefault();
            }
        }

        public String NombreCompleto
        {
            get
            {
                return "Monto: " + AvaluoProcasa.ToCurrencyFormat() + " Plazo(días): " + Plazo;
            }
        }

        [DataType(DataType.Currency)]
        public decimal AvaluoDeMercadoSuma
        {
            get
            {
                decimal subTotal = 0;
                decimal cien = 100;
                decimal totalAjusteFisico = this.AjusteFisico.Count == 0 ? 0 : AjusteFisico.Single().AjusteTotalFisico;
                decimal totalAjusteDocumental = this.AjusteDocumental.Count == 0 ? 0 : AjusteDocumental.Single().DemeritoDocumental;
                decimal ajusteDeKilometraje = ((AjustePorKilometraje / cien) * PrecioDeCompra);

                decimal demerito = DemeritoUnidad ?? 0;
                

                subTotal = PrecioDeCompra - totalAjusteFisico - totalAjusteDocumental + ajusteDeKilometraje - demerito;
                return subTotal;
            }
        }

        [DataType(DataType.Currency)]
        public decimal GetAvaluoProcasa
        {
            get
            {
                decimal avaluoProcasa = 0;
                avaluoProcasa = AvaluoDeMercadoSuma * PorcentajeAvaluo;
                return avaluoProcasa;

            }
        }

        [DataType(DataType.Currency)]
        public decimal GetAvaluoProcasaSinSeguro
        {
            get
            {
                decimal avaluoProcasa = 0;
                decimal gps = 0;// getPlazoMes * SettingsHelper.getGPS();
                avaluoProcasa = ((AvaluoDeMercadoSuma * (decimal)0.90) * PorcentajeAvaluo) + gps;
                return avaluoProcasa;

            }
        }


        [DataType(DataType.Currency)]
        public decimal GetInteresDeAvaluoProcasa
        {
            get
            {

                decimal interesAvaluo = GetAvaluoProcasa * PorcentajeInteres;
                return interesAvaluo;
            }
        }

        [DataType(DataType.Currency)]
        public decimal GetMontoMaximoAPrestar
        {
            get
            {
                decimal MontoMaximoAPrestar = 0;
                MontoMaximoAPrestar = GetAvaluoProcasa;
                return MontoMaximoAPrestar;

            }
        }

        [DataType(DataType.Currency)]
        public decimal GetLiquido
        {
            get
            {
                decimal liquido = 0;
                liquido = GetMontoMaximoAPrestar - Utilities.MontoMasIVA(AperturaCredito);
                return liquido;
            }
        }

        [DataType(DataType.Currency)]
        public decimal GetComisionDeVenta
        {
            get
            {
                decimal comision = 0;
                comision = AvaluoProcasa * .03m;
                return comision;
            }
        }

        public int getPorcentajeDeProceso
        {
            get
            {

                switch (IdTipoEstatusCredito)
                {
                    case 1: return 10;
                    case 3: return 50;
                    case 5: return 80;
                    case 6: return 100;
                    default: return 0;

                }

            }
        }

        public DateTime FechaFin
        {
            get
            {
                if (PromocionSucursal != null)
                    return FechaCredito.AddDays(Plazo + (PromocionSucursal.Promocion.NumeroMeses * 30));
                else
                    return FechaCredito.AddDays(Plazo);
            }
        }

        public double getFactor
        {
            get
            {

                double factorPotencia = 0;
                decimal factorA = 0;
                double factor;

                factorA = 1 + Helpers.UtilitiesHelper.Utilities.MontoMasIVA(PorcentajeInteres);

                //Calcular factor 1-((1 + interes)^-plazo) / interes
                factorPotencia = 1 - Math.Pow(Convert.ToDouble(factorA), Convert.ToDouble(-(getPlazoMes)));
                factor = factorPotencia / Convert.ToDouble(Helpers.UtilitiesHelper.Utilities.MontoMasIVA(PorcentajeInteres));

                return factor;
            }
        }
        [DataType(DataType.Currency)]
        public double getPagoMensual
        {
            get
            {
                return Convert.ToDouble(AvaluoProcasa) / getFactor;
            }
        }

        [DataType(DataType.Currency)]
        public decimal getPagoTotalATerceros
        {
            get
            {
                return AdeudoTenencia + AdeudoCirculacion + AdeudoOtros;
            }
        }

        [DataType(DataType.Currency)]
        public double getPagoMensualConGastosAdministrativos
        {
            get
            {
                return getPagoMensual + Convert.ToDouble(GastoAdministrativo) + Convert.ToDouble(Seguro);
            }
        }

        public int getPlazoMes
        {
            get
            {
                return Plazo / 30;
            }
        }
        //Es el pago mensua por el plazo en meses
        public double getPagoTotal
        {
            get
            {
                if (TipoDeCredito != null)
                {
                    if (TipoDeCredito.Amortiza == true)
                    {
                        return getPagoMensualConGastosAdministrativos * getPlazoMes;
                    }
                    else
                    {
                        return (double)getPagoTotalSinAmortizacionSinPension;
                    }
                }
                return 0;
            }
        }


        public double getAdministrativosSeguroGPS {
            get
            {
                decimal n = 0;
                if (IdPromocionSucursal != null) {
                    n = 1;
                }
                    return Convert.ToDouble(n) * (Convert.ToDouble(GastoAdministrativo) + Convert.ToDouble(Seguro));                 
            }
        }

        public decimal getPagoTotalConBonificacion
        {
            get
            {
                return getPagoTotalSinAmortizacionSinPension - getBonificacion;

            }
        }

        public decimal getBonificacion
        {
            get
            {
                return (AvaluoProcasa * getPlazoMes) / 100;
            }
        }

        public decimal GetPorcentajeInteres
        {
            get
            {
                return PorcentajeInteres * 100;

            }
        }
        public decimal GetPorcentajeInteresAnual
        {
            get
            {
                return GetPorcentajeInteres * 12;

            }
        }


        public decimal GetGastoAdministrativo
        {
            get
            {
                return GastoAdministrativo + Seguro;

            }
        }


        public decimal getPagoTotalSinAmortizacionSinPension
        {
            get
            {
                decimal capital = 0;
                decimal amortizacionValor = 0;
                decimal sumaTotal = 0;
                int plazoMes = getPlazoMes;
                decimal pagoTotal = 0;

                capital = AvaluoProcasa;
                decimal interes = capital * Utilities.MontoMasIVA(PorcentajeInteres, IVA);
                amortizacionValor = interes;
                for (int i = 1; i <= plazoMes; i++)
                {
                    decimal total = capital + interes;
                    if (i == plazoMes) amortizacionValor += capital;
                    sumaTotal = amortizacionValor + Seguro + GastoAdministrativo;
                    pagoTotal += sumaTotal;
                };
                return pagoTotal;
            }
        }



        public decimal getCantidadAPagar
        {
            get
            {
                decimal total = Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).Sum(s => s.GetPagoMensualNeto);
                if (Mensualidad.Where(m => m.Activo == true).Count() > 0 && Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).Count() == 0) {
                    total = Mensualidad.Where(m => m.Mensualidad1 == 1).Sum(s => s.GetPagoMensualNeto);
                }
                return total;
            }
        }

        public int getUltimaMensualidadReportada
        {
            get
            {
                if (Mensualidad.Count == 0) {
                    return 0;
                }
                int numMensualiadad = Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).Count() > 0 ? Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).OrderByDescending(a=> a.Mensualidad1).FirstOrDefault().Mensualidad1 : Mensualidad.Where(m => m.Activo == false).OrderByDescending(a=> a.Mensualidad1).FirstOrDefault().Mensualidad1;
                return numMensualiadad;
            }
        }

        public decimal getPenalizacion
        {
            get
            {
                if (Mensualidad.Count == 0)
                {
                    return 0;
                }
                int numMensualiadad = Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).Count() > 0 ? Mensualidad.Where(m => m.Activo == true && m.FechaPago <= DateTime.Today.AddDays(20)).OrderByDescending(a => a.Mensualidad1).FirstOrDefault().Mensualidad1 : Mensualidad.Where(m => m.Activo == false).Count() > 0 ? Mensualidad.Where(m => m.Activo == false).OrderByDescending(a => a.Mensualidad1).FirstOrDefault().Mensualidad1 : 1;
                if (numMensualiadad < 6)
                {
                    return Mensualidad.Where(m => m.Mensualidad1 > numMensualiadad & m.Mensualidad1 <= 6).Sum(s => s.Interes);
                }
                else {
                    return 0;
                }
                
            }
        }


        public DateTime getFechaDeCortePDF
        {
            get
            {
                if (Mensualidad.Count > 0 & Mensualidad.Where(m => m.FechaPago < DateTime.Today.AddDays(20) && m.Activo == true).Count() > 0)
                    return Mensualidad.Where(m => m.FechaPago < DateTime.Today.AddDays(20) && m.Activo == true).OrderByDescending(or => or.Mensualidad1).FirstOrDefault().FechaPago;
                else
                {
                    if (Mensualidad.Count == Mensualidad.Where(m => m.Activo == true).Count())
                    {
                        if (Mensualidad.Count()==0) {
                            return DateTime.Today;
                        }
                        return Mensualidad.FirstOrDefault().FechaPago;
                    }
                    if (IdTipoEstatusCredito == 6 || IdTipoEstatusCredito == 10)
                    {
                        return Mensualidad.Where(m => m.Activo == false).OrderByDescending(or => or.Mensualidad1).FirstOrDefault().FechaPago;
                    }
                }
                return DateTime.Today;

            }
        }

        public Int32 getPagosVencidos
        {
            get
            {
                return Mensualidad.Count(m => m.FechaPago < DateTime.Today.AddDays(20) && m.Activo == true);
            }
        }

        public decimal CapitalDeudorEstadoCuenta
        {
            get
            {
                var amortizacion = Mensualidad.Where(x => x.FechaPago > DateTime.Today.AddDays(20)).Sum(a => a.Amortizacion);
                return amortizacion;
            }
        }



        public IList<Amortizacion> GetAmortizacion
        {
            get
            {
                decimal capital = 0;
                decimal amortizacionValor = 0;

                decimal sumaTotal = 0;

                int plazoMes = getPlazoMes;
                int MesInicio = 1;
                //DateTime fechaDePago = FechaCredito;
                List<Amortizacion> amortizacion = new List<Amortizacion>();
                if (IdPromocionSucursal != null)
                {
                    capital = AvaluoProcasa;
                    plazoMes += PromocionSucursal.Promocion.NumeroMeses;
                    MesInicio += PromocionSucursal.Promocion.NumeroMeses;
                    for (int i = 1; i <= PromocionSucursal.Promocion.NumeroMeses; i++)
                    {
                        amortizacion.Add(new Amortizacion(Math.Round(capital, 2), 0, Math.Round(capital, 2), 0, Math.Round(GastoAdministrativo, 2), Math.Round(GastoAdministrativo + Seguro, 2), Math.Round(Seguro, 2), getFechaDeCorte(i), i + " de " + plazoMes));
                    }
                }

                //Con amortizacion
                if (TipoDeCredito.Amortiza == true)
                {
                    amortizacionValor = Convert.ToDecimal(getPagoMensual);
                    capital = AvaluoProcasa;

                    for (int i = MesInicio; i <= plazoMes; i++)
                    {
                        decimal interes = capital * PorcentajeInteres;
                        decimal total = capital + Utilities.MontoMasIVA(interes);
                        interes = Utilities.MontoMasIVA(interes, IVA);
                        decimal montoACapital = amortizacionValor - interes;

                        //fechaDePago = fechaDePago.AddDays(30);
                        sumaTotal = (decimal)getPagoMensualConGastosAdministrativos;
                        // amortizacion.Add(new Amortizacion(Math.Round(capital, 2), Math.Round(Utilities.MontoMasIVA(interes), 2), Math.Round(total, 2), Math.Round(amortizacionValor, 2), Math.Round(gastoAdministrativo, 2), Math.Round(Convert.ToDecimal(sumaTotal), 2), Math.Round(Seguro, 2), fechaDePago, i + " de " + plazoMes));
                        amortizacion.Add(new Amortizacion(Math.Round(capital, 2), Math.Round(interes, 2), Math.Round(total, 2), Math.Round(amortizacionValor, 2), Math.Round(GastoAdministrativo, 2), Math.Round(Convert.ToDecimal(sumaTotal), 2), Math.Round(Seguro, 2), getFechaDeCorte(i), i + " de " + plazoMes));
                        //capital = total - amortizacionValor;
                        capital -= montoACapital;

                    };
                }
                else
                {
                    //sin amortizacion                    
                    capital = AvaluoProcasa;
                    decimal interes = capital * PorcentajeInteres;
                    interes = Utilities.MontoMasIVA(interes, IVA);
                    amortizacionValor = interes;
                    for (int i = MesInicio; i <= plazoMes; i++)
                    {
                        decimal total = capital + interes;
                        if (i == plazoMes) amortizacionValor += capital;
                        sumaTotal = amortizacionValor + Seguro + GastoAdministrativo;
                        amortizacion.Add(new Amortizacion(Math.Round(capital, 2), Math.Round(interes, 2), Math.Round(total, 2), Math.Round(amortizacionValor, 2), Math.Round(GastoAdministrativo, 2), Math.Round(sumaTotal, 2), Math.Round(Seguro, 2), getFechaDeCorte(i), i + " de " + plazoMes));
                    };
                }


                return amortizacion;
            }
        }

        public DateTime getFechaDeCorte(int numeroDePago)
        {
            int diasDePago = 30 * numeroDePago;
            DateTime fecha = FechaContrato ?? FechaCredito;
            return fecha.AddDays(diasDePago);
        }

        public int getPlazoEnMeses()
        {
            return Plazo / 30;
        }

    }

    public class MetadataCredito
    {
        [Display(Name = "Identificador")]
        public int id;

        [Display(Name = "Plazo")]
        public int Plazo;

        [Required(ErrorMessage = "El precio de compra es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Display(Name = "Precio de compra")]
        [RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal PrecioDeCompra;

        public int AjustePorKilometraje;

        [Display(Name = "Avalúo de mercado")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal AvaluoMercado;

        [Display(Name = "Avalúo Procasa")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal AvaluoProcasa;

        [Display(Name = "Apertura de crédito")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal AperturaCredito;

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal GastoAdministrativo;

        [Display(Name = "Entrega líquida")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal EntregaLiquida;

        public decimal PorcentajeInteres;

        public decimal IVA;

        [Display(Name = "Fecha del crédito")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM dd, yyyy}")]
        public System.DateTime FechaCredito;

        [Display(Name = "CLABE")]
        public string CLABE;

        public Nullable<int> TipoDePago;

        [Display(Name = "Cuenta")]
        public string Cuenta;

        [Display(Name = "Nombre del Banco")]
        public Nullable<int> IdBanco;

        public Nullable<System.DateTime> FechaDispersion;

        public int idEsquema;

        public bool Activo;

        public int Sucursal;

        public decimal PorcentajeAvaluo;

        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = false)]
        [RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal Seguro;

        [Display(Name = "Fecha de contrato")]
        public Nullable<System.DateTime> FechaContrato;

        [Display(Name = "Adeudo de tenencia")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal AdeudoTenencia;

        [Display(Name = "Adeudo de circulación")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal AdeudoCirculacion;

        [Display(Name = "Otros adeudos")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal AdeudoOtros;

        public bool DispersionCobrada;

        public string NombreAdeudoTenencia;

        public Nullable<int> ReferenciaCIE;

        public string NobreAdeudoOtros;
    }

}