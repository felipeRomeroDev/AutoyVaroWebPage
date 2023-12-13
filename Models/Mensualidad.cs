using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AutoyVaro
{
    [MetadataType(typeof(MetadataMensualidad))]
    public partial class Mensualidad
    {
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetSumaCargos
        {
            get
            {
                return Cargo.Sum(e => e.Monto);
            }
        }

        [Display(Name = "Pago Mensual")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetPagoMensual
        {
            get
            {
                return GPS + Pension + Seguro + Interes + Amortizacion;
            }
        }


        [Display(Name = "Pago GPS")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetTotalGps
        {
            get
            {
                return GPS - PagoAcumuladoGPS;
            }
        }
        [Display(Name = "Pago Pención")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetTotalPension
        {
            get
            {
                return Pension - PagoAcumuladoPension;
            }
        }
        [Display(Name = "Pago seguro")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetTotalSeguro
        {
            get
            {
                return Seguro - PagoAcumuladoSeguro;
            }
        }
        [Display(Name = "Pago Interes")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetTotalInteres
        {
            get
            {
                return Interes - PagoAcumuladoInteres;
            }
        }


        [Display(Name = "Pago Amortización")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetTotalAmortización
        {
            get
            {
                return Amortizacion - PagoAcumuladoAmortizacion;
            }
        }

        [Display(Name = "Pago Mensual")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal GetPagoMensualNeto
        {
            get
            {
                return GetTotalGps + GetTotalPension + GetTotalSeguro+ GetTotalInteres + GetTotalAmortización+ getMoraTotal;
            }
        }


        public decimal getMoraTotal
        {
            get
            {
                decimal mo = 0;
                if (Mora1.Count==0)
                {
                    return mo;
                }
                mo = Mora1.Sum(m => m.MontoAplicado);
                if (!Activo) {
                    return 0;
                }

                return mo==0 ? CalculateMoratoryInterest : mo;
            }
        }


        public decimal CapitalDeudorEstadoCuenta {
            get
            {
                var amortizacion = Credito.Mensualidad.Where(x => x.FechaPago> DateTime.Today).Sum(a=> a.Amortizacion);
                return amortizacion;
            }    
        }


        public decimal CalculateMoratoryInterest
        {
            get
            {
                /*Los intereses moratorios totales es la suma los intereses moratorios de cada mensualidad activa
                 * -------------------------------------
                 * El interés moratorio de una mensualidad es: 
                 * la multiplicación del 6% de la suma de gps + pensión + seguro+ interés + amortización
                 * por la mora, restando: el monto acumulado de interés moratorio  y el acumulado de condonación
                 * (una o más condonaciones pueden afectar a una mensualidad)
                 * ---------------------------------------
                 *  
                 */
                //var Mounthly = db.Mensualidad.Find(IdMounthly);
                int mora_tem = 0;
                if (Mora > 0) { mora_tem = Mora; }
                var MontoCondonacion = 0;//Condonacion.Where(e => e.Activo = true && e.TipoCondonacion.Administrativo == false).Sum(e => e.Monto);
                var MontoCargoMoratorio = 0; //Cargo.Where(e => e.TipoCargo.Administrativo == false && e.TipoCargo.Id != ).Sum(e => e.Monto);
                var MontoMoratorios = (mora_tem * (Convert.ToDecimal(0.0696) * (GPS + Seguro + Pension + Interes + Amortizacion))) - PagoAcumuladoInteresMoratorio - MontoCondonacion;
                MontoMoratorios += MontoCargoMoratorio;
                return MontoMoratorios;
            }
        }
    }

    public class MetadataMensualidad
    {
        [DisplayFormat(DataFormatString = "{0:dd MMMM yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago;

        [Required(ErrorMessage = "El capital deudor es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Capital Deudor")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal CapitalDeudor;

        [Required(ErrorMessage = "El GPS es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "GPS")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal GPS;

        [Required(ErrorMessage = "La pensión es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Pensión")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal Pension;

        [Required(ErrorMessage = "El seguro es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Seguro")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal Seguro;

        [Required(ErrorMessage = "El interés es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Interés")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal Interes;

        [Required(ErrorMessage = "La amortización es un campo requerido")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [Display(Name = "Amortización")]
        //[RegularExpression(@"^\$?([1-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$", ErrorMessage = "{0} debe ser una cantidad en números.")]
        public decimal Amortizacion;

        [Display(Name = "Mora")]
        public int Mora;

        [Display(Name = "Mensualidad")]
        public int Mensualidad1;

    }
}