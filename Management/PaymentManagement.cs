using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Models;
using AutoyVaro.Models.Auxiliares;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AutoyVaro.Management
{
    public class PaymentManagement
    {
        //instanciar crédito a tratar
        Credito Credit = new Credito();
        //Instanciar las entidades
        private Entities db = new Entities();

        //Constructores
        public PaymentManagement()
        {
        }
        public PaymentManagement(int IdCredito)
        {
            this.Credit = db.Credito.Find(IdCredito);

        }

        //Administración de pagos

        public void EntersPayment(decimal Amount, DateTime DateOfPaymentA)
        {
            #region
            var mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true && e.FechaPago.AddDays(10) > DateOfPaymentA).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mesAplicable == null)
            {
                mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true).OrderByDescending(e => e.Mensualidad1).FirstOrDefault();
            }
            DateTime DateOfPayment = mesAplicable.FechaPago;

            var mes = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mes.FechaPago > DateOfPayment && mes.Mensualidad1 > 1)
            {
                var mes_update = Credit.Mensualidad.Where(e => e.Id == (mes.Id - 1)).FirstOrDefault();
                mes_update.Activo = true;
                db.Entry(mes_update).State = EntityState.Modified;
                db.SaveChanges();
            }

            /*Variable declaration*/
            decimal AmountToApply = Amount;
            //Valor acumulado por concepto aplicado al pago
            decimal AdministrativeEAmountApplied = 0;
            decimal GPSAmountApplied = 0;
            decimal PensionAmountApplied = 0;
            decimal InsuranceAmountApplied = 0;
            decimal InterestAmountApplied = 0;
            decimal MoratoryIAmountApplied = 0;
            decimal AmortizationAmountApplied = 0;
            decimal CapitalAmountApplied = 0;
            List<Mensualidad> listaMesesPago = new List<Mensualidad>();

            #endregion
            #region
            while (AmountToApply > 0)
            {

                /*Order of application:
                 * GPSAmount, Pension, Administrative Expenditure, Insurance, Interest, Moratory Interest, Amortization and Capital
                 */
                //Find mounthly to use
                var Mounthly = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
                listaMesesPago.Add(Mounthly);
                //Si es el crédito es sin pensión
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    /*Pay GPS:
                     * Calcular monto que falta por cubrir: Pago normal de gps - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado del GPS
                     */
                    //Monto deudor de GPS    
                    decimal GPSAmount = Mounthly.GPS - Mounthly.PagoAcumuladoGPS;
                    //Actualización de monto por aplicar
                    AmountToApply -= GPSAmount;
                    //Actualización de monto aplicado para el PAGO
                    GPSAmountApplied += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    //Actualización de monto aplicado para el acumulado de la mensualidad
                    Mounthly.PagoAcumuladoGPS += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                else
                {
                    /*Pay Pensión:
                     * Calcular monto que falta por cubrir: Pago normal de pensión - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado de la pensión
                     */
                    decimal PensionAmount = Mounthly.Pension - Mounthly.PagoAcumuladoPension;
                    AmountToApply -= PensionAmount;
                    PensionAmountApplied += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    Mounthly.PagoAcumuladoPension += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }

                //Pay administrative Expenditure                
                decimal AdministrativeEAmount = Mounthly.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - Mounthly.PagoAcumuladoGastoAdmon;
                AmountToApply -= AdministrativeEAmount;
                AdministrativeEAmountApplied += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                Mounthly.PagoAcumuladoGastoAdmon += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Insurance
                decimal InsuranceAmount = Mounthly.Seguro - Mounthly.PagoAcumuladoSeguro;
                AmountToApply -= InsuranceAmount;
                InsuranceAmountApplied += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                Mounthly.PagoAcumuladoSeguro += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Interest
                decimal InterestAmount = Mounthly.Interes - Mounthly.PagoAcumuladoInteres;
                AmountToApply -= InterestAmount;
                InterestAmountApplied += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                Mounthly.PagoAcumuladoInteres += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                if (AmountToApply <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (Mounthly.Interes == Mounthly.PagoAcumuladoInteres)
                        {
                            Mounthly.Activo = false;
                        }
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();

                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }
                    }

                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }





                var cont_plaso = Credit.Mensualidad.Where(z => z.CapitalDeudor <= 0).Count();


                /*----Pay Amortization----*/
                //Si es crédito con amortización o es la última mensualidad de un crédito sin amortización
                var PlazoTotal = (((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses) + Credit.getPlazoMes) - cont_plaso;
                //if (Credit.TipoDeCredito.Amortiza == true || Mounthly.Mensualidad1 == PlazoTotal)
                //{
                decimal AmortizationAmount = Mounthly.Amortizacion - Mounthly.PagoAcumuladoAmortizacion;
                AmountToApply -= AmortizationAmount;
                AmortizationAmountApplied += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;
                Mounthly.PagoAcumuladoAmortizacion += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;

                //instanciar manejo de la mensualidad
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                //---si se cubre el monto de la amortización
                if (Mounthly.Amortizacion == Mounthly.PagoAcumuladoAmortizacion)
                {
                    //Dejar inactiva la mensualidad
                    Mounthly.Activo = false;
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();

                    if (AmountToApply > 0)
                    {
                        decimal MoratoryIAmount = 0;
                        //Pay Moratory Interest (Condonations by agreement)                
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();
                        if (mora != null)
                        {
                            MoratoryIAmount = mora.MontoAplicado - Mounthly.PagoAcumuladoInteresMoratorio;
                        }
                        AmountToApply -= MoratoryIAmount;
                        //UpdateCondonations(decimal Amount, DateTime Date)
                        decimal MoratoryIAmountAppliedToMounthly = (AmountToApply < 0) ? MoratoryIAmount - Math.Abs(AmountToApply) : MoratoryIAmount;
                        MoratoryIAmountApplied += MoratoryIAmountAppliedToMounthly;
                        Mounthly.PagoAcumuladoInteresMoratorio += MoratoryIAmountAppliedToMounthly;
                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }

                        UpdateCondonations(MoratoryIAmountAppliedToMounthly);
                        if (AmountToApply <= 0)
                        {
                            //actualizar mensualidad
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                        }

                    }




                    if (AmountToApply > 0)
                    {
                        //calcular deuda después de amortización
                        decimal CapitalAmount = Mounthly.CapitalDeudor - Mounthly.Amortizacion;
                        //Si es la última mensualidad o no existen más mensualidades con mora

                        if (Mounthly.Mensualidad1 == PlazoTotal || Mounthly.Mora == 0)
                        {
                            //Pay Capital
                            AmountToApply -= CapitalAmount;
                            CapitalAmountApplied += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            Mounthly.PagoCapitalDeudor += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            CapitalAmount -= Mounthly.PagoCapitalDeudor;
                        }

                        //si es la última mensualidad se reparte el sobrante como gasto administrativo
                        if (CapitalAmount <= 0)
                        {
                            if (AmountToApply > 0)
                            {
                                //agregar a gasto administrativo: crear gasto administrativo
                                var GastoA = new Cargo();
                                GastoA.IdMensualidad = Mounthly.Id;
                                GastoA.Monto = AmountToApply;
                                GastoA.Tipo = 1;
                                db.Cargo.Add(GastoA);
                                db.SaveChanges();

                                AdministrativeEAmount = AmountToApply;
                                AmountToApply = 0;
                                AdministrativeEAmountApplied += AdministrativeEAmount;
                                Mounthly.PagoAcumuladoGastoAdmon += AdministrativeEAmount;
                            }
                            //guardar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            //liquidar    
                            MManagement.LiquidateCredit();
                            break;
                        }
                        else
                        {
                            //actualizar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            if (CapitalAmountApplied > 0)
                                MManagement.ActualizarPorAbonoACApital(Mounthly.Mensualidad1, CapitalAmountApplied);
                            if (AmountToApply <= 0)
                                break;
                        }
                    }
                }
                else
                {
                    //actualizar mensualidad
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
            }
            #endregion
            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPaymentA;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = GPSAmountApplied;
            Payment.Pension = PensionAmountApplied;
            Payment.GastosAdmon = AdministrativeEAmountApplied;
            Payment.Seguro = InsuranceAmountApplied;
            Payment.Interes = InterestAmountApplied;
            Payment.Moratorios = MoratoryIAmountApplied;
            Payment.Capital = AmortizationAmountApplied + CapitalAmountApplied;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            foreach (Mensualidad mounth in listaMesesPago)
            {
                CrucePagoMensualidad PPM = new CrucePagoMensualidad();
                PPM.IdMensualidad = mounth.Id;
                PPM.IdPago = Payment.Id;
                PPM.Monto = 0;
                db.CrucePagoMensualidad.Add(PPM);
                db.SaveChanges();
            }
        }


        //Administración de pagos

        public void EntersPaymentAdjudicacion(decimal Amount, DateTime DateOfAdjudicacion, DateTime DateOfPay)
        {
            var mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true && e.FechaPago.AddDays(10) > DateOfAdjudicacion).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mesAplicable == null)
            {
                mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true).OrderByDescending(e => e.Mensualidad1).FirstOrDefault();
            }
            DateTime DateOfPayment = mesAplicable.FechaPago;
            //fechaTempo.AddDays(-fechaTempo.Day);
            
            var mes = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mes.FechaPago > DateOfPayment && mes.Mensualidad1 > 1)
            {
                var mes_update = Credit.Mensualidad.Where(e => e.Id == (mes.Id - 1)).FirstOrDefault();
                mes_update.Activo = true;
                db.Entry(mes_update).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            /*Variable declaration*/
            decimal AmountToApply = Amount;
            //var MesesPromocion = ((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses);
            //Valor acumulado por concepto aplicado al pago
            decimal AdministrativeEAmountApplied = 0;
            decimal GPSAmountApplied = 0;
            decimal PensionAmountApplied = 0;
            decimal InsuranceAmountApplied = 0;
            decimal InterestAmountApplied = 0;
            decimal MoratoryIAmountApplied = 0;
            decimal AmortizationAmountApplied = 0;
            decimal CapitalAmountApplied = 0;
            List<Mensualidad> listaMesesPago = new List<Mensualidad>();

            //Si se intenta pagar un crédito antes de los primero tres meses se aplica un 20% de penalización
            #region
            /*if (AssessPenalty(DateOfPayment) || AssessAdjudication() || AssessAmount(Amount))
            {
                decimal PenaltyAmount = AssessPenalty();
                LiquidationResult result = LiquidateCredit();
                return result;
            }*/

            #endregion
            #region
            while (AmountToApply > 0)
            {

                /*Order of application:
                 * GPSAmount, Pension, Administrative Expenditure, Insurance, Interest, Moratory Interest, Amortization and Capital
                 */
                //Find mounthly to use
                var Mounthly = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
                listaMesesPago.Add(Mounthly);
                //Si es el crédito es sin pensión
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    /*Pay GPS:
                     * Calcular monto que falta por cubrir: Pago normal de gps - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado del GPS
                     */
                    //Monto deudor de GPS    
                    decimal GPSAmount = Mounthly.GPS - Mounthly.PagoAcumuladoGPS;
                    //Actualización de monto por aplicar
                    AmountToApply -= GPSAmount;
                    //Actualización de monto aplicado para el PAGO
                    GPSAmountApplied += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    //Actualización de monto aplicado para el acumulado de la mensualidad
                    Mounthly.PagoAcumuladoGPS += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                else
                {
                    /*Pay Pensión:
                     * Calcular monto que falta por cubrir: Pago normal de pensión - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado de la pensión
                     */
                    decimal PensionAmount = Mounthly.Pension - Mounthly.PagoAcumuladoPension;
                    AmountToApply -= PensionAmount;
                    PensionAmountApplied += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    Mounthly.PagoAcumuladoPension += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }

                //Pay administrative Expenditure                
                decimal AdministrativeEAmount = Mounthly.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - Mounthly.PagoAcumuladoGastoAdmon;
                AmountToApply -= AdministrativeEAmount;
                AdministrativeEAmountApplied += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                Mounthly.PagoAcumuladoGastoAdmon += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Insurance
                decimal InsuranceAmount = Mounthly.Seguro - Mounthly.PagoAcumuladoSeguro;
                AmountToApply -= InsuranceAmount;
                InsuranceAmountApplied += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                Mounthly.PagoAcumuladoSeguro += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Interest
                decimal InterestAmount = Mounthly.Interes - Mounthly.PagoAcumuladoInteres;
                AmountToApply -= InterestAmount;
                InterestAmountApplied += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                Mounthly.PagoAcumuladoInteres += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                if (AmountToApply <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (Mounthly.Interes == Mounthly.PagoAcumuladoInteres)
                        {
                            Mounthly.Activo = false;
                        }
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();

                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }
                    }

                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }





                var cont_plaso = Credit.Mensualidad.Where(z => z.CapitalDeudor <= 0).Count();


                /*----Pay Amortization----*/
                //Si es crédito con amortización o es la última mensualidad de un crédito sin amortización
                var PlazoTotal = (((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses) + Credit.getPlazoMes) - cont_plaso;
                //if (Credit.TipoDeCredito.Amortiza == true || Mounthly.Mensualidad1 == PlazoTotal)
                //{
                decimal AmortizationAmount = Mounthly.Amortizacion - Mounthly.PagoAcumuladoAmortizacion;
                AmountToApply -= AmortizationAmount;
                AmortizationAmountApplied += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;
                Mounthly.PagoAcumuladoAmortizacion += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;

                //instanciar manejo de la mensualidad
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                //---si se cubre el monto de la amortización
                if (Mounthly.Amortizacion == Mounthly.PagoAcumuladoAmortizacion)
                {
                    //Dejar inactiva la mensualidad
                    Mounthly.Activo = false;
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();


                    if (AmountToApply > 0)
                    {

                        decimal MoratoryIAmount = 0;
                        //Pay Moratory Interest (Condonations by agreement)                
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();
                        if (mora != null)
                        {
                            MoratoryIAmount = mora.MontoAplicado - Mounthly.PagoAcumuladoInteresMoratorio;
                        }
                        AmountToApply -= MoratoryIAmount;
                        //UpdateCondonations(decimal Amount, DateTime Date)
                        decimal MoratoryIAmountAppliedToMounthly = (AmountToApply < 0) ? MoratoryIAmount - Math.Abs(AmountToApply) : MoratoryIAmount;
                        MoratoryIAmountApplied += MoratoryIAmountAppliedToMounthly;
                        Mounthly.PagoAcumuladoInteresMoratorio += MoratoryIAmountAppliedToMounthly;
                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }

                        UpdateCondonations(MoratoryIAmountAppliedToMounthly);
                        if (AmountToApply <= 0)
                        {
                            //actualizar mensualidad
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                        }

                    }




                    if (AmountToApply > 0)
                    {
                        //calcular deuda después de amortización
                        decimal CapitalAmount = Mounthly.CapitalDeudor - Mounthly.Amortizacion;
                        //Si es la última mensualidad o no existen más mensualidades con mora
                        if (Mounthly.Mensualidad1 == PlazoTotal || Mounthly.Mora == 0 || DateOfAdjudicacion == Mounthly.FechaPago)
                        {
                            //Pay Capital
                            AmountToApply -= CapitalAmount;
                            CapitalAmountApplied += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            Mounthly.PagoCapitalDeudor += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            CapitalAmount -= Mounthly.PagoCapitalDeudor;
                        }

                        //si es la última mensualidad se reparte el sobrante como gasto administrativo
                        if (CapitalAmount <= 0)
                        {
                            if (AmountToApply > 0)
                            {
                                //agregar a gasto administrativo: crear gasto administrativo
                                var GastoA = new Cargo();
                                GastoA.IdMensualidad = Mounthly.Id;
                                GastoA.Monto = AmountToApply;
                                GastoA.Tipo = 1;
                                db.Cargo.Add(GastoA);
                                db.SaveChanges();

                                AdministrativeEAmount = AmountToApply;
                                AmountToApply = 0;
                                AdministrativeEAmountApplied += AdministrativeEAmount;
                                Mounthly.PagoAcumuladoGastoAdmon += AdministrativeEAmount;
                            }
                            //guardar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            //liquidar    
                            MManagement.LiquidateCredit();
                            break;
                        }
                        else
                        {
                            //actualizar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            if (CapitalAmountApplied > 0)
                                MManagement.ActualizarPorAbonoACApital(Mounthly.Mensualidad1, CapitalAmountApplied);
                            if (AmountToApply <= 0)
                                break;
                        }
                    }
                }
                else
                {
                    //actualizar mensualidad
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
            }
            #endregion
            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPay;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = GPSAmountApplied;
            Payment.Pension = PensionAmountApplied;
            Payment.GastosAdmon = AdministrativeEAmountApplied;
            Payment.Seguro = InsuranceAmountApplied;
            Payment.Interes = InterestAmountApplied;
            Payment.Moratorios = MoratoryIAmountApplied;
            Payment.Capital = AmortizationAmountApplied + CapitalAmountApplied;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            foreach (Mensualidad mounth in listaMesesPago)
            {
                CrucePagoMensualidad PPM = new CrucePagoMensualidad();
                PPM.IdMensualidad = mounth.Id;
                PPM.IdPago = Payment.Id;
                PPM.Monto = 0;
                db.CrucePagoMensualidad.Add(PPM);
                db.SaveChanges();
            }
        }





        public void EntersPaymentLiquidacionCredito(decimal Amount, DateTime DateOfPaymentA, DateTime DateOfPay)
        {
            var mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true && e.FechaPago.AddDays(10) > DateOfPaymentA).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mesAplicable == null)
            {
                mesAplicable = Credit.Mensualidad.Where(e => e.Activo == true).OrderByDescending(e => e.Mensualidad1).FirstOrDefault();
            }
            DateTime DateOfPayment = mesAplicable.FechaPago;
            //fechaTempo.AddDays(-fechaTempo.Day);

            var mes = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            if (mes.FechaPago > DateOfPayment && mes.Mensualidad1 > 1)
            {
                var mes_update = Credit.Mensualidad.Where(e => e.Id == (mes.Id - 1)).FirstOrDefault();
                mes_update.Activo = true;
                db.Entry(mes_update).State = EntityState.Modified;
                db.SaveChanges();
            }

            /*Variable declaration*/
            decimal AmountToApply = Amount;
            //var MesesPromocion = ((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses);

            //Valor acumulado por concepto aplicado al pago
            decimal AdministrativeEAmountApplied = 0;
            decimal GPSAmountApplied = 0;
            decimal PensionAmountApplied = 0;
            decimal InsuranceAmountApplied = 0;
            decimal InterestAmountApplied = 0;
            decimal MoratoryIAmountApplied = 0;
            decimal AmortizationAmountApplied = 0;
            decimal CapitalAmountApplied = 0;
            List<Mensualidad> listaMesesPago = new List<Mensualidad>();

            //Si se intenta pagar un crédito antes de los primero tres meses se aplica un 20% de penalización
            #region
            /*if (AssessPenalty(DateOfPayment) || AssessAdjudication() || AssessAmount(Amount))
            {
                decimal PenaltyAmount = AssessPenalty();
                LiquidationResult result = LiquidateCredit();
                return result;
            }*/

            #endregion
            #region
            while (AmountToApply > 0)
            {

                /*Order of application:
                 * GPSAmount, Pension, Administrative Expenditure, Insurance, Interest, Moratory Interest, Amortization and Capital
                 */
                //Find mounthly to use
                var Mounthly = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
                listaMesesPago.Add(Mounthly);
                //Si es el crédito es sin pensión
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    /*Pay GPS:
                     * Calcular monto que falta por cubrir: Pago normal de gps - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado del GPS
                     */
                    //Monto deudor de GPS    
                    decimal GPSAmount = Mounthly.GPS - Mounthly.PagoAcumuladoGPS;
                    //Actualización de monto por aplicar
                    AmountToApply -= GPSAmount;
                    //Actualización de monto aplicado para el PAGO
                    GPSAmountApplied += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    //Actualización de monto aplicado para el acumulado de la mensualidad
                    Mounthly.PagoAcumuladoGPS += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                else
                {
                    /*Pay Pensión:
                     * Calcular monto que falta por cubrir: Pago normal de pensión - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado de la pensión
                     */
                    decimal PensionAmount = Mounthly.Pension - Mounthly.PagoAcumuladoPension;
                    AmountToApply -= PensionAmount;
                    PensionAmountApplied += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    Mounthly.PagoAcumuladoPension += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    if (AmountToApply <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }

                //Pay administrative Expenditure                
                decimal AdministrativeEAmount = Mounthly.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - Mounthly.PagoAcumuladoGastoAdmon;
                AmountToApply -= AdministrativeEAmount;
                AdministrativeEAmountApplied += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                Mounthly.PagoAcumuladoGastoAdmon += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Insurance
                decimal InsuranceAmount = Mounthly.Seguro - Mounthly.PagoAcumuladoSeguro;
                AmountToApply -= InsuranceAmount;
                InsuranceAmountApplied += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                Mounthly.PagoAcumuladoSeguro += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pay Interest
                decimal InterestAmount = Mounthly.Interes - Mounthly.PagoAcumuladoInteres;
                AmountToApply -= InterestAmount;
                InterestAmountApplied += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                Mounthly.PagoAcumuladoInteres += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                if (AmountToApply <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (Mounthly.Interes == Mounthly.PagoAcumuladoInteres)
                        {
                            Mounthly.Activo = false;
                        }
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();

                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }
                    }

                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }





                var cont_plaso = Credit.Mensualidad.Where(z => z.CapitalDeudor <= 0).Count();


                /*----Pay Amortization----*/
                //Si es crédito con amortización o es la última mensualidad de un crédito sin amortización
                var PlazoTotal = (((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses) + Credit.getPlazoMes) - cont_plaso;
                //if (Credit.TipoDeCredito.Amortiza == true || Mounthly.Mensualidad1 == PlazoTotal)
                //{
                decimal AmortizationAmount = Mounthly.Amortizacion - Mounthly.PagoAcumuladoAmortizacion;
                AmountToApply -= AmortizationAmount;
                AmortizationAmountApplied += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;
                Mounthly.PagoAcumuladoAmortizacion += (AmountToApply < 0) ? AmortizationAmount - Math.Abs(AmountToApply) : AmortizationAmount;

                //instanciar manejo de la mensualidad
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                //---si se cubre el monto de la amortización
                if (Mounthly.Amortizacion == Mounthly.PagoAcumuladoAmortizacion)
                {
                    //Dejar inactiva la mensualidad
                    Mounthly.Activo = false;
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();


                    if (AmountToApply > 0)
                    {

                        decimal MoratoryIAmount = 0;
                        //Pay Moratory Interest (Condonations by agreement)                
                        var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();
                        if (mora != null)
                        {
                            MoratoryIAmount = mora.MontoAplicado - Mounthly.PagoAcumuladoInteresMoratorio;
                        }
                        AmountToApply -= MoratoryIAmount;
                        //UpdateCondonations(decimal Amount, DateTime Date)
                        decimal MoratoryIAmountAppliedToMounthly = (AmountToApply < 0) ? MoratoryIAmount - Math.Abs(AmountToApply) : MoratoryIAmount;
                        MoratoryIAmountApplied += MoratoryIAmountAppliedToMounthly;
                        Mounthly.PagoAcumuladoInteresMoratorio += MoratoryIAmountAppliedToMounthly;
                        if (Mounthly.Mora > 0)
                        {
                            if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                            {
                                Mounthly.Activo = true;
                            }
                        }

                        UpdateCondonations(MoratoryIAmountAppliedToMounthly);
                        if (AmountToApply <= 0)
                        {
                            //actualizar mensualidad
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                        }

                    }




                    if (AmountToApply > 0)
                    {
                        //calcular deuda después de amortización
                        decimal CapitalAmount = Mounthly.CapitalDeudor - Mounthly.Amortizacion;
                        //Si es la última mensualidad o no existen más mensualidades con mora
                        if (Mounthly.Mensualidad1 == PlazoTotal || Mounthly.Mora == 0 || DateOfPaymentA == Mounthly.FechaPago)
                        {
                            //Pay Capital
                            AmountToApply -= CapitalAmount;
                            CapitalAmountApplied += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            Mounthly.PagoCapitalDeudor += (AmountToApply < 0) ? CapitalAmount - Math.Abs(AmountToApply) : CapitalAmount;
                            CapitalAmount -= Mounthly.PagoCapitalDeudor;
                        }

                        //si es la última mensualidad se reparte el sobrante como gasto administrativo
                        if (CapitalAmount <= 0)
                        {
                            if (AmountToApply > 0)
                            {
                                //agregar a gasto administrativo: crear gasto administrativo
                                var GastoA = new Cargo();
                                GastoA.IdMensualidad = Mounthly.Id;
                                GastoA.Monto = AmountToApply;
                                GastoA.Tipo = 1;
                                db.Cargo.Add(GastoA);
                                db.SaveChanges();

                                AdministrativeEAmount = AmountToApply;
                                AmountToApply = 0;
                                AdministrativeEAmountApplied += AdministrativeEAmount;
                                Mounthly.PagoAcumuladoGastoAdmon += AdministrativeEAmount;
                            }
                            //guardar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            //liquidar    
                            MManagement.LiquidateCredit();
                            break;
                        }
                        else
                        {
                            //actualizar mensualidad
                            Mounthly = NormalizarMensualidad(Mounthly);
                            db.Entry(Mounthly).State = EntityState.Modified;
                            db.SaveChanges();
                            if (CapitalAmountApplied > 0)
                                MManagement.ActualizarPorAbonoACApital(Mounthly.Mensualidad1, CapitalAmountApplied);
                            if (AmountToApply <= 0)
                                break;
                        }
                    }
                }
                else
                {
                    //actualizar mensualidad
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
            }
            #endregion
            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPay;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = GPSAmountApplied;
            Payment.Pension = PensionAmountApplied;
            Payment.GastosAdmon = AdministrativeEAmountApplied;
            Payment.Seguro = InsuranceAmountApplied;
            Payment.Interes = InterestAmountApplied;
            Payment.Moratorios = MoratoryIAmountApplied;
            Payment.Capital = AmortizationAmountApplied + CapitalAmountApplied;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            foreach (Mensualidad mounth in listaMesesPago)
            {
                CrucePagoMensualidad PPM = new CrucePagoMensualidad();
                PPM.IdMensualidad = mounth.Id;
                PPM.IdPago = Payment.Id;
                PPM.Monto = 0;
                db.CrucePagoMensualidad.Add(PPM);
                db.SaveChanges();
            }
        }





        public void OverdueReceivables(DateTime DateOfPayment)
        {

            //Find Overdue Receivables list
            var OverdueReceivables = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1);

            foreach (Mensualidad Mounthly in OverdueReceivables)
            {
                //DateTime Today = DateTime.Today;
                //si la fecha de pago es menor al día actual entra

                int DiasTolerancia = CalculoDiasHabiles(Mounthly.FechaPago);


                if (Mounthly.FechaPago.AddDays(DiasTolerancia) < DateOfPayment)
                {
                    long meses = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Month, Mounthly.FechaPago.AddDays(DiasTolerancia), DateOfPayment);
                    long dias = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Day, Mounthly.FechaPago.AddDays(DiasTolerancia).AddMonths((int)meses), DateOfPayment);
                    // Difference in mounths
                    long DiferenciaMeses = 0;
                    DiferenciaMeses += meses;
                    DiferenciaMeses += (dias > 0) ? 1 : 0;
                    Mounthly.Mora = (int)DiferenciaMeses;

                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    addMoratorios(Mounthly);
                }
            }
        }


        public void RevertOverdueReceivables(DateTime DateOfPayment)
        {
            //Find Overdue Receivables list
            var OverdueReceivables = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1);

            foreach (Mensualidad Mounthly in OverdueReceivables)
            {
                int DiasTolerancia = CalculoDiasHabiles(Mounthly.FechaPago);
                //DateTime Today = DateTime.Today;
                //si la fecha de pago es menor al día actual entra
                if (Mounthly.FechaPago.AddDays(DiasTolerancia) < DateOfPayment)
                {
                    long meses = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Month, Mounthly.FechaPago.AddDays(DiasTolerancia), DateOfPayment);
                    long dias = Microsoft.VisualBasic.DateAndTime.DateDiff(Microsoft.VisualBasic.DateInterval.Day, Mounthly.FechaPago.AddDays(DiasTolerancia).AddMonths((int)meses), DateOfPayment);
                    // Difference in mounths
                    long DiferenciaMeses = 0;
                    DiferenciaMeses += meses;
                    DiferenciaMeses += (dias > 0) ? 1 : 0;
                    Mounthly.Mora = (int)DiferenciaMeses;
                    //actualizar mensualidad

                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    addMoratorios(Mounthly);
                }
                else
                {
                    Mounthly.Mora = 0;
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    addMoratorios(Mounthly);
                }
            }
        }

        public int CalculoDiasHabiles(DateTime fecha)
        {


            DateTime fechaResp = fecha;
            int diasNoHabiles = 0;
            int diasToleranciaMora = SettingsHelper.getDiasToleranciaMora();
            int contPago = SettingsHelper.getDiasToleranciaMora();
            while (contPago > 0)
            {
                fecha = fecha.AddDays(1);
                if (fecha.DayOfWeek == DayOfWeek.Saturday ||
                    fecha.DayOfWeek == DayOfWeek.Sunday ||
                    IsHoliday(fecha))
                {
                    diasNoHabiles++;
                }
                else
                {
                    contPago--;
                }
            }
            return diasToleranciaMora + diasNoHabiles;
        }

        public bool IsHoliday(DateTime originalDate)
        { // INSERT YOUR HOlIDAY-CODE HERE! 
            int dia = db.dias_no_laborales.Where(f => f.fecha == originalDate).Count();
            return dia > 0;

        }

        public void addMoratorios(Mensualidad Mounthly)
        {
            var mensualidad = Mounthly;
            Mora mora = db.Mora.Where(a => a.IdMensualidad == Mounthly.Id).FirstOrDefault();
            if (mora == null)
            {
                if (Mounthly.Mora > 0)
                {
                    mora = new Mora();
                    mora.IdMensualidad = Mounthly.Id;
                    mora.Monto = Mounthly.CalculateMoratoryInterest;
                    mora.MontoAplicado = 0;
                    mora.Activo = true;
                    mora.Contabilizado = false;
                    db.Mora.Add(mora);
                    db.SaveChanges();
                }
            }
            else
            {
                if (Mounthly.Mora == 0)
                {
                    db.Mora.Remove(mora);
                    db.SaveChanges();
                }
                else
                {
                    mora.IdMensualidad = Mounthly.Id;
                    mora.Monto = Mounthly.CalculateMoratoryInterest;
                    db.Entry(mora).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }


        //Se utiliza para reconstruir el pago al eliminar o actualizar pagos
        public void AdjustMonthlyPaymentsByUnpostedPayments()
        {
            //Seleccionar mensualidades que tiene relación con pagos no contabilizados
            //ordenada por mensualidad1
            var Pagos = Credit.AplicacionPago.Where(e => e.Contabilizado == false);
            var PagosContabilizados = Credit.AplicacionPago.Where(e => e.Contabilizado == true);
            var mensualidades = Credit.Mensualidad.Where(e => e.Contabilizado == false);
            //obtener el monto total de la suma de los pagos
            decimal MontoPagos = Pagos.Sum(e => e.Monto);
            decimal MontoPagosContabilizados = PagosContabilizados.Sum(e => e.Monto);
            //obtener el monto de la suma de los acumulados de las últimas mensualidades
            decimal MontoMensualidades = mensualidades.Select(c => new
            {
                TotalMensualidad = c.PagoAcumuladoGPS + c.PagoAcumuladoPension + c.PagoAcumuladoGastoAdmon + c.PagoAcumuladoSeguro + c.PagoAcumuladoInteres + c.PagoAcumuladoInteresMoratorio + c.PagoAcumuladoAmortizacion + c.PagoCapitalDeudor
            }).Sum(e => e.TotalMensualidad);

            //monto contabilizado
            decimal MontoC = MontoMensualidades - MontoPagos;

            if (MontoPagosContabilizados > 0)
            {
                if (MontoPagosContabilizados > 0)
                {
                    RegenerarPago(MontoPagosContabilizados);
                }
                var Mounthly = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                MManagement.ActualizarCapitalPorEliminacion(Mounthly.Mensualidad1 - 1, 0);
            }
        }



        public void RegenerarPago(decimal Amount)
        {
            var mensualidades = Credit.Mensualidad.Where(a => (a.PagoAcumuladoGPS > 0 || a.PagoAcumuladoPension > 0 || a.PagoAcumuladoGastoAdmon > 0));
            foreach (Mensualidad mensualidad in mensualidades.ToList())
            {
                var Mounthly = mensualidad;
                bool editado = false;
                if (Amount > mensualidad.PagoAcumuladoGPS)
                {
                    Amount -= mensualidad.PagoAcumuladoGPS;
                }
                else
                {
                    Mounthly.PagoAcumuladoGPS = Amount;
                    Amount = 0; editado = true;
                }
                if (Amount > mensualidad.PagoAcumuladoPension)
                {
                    Amount -= mensualidad.PagoAcumuladoPension;
                }
                else
                {
                    Mounthly.PagoAcumuladoPension = Amount;
                    Amount = 0; editado = true;
                }


                if (Amount > mensualidad.PagoAcumuladoGastoAdmon)
                {
                    Amount -= mensualidad.PagoAcumuladoGastoAdmon;
                }
                else
                {
                    Mounthly.PagoAcumuladoGastoAdmon = Amount;
                    Amount = 0; editado = true;
                }


                if (Amount > mensualidad.PagoAcumuladoSeguro)
                {
                    Amount -= mensualidad.PagoAcumuladoSeguro;
                }
                else
                {
                    Mounthly.PagoAcumuladoSeguro = Amount;
                    Amount = 0; editado = true;
                }
                if (Amount > mensualidad.PagoAcumuladoInteres)
                {
                    Amount -= mensualidad.PagoAcumuladoInteres;
                }
                else
                {
                    Mounthly.PagoAcumuladoInteres = Amount;
                    Amount = 0; editado = true;
                }
                if (Amount > mensualidad.PagoAcumuladoInteresMoratorio)
                {
                    Amount -= mensualidad.PagoAcumuladoInteresMoratorio;
                }
                else
                {
                    Mounthly.PagoAcumuladoInteresMoratorio = Amount;

                    var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();
                    if (mora == null)
                    {
                        Mounthly.Activo = true;
                    }
                    else
                    {
                        if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                        {
                            Mounthly.Activo = true;
                        }
                    }
                    Amount = 0; editado = true;
                }
                if (Amount > mensualidad.PagoAcumuladoAmortizacion)
                {
                    Amount -= mensualidad.PagoAcumuladoAmortizacion;
                }
                else
                {
                    Mounthly.PagoAcumuladoAmortizacion = Amount;

                    if (Mounthly.PagoAcumuladoAmortizacion < Mounthly.Amortizacion)
                    {
                        Mounthly.Activo = true;
                    }

                    Amount = 0; editado = true;
                }
                if (Amount > mensualidad.PagoCapitalDeudor)
                {
                    Amount -= mensualidad.PagoCapitalDeudor;
                }
                else
                {
                    Mounthly.PagoCapitalDeudor = Amount;
                    Amount = 0; editado = true;
                }
                if (editado == true)
                {
                    if (Mounthly.PagoAcumuladoAmortizacion < Mounthly.Amortizacion)
                    {
                        Mounthly.Activo = true;
                    }
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public decimal UpdateCondonations(decimal Amount)
        {
            return 0;
        }
        public decimal CalculateCondonation()
        {
            return 0;
        }
        public void ApplyPayment()
        {
        }
        public void DeleteCondonation()
        {
        }
        public Mensualidad NormalizarMensualidad(Mensualidad Mounthly)
        {
            Mounthly.GPS = Math.Round(Mounthly.GPS, 2);
            Mounthly.CapitalDeudor = Math.Round(Mounthly.CapitalDeudor, 2);
            Mounthly.Pension = Math.Round(Mounthly.Pension, 2);
            Mounthly.Seguro = Math.Round(Mounthly.Seguro, 2);
            Mounthly.Interes = Math.Round(Mounthly.Interes, 2);
            Mounthly.Amortizacion = Math.Round(Mounthly.Amortizacion, 2);
            return Mounthly;
        }
        
        public void EntersPaymentAdmon(decimal Amount, DateTime DateOfPaymentA, int idMensualidad, string concepto, int cuenta)
        {

            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPaymentA;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = 0;
            Payment.Pension = 0;
            Payment.GastosAdmon = Amount;
            Payment.Seguro = 0;
            Payment.Interes = 0;
            Payment.Moratorios = 0;
            Payment.Capital = 0;
            Payment.GastosCobrenza = 0;
            Payment.Concepto = concepto;
            Payment.IdTipoPago = (int)Enums.TipoDePago.PagoGastosAdmon;
            Payment.IdCuentasPagos = cuenta;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            RespaldarAmortizacionPrePago(Payment.Id);


            Mensualidad mmount = db.Mensualidad.Find(idMensualidad);
            mmount.PagoAcumuladoGastoAdmon = mmount.PagoAcumuladoGastoAdmon + Amount;
            db.Entry(mmount).State = EntityState.Modified;
            db.SaveChanges();

            Cargo cargo = new Cargo();
            cargo.Descripcion = concepto;
            cargo.IdMensualidad = idMensualidad;
            cargo.Monto = Amount;
            cargo.idAplicacionPago = Payment.Id;
            cargo.Tipo = (int)Enums.CatTipoCargo.PagoGastosAdmon;
            db.Cargo.Add(cargo);
            db.SaveChanges();
            
            DetalleDePago detallePago = new DetalleDePago();
            detallePago.IdPago = Payment.Id;
            detallePago.IdMensualidad = idMensualidad;
            detallePago.IdTipoDetallePago = (int)Enums.TipoDetallePago.GastosAdministrativos;
            detallePago.Monto = Amount;
            detallePago.Concepto = concepto;
            db.DetalleDePago.Add(detallePago);
            db.SaveChanges();

        }

        #region PAGO A CAPITAL





        public void EntersPaymentCapital(decimal Amount, DateTime DateOfPaymentA, int idMensualidad, string concepto, int cuenta)
        {

            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPaymentA;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = 0;
            Payment.Pension = 0;
            Payment.GastosAdmon = 0;
            Payment.GastosCobrenza = 0;
            Payment.Seguro = 0;
            Payment.Interes = 0;
            Payment.Moratorios = 0;
            Payment.Capital = Amount;
            Payment.IdCuentasPagos = cuenta;
            Payment.Concepto = concepto;
            Payment.Contabilizado = false;
            Payment.IdTipoPago = (int)Enums.TipoDePago.PagoACapital;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            RespaldarAmortizacionPrePago(Payment.Id);

            var Mounthly = Credit.Mensualidad.Where(e => e.Id == idMensualidad).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
            Mounthly.PagoCapitalDeudor += Amount;
            Mounthly = NormalizarMensualidad(Mounthly);
            db.Entry(Mounthly).State = EntityState.Modified;
            db.SaveChanges();

            MManagement.ActualizarPorAbonoACApital(Mounthly.Mensualidad1, Amount);
            Mounthly = NormalizarMensualidad(Mounthly);
            db.Entry(Mounthly).State = EntityState.Modified;
            db.SaveChanges();
            
            DetalleDePago detallePago = new DetalleDePago();
            detallePago.IdPago = Payment.Id;
            detallePago.IdMensualidad = idMensualidad;
            detallePago.IdTipoDetallePago = (int)Enums.TipoDetallePago.Capital;
            detallePago.Monto = Amount;
            detallePago.Concepto = concepto;
            db.DetalleDePago.Add(detallePago);
            db.SaveChanges();

        }
        
        #endregion

        #region Liquidacion por pago a Capital








        public int EntersPaymentIntereses(decimal Amount, DateTime DateOfPaymentA, DateTime DateOfPaymentR)
        {

            /*Variable declaration*/
            decimal AmountToApply = Amount;
            //var MesesPromocion = ((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses);

            //Valor acumulado por concepto aplicado al pago
            decimal AdministrativeEAmountApplied = 0;
            decimal GPSAmountApplied = 0;
            decimal PensionAmountApplied = 0;
            decimal InsuranceAmountApplied = 0;
            decimal InterestAmountApplied = 0;
            decimal MoratoryIAmountApplied = 0;
            decimal AmortizationAmountApplied = 0;
            decimal CapitalAmountApplied = 0;

            List<Mensualidad> listaMesesPago = new List<Mensualidad>();

            while (AmountToApply > 0)
            {
                var Mounthly = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();
                listaMesesPago.Add(Mounthly);
                //Si es el crédito es sin pensión
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    //Monto deudor de GPS    
                    decimal GPSAmount = Mounthly.GPS - Mounthly.PagoAcumuladoGPS;
                    AmountToApply -= GPSAmount;
                    GPSAmountApplied += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    Mounthly.PagoAcumuladoGPS += (AmountToApply < 0) ? GPSAmount - Math.Abs(AmountToApply) : GPSAmount;
                    if (AmountToApply <= 0)
                    {
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                else
                {
                    /*Pay Pensión:*/
                    decimal PensionAmount = Mounthly.Pension - Mounthly.PagoAcumuladoPension;
                    AmountToApply -= PensionAmount;
                    PensionAmountApplied += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    Mounthly.PagoAcumuladoPension += (AmountToApply < 0) ? PensionAmount - Math.Abs(AmountToApply) : PensionAmount;
                    if (AmountToApply <= 0)
                    {
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }

                //Pay administrative Expenditure                
                decimal AdministrativeEAmount = Mounthly.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - Mounthly.PagoAcumuladoGastoAdmon;
                AmountToApply -= AdministrativeEAmount;
                AdministrativeEAmountApplied += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                Mounthly.PagoAcumuladoGastoAdmon += (AmountToApply < 0) ? AdministrativeEAmount - Math.Abs(AdministrativeEAmount) : AdministrativeEAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pago de seguro
                decimal InsuranceAmount = Mounthly.Seguro - Mounthly.PagoAcumuladoSeguro;
                AmountToApply -= InsuranceAmount;
                InsuranceAmountApplied += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                Mounthly.PagoAcumuladoSeguro += (AmountToApply < 0) ? InsuranceAmount - Math.Abs(AmountToApply) : InsuranceAmount;
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }

                //Pago de Intereses
                decimal InterestAmount = Mounthly.Interes - Mounthly.PagoAcumuladoInteres;
                AmountToApply -= InterestAmount;
                InterestAmountApplied += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                Mounthly.PagoAcumuladoInteres += (AmountToApply < 0) ? InterestAmount - Math.Abs(AmountToApply) : InterestAmount;
                if (AmountToApply <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (Mounthly.Interes == Mounthly.PagoAcumuladoInteres)
                        {
                            Mounthly.Activo = false;
                        }
                    }
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }


                decimal MoratoryIAmount = 0;
                //Pay Moratory Interest (Condonations by agreement)                
                var mora = Mounthly.Mora1.Where(m => m.IdMensualidad == Mounthly.Id).FirstOrDefault();
                if (mora != null)
                {
                    MoratoryIAmount = mora.MontoAplicado - Mounthly.PagoAcumuladoInteresMoratorio;
                }
                AmountToApply -= MoratoryIAmount;
                //UpdateCondonations(decimal Amount, DateTime Date)
                decimal MoratoryIAmountAppliedToMounthly = (AmountToApply < 0) ? MoratoryIAmount - Math.Abs(AmountToApply) : MoratoryIAmount;
                MoratoryIAmountApplied += MoratoryIAmountAppliedToMounthly;
                Mounthly.PagoAcumuladoInteresMoratorio += MoratoryIAmountAppliedToMounthly;
                if (Mounthly.Mora > 0)
                {
                    if (Mounthly.PagoAcumuladoInteresMoratorio < mora.MontoAplicado)
                    {
                        Mounthly.Activo = true;
                        //actualizar mensualidad
                        db.Entry(Mounthly).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                UpdateCondonations(MoratoryIAmountAppliedToMounthly);
                if (AmountToApply <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }


                //si es la última mensualidad se reparte el sobrante como gasto administrativo
                if (AmountToApply > 0 && Mounthly.FechaPago == DateOfPaymentA)
                {
                    //agregar a gasto administrativo: crear gasto administrativo
                    var GastoA = new Cargo();
                    GastoA.IdMensualidad = Mounthly.Id;
                    GastoA.Monto = AmountToApply;
                    GastoA.Tipo = 1;
                    db.Cargo.Add(GastoA);
                    db.SaveChanges();

                    AdministrativeEAmount = AmountToApply;
                    AmountToApply = 0;
                    AdministrativeEAmountApplied += AdministrativeEAmount;
                    Mounthly.PagoAcumuladoGastoAdmon += AdministrativeEAmount;


                    //guardar mensualidad
                    Mounthly = NormalizarMensualidad(Mounthly);
                    db.Entry(Mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    //liquidar    
                    break;



                }












            }

            AplicacionPago Payment = new AplicacionPago();
            Payment.IdCredito = Credit.id;
            Payment.FechaPago = DateOfPaymentR;
            Payment.FechaCaptura = DateTime.Today;
            Payment.Monto = Amount;
            Payment.GPS = GPSAmountApplied;
            Payment.Pension = PensionAmountApplied;
            Payment.GastosAdmon = AdministrativeEAmountApplied;
            Payment.Seguro = InsuranceAmountApplied;
            Payment.Interes = InterestAmountApplied;
            Payment.Moratorios = MoratoryIAmountApplied;
            Payment.Capital = AmortizationAmountApplied + CapitalAmountApplied;
            db.AplicacionPago.Add(Payment);
            db.SaveChanges();
            foreach (Mensualidad mounth in listaMesesPago)
            {
                CrucePagoMensualidad PPM = new CrucePagoMensualidad();
                PPM.IdMensualidad = mounth.Id;
                PPM.IdPago = Payment.Id;
                PPM.Monto = 0;
                db.CrucePagoMensualidad.Add(PPM);
                db.SaveChanges();
            }
            return Payment.Id;


        }



        public void EntersPaymentCapitalLiquidacion(decimal Amount, DateTime DateOfPaymentA, int idMensualidad, int IdPago)
        {
            var Mounthly = Credit.Mensualidad.Where(e => e.Id == idMensualidad).OrderBy(e => e.Mensualidad1).FirstOrDefault();
            MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
            Mounthly.PagoCapitalDeudor += Amount;
            //actualizar mensualidad
            Mounthly = NormalizarMensualidad(Mounthly);
            db.Entry(Mounthly).State = EntityState.Modified;
            db.SaveChanges();

            MManagement.ActualizarPorAbonoACApital(Mounthly.Mensualidad1, Amount);
            //actualizar mensualidad
            Mounthly = NormalizarMensualidad(Mounthly);
            db.Entry(Mounthly).State = EntityState.Modified;
            db.SaveChanges();

            AplicacionPago Payment = new AplicacionPago();

            Payment = db.AplicacionPago.Find(IdPago);

            Payment.Monto = Payment.Monto + Amount;
            Payment.Contabilizado = true;

            db.Entry(Payment).State = EntityState.Modified;
            db.SaveChanges();

            CrucePagoMensualidad PPM = new CrucePagoMensualidad();
            PPM.IdMensualidad = Mounthly.Id;
            PPM.IdPago = Payment.Id;
            PPM.Monto = Amount;
            db.CrucePagoMensualidad.Add(PPM);
            db.SaveChanges();

        }











        #endregion
        
        public void EntersPaymentMoratorios(decimal MONTOPAGOINGRESADO, DateTime fechaDePago, List<PagoMora> pagosMoras, String Concepto, int cuenta)
        {
            #region Crear pago
            //Respaldar Amortización antes del Pago

            AplicacionPago AplicacionDelPago = new AplicacionPago();
            AplicacionDelPago.IdCredito = Credit.id;
            AplicacionDelPago.FechaPago = fechaDePago;
            AplicacionDelPago.FechaCaptura = DateTime.Today;
            AplicacionDelPago.IdTipoPago = (int)Enums.TipoDePago.PagoNormal;
            AplicacionDelPago.Monto = MONTOPAGOINGRESADO;
            AplicacionDelPago.Concepto = Concepto;
            AplicacionDelPago.IdCuentasPagos = cuenta;
            db.AplicacionPago.Add(AplicacionDelPago);
            db.SaveChanges();
            RespaldarAmortizacionPrePago(AplicacionDelPago.Id);

            //Aplicar Moras en caso de existir
            //Pagar Moratorios
            decimal montoMora = 0;
            if (pagosMoras.Count > 0) {
                montoMora = PagarMoratorios(MONTOPAGOINGRESADO, pagosMoras, AplicacionDelPago.Id);

            }
            AplicacionDelPago.Moratorios = montoMora;
            MONTOPAGOINGRESADO -= montoMora;

            #endregion

            #region Activacion de mensualidad en caso de adelanto a capital
            //Se activa la mensualidad en caso de que el pago pertenesca a adelanto de capital.
            var mes = Credit.Mensualidad.Where(e => e.Activo == false).OrderByDescending(e => e.Mensualidad1).FirstOrDefault();
            if (mes != null)
            {
                if (mes.FechaPago.AddDays(10) >= fechaDePago)
                {
                    mes.Activo = true;
                    db.Entry(mes).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            #endregion

            #region Declaracion de variables

            decimal MontoRemanenteDelPago = MONTOPAGOINGRESADO;
            //Valor acumulado por concepto aplicado al pago
            decimal MONTO_ADMINISTRATIVO_APLICADO = 0;
            decimal MONTO_COBRANZA_APLICADO = 0;
            decimal MONTO_GPS_APLICADO = 0;
            decimal MONTO_PENSION_APLICADO = 0;
            decimal MONTO_SEGURO_APLICADO = 0;
            decimal MONTO_INTERES_APLICADO = 0;
            decimal MONTO_AMORTIZACION_APLICADO = 0;
            decimal MONTO_CAPITAL_APLICADO = 0;

            #endregion
            
            while (MontoRemanenteDelPago > 0)
            {

                /*Order of application:
                 * GPSAmount, Pension, Administrative Expenditure, Insurance, Interest, Moratory Interest, Amortization and Capital
                 */
                //Se busca la mensualidad activa siguiente
                var MensualidadEnUso = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();

                //Si es el crédito es sin pensión
                #region Aplicación de GPS
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    /*Pago GPS:
                     * Calcular monto que falta por cubrir: Pago normal de gps - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado del GPS
                     */
                    //Monto deudor de GPS    

                    decimal GPS_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.GPS).Sum(s=> s.MontoAplicado);

                    decimal FaltanteGPS = MensualidadEnUso.GPS - MensualidadEnUso.PagoAcumuladoGPS - GPS_Condonado;
                    //Actualización de monto por aplicar
                    MontoRemanenteDelPago -= FaltanteGPS;
                    //Actualización de monto aplicado para el PAGO
                    decimal MontoNetoGps = (MontoRemanenteDelPago < 0) ? FaltanteGPS - Math.Abs(MontoRemanenteDelPago) : FaltanteGPS;
                    MONTO_GPS_APLICADO += MontoNetoGps;
                    //Actualización de monto aplicado para el acumulado de la mensualidad
                    MensualidadEnUso.PagoAcumuladoGPS += MontoNetoGps;
                    //Se agrega el detalle del Pago
                    if (MontoNetoGps > 0) {
                        AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GPS, MontoNetoGps, "GPS " + MensualidadEnUso.FechaPago.ToShortDateString());
                    }                   

                    if (MontoRemanenteDelPago <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(MensualidadEnUso).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                #endregion

                #region Aplicacion de la pensión
                else
                {

                    /*Pay Pensión:
                     * Calcular monto que falta por cubrir: Pago normal de pensión - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado de la pensión
                     */

                    decimal Pension_Condonada = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Pensión).Sum(s => s.MontoAplicado);

                    decimal FaltantePension = MensualidadEnUso.Pension - MensualidadEnUso.PagoAcumuladoPension - Pension_Condonada;
                    MontoRemanenteDelPago -= FaltantePension;
                    decimal MontoNetoPension = (MontoRemanenteDelPago < 0) ? FaltantePension - Math.Abs(MontoRemanenteDelPago) : FaltantePension;
                    MONTO_PENSION_APLICADO += MontoNetoPension;
                    //Add detalle de pago pension
                    if (MontoNetoPension > 0) {
                        AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Pension, MontoNetoPension, "Pensión " + MensualidadEnUso.FechaPago.ToShortDateString());
                    }
                    

                    MensualidadEnUso.PagoAcumuladoPension += (MontoRemanenteDelPago < 0) ? FaltantePension - Math.Abs(MontoRemanenteDelPago) : FaltantePension;
                    if (MontoRemanenteDelPago <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(MensualidadEnUso).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                #endregion

                #region Aplicación de gastos Administrativos
                decimal FaltanteGastosADMON = MensualidadEnUso.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - MensualidadEnUso.PagoAcumuladoGastoAdmon;
                MontoRemanenteDelPago -= FaltanteGastosADMON;
                decimal MontoNetoGastosADMON = (MontoRemanenteDelPago < 0) ? FaltanteGastosADMON - Math.Abs(MontoRemanenteDelPago) : FaltanteGastosADMON;
                MONTO_ADMINISTRATIVO_APLICADO += MontoNetoGastosADMON;

                if (MontoNetoGastosADMON > 0) {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosAdministrativos, MontoNetoGastosADMON, "Gastos Administrativos " + MensualidadEnUso.FechaPago.ToShortDateString());
                }
                

                MensualidadEnUso.PagoAcumuladoGastoAdmon += MontoNetoGastosADMON;
                if (MontoRemanenteDelPago <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicación de gastos Cobranza
                decimal FaltanteGastosCobranza = MensualidadEnUso.Cargo.Where(e => e.TipoCargo.Id == (int)Enums.CatTipoCargo.GastosCobranza).Sum(e => e.Monto) - MensualidadEnUso.PagoAcumuladoGastosCobrenza;
                MontoRemanenteDelPago -= FaltanteGastosCobranza;
                decimal MontoNetoGastosCobranza = (MontoRemanenteDelPago < 0) ? FaltanteGastosCobranza - Math.Abs(MontoRemanenteDelPago) : FaltanteGastosCobranza;
                MONTO_COBRANZA_APLICADO += MontoNetoGastosCobranza;

                if (MontoNetoGastosCobranza > 0) {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosCobrenza, MontoNetoGastosCobranza, "Gastos de cobranza " + MensualidadEnUso.FechaPago.ToShortDateString());
                }
                

                MensualidadEnUso.PagoAcumuladoGastosCobrenza += MontoNetoGastosCobranza;
                if (MontoRemanenteDelPago <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicacion a pago de seguro
                //PAgo a Seguro

                decimal Seguro_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Seguro).Sum(s => s.MontoAplicado);

                decimal FaltantePagoASeguro = MensualidadEnUso.Seguro - MensualidadEnUso.PagoAcumuladoSeguro - Seguro_Condonado;
                MontoRemanenteDelPago -= FaltantePagoASeguro;
                decimal MontoNetoSeguro = (MontoRemanenteDelPago < 0) ? FaltantePagoASeguro - Math.Abs(MontoRemanenteDelPago) : FaltantePagoASeguro;
                MONTO_SEGURO_APLICADO += MontoNetoSeguro;
                if (MontoNetoSeguro > 0) {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Seguro, MontoNetoSeguro, "Seguro " + MensualidadEnUso.FechaPago.ToShortDateString());
                }               

                MensualidadEnUso.PagoAcumuladoSeguro += MontoNetoSeguro;
                if (MontoRemanenteDelPago <= 0)
                {
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicación de Pago a intereses
                //Pay Interest

                decimal Interes_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Interes).Sum(s => s.MontoAplicado);

                decimal FaltantePagoAInteres = MensualidadEnUso.Interes - MensualidadEnUso.PagoAcumuladoInteres- Interes_Condonado;
                MontoRemanenteDelPago -= FaltantePagoAInteres;
                decimal MontoNetoInteres = (MontoRemanenteDelPago < 0) ? FaltantePagoAInteres - Math.Abs(MontoRemanenteDelPago) : FaltantePagoAInteres;
                MONTO_INTERES_APLICADO += MontoNetoInteres;

                if (MontoNetoInteres > 0) {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Interes, MontoNetoInteres, "Interes " + MensualidadEnUso.FechaPago.ToShortDateString());
                }                

                MensualidadEnUso.PagoAcumuladoInteres += MontoNetoInteres;
                if (MontoRemanenteDelPago <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (MensualidadEnUso.Interes == MensualidadEnUso.PagoAcumuladoInteres)
                        {
                            MensualidadEnUso.Activo = false;
                        }
                    }
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                var cont_plaso = Credit.Mensualidad.Where(z => z.CapitalDeudor <= 0).Count();
                /*----Pay Amortization----*/
                //Si es crédito con amortización o es la última mensualidad de un crédito sin amortización
                var PlazoTotal = (((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses) + Credit.getPlazoMes) - cont_plaso;
                
                #region Aplicación de pago a Amortización
                decimal FaltantePagoAmortizacion = MensualidadEnUso.Amortizacion - MensualidadEnUso.PagoAcumuladoAmortizacion;
                MontoRemanenteDelPago -= FaltantePagoAmortizacion;
                decimal montoNetoAmortizacion = (MontoRemanenteDelPago < 0) ? FaltantePagoAmortizacion - Math.Abs(MontoRemanenteDelPago) : FaltantePagoAmortizacion;
                MONTO_AMORTIZACION_APLICADO += montoNetoAmortizacion;
                if (montoNetoAmortizacion > 0) {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Amortización, montoNetoAmortizacion, "Amortización " + MensualidadEnUso.FechaPago.ToShortDateString());
                }               

                MensualidadEnUso.PagoAcumuladoAmortizacion += montoNetoAmortizacion;



                //instanciar manejo de la mensualidad
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                //---si se cubre el monto de la amortización
                if (MensualidadEnUso.Amortizacion == MensualidadEnUso.PagoAcumuladoAmortizacion)
                {
                    //Dejar inactiva la mensualidad
                    MensualidadEnUso.Activo = false;
                    MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();

                    if (MontoRemanenteDelPago > 0)
                    {
                        //calcular deuda después de amortización
                        decimal CapitalAmount = MensualidadEnUso.CapitalDeudor - MensualidadEnUso.Amortizacion - MensualidadEnUso.PagoCapitalDeudor;

                        //Si es la última mensualidad o no existen más mensualidades con mora
                        if (MensualidadEnUso.Mensualidad1 == PlazoTotal || fechaDePago <= MensualidadEnUso.FechaPago.AddDays(10))
                        {
                            //Pay Capital
                            MontoRemanenteDelPago -= CapitalAmount;

                            decimal montoCapitalneto = (MontoRemanenteDelPago < 0) ? CapitalAmount - Math.Abs(MontoRemanenteDelPago) : CapitalAmount;
                            MONTO_CAPITAL_APLICADO += montoCapitalneto;
                            MensualidadEnUso.PagoCapitalDeudor += montoCapitalneto;
                            if (montoCapitalneto > 0) {
                                AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Capital, montoCapitalneto, "Pago a Capital " + MensualidadEnUso.FechaPago.ToShortDateString());
                            }                            
                            CapitalAmount -= montoCapitalneto;
                        }

                        //si es la última mensualidad se reparte el sobrante como gasto administrativo
                        if (CapitalAmount <= 0)
                        {
                            if (MontoRemanenteDelPago > 0)
                            {
                                //agregar a gasto administrativo: crear gasto administrativo
                                var GastoA = new Cargo();
                                GastoA.IdMensualidad = MensualidadEnUso.Id;
                                GastoA.Monto = MontoRemanenteDelPago;
                                GastoA.Tipo = 1;
                                GastoA.Descripcion = "Remanente de la liquidación de crédito";
                                db.Cargo.Add(GastoA);
                                db.SaveChanges();

                                if (MontoRemanenteDelPago > 0) {
                                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosAdministrativos, MontoRemanenteDelPago, "Pago generado por remanente de liquidación " + MensualidadEnUso.FechaPago.ToShortDateString());
                                }
                                

                                MontoRemanenteDelPago = 0;
                                MONTO_ADMINISTRATIVO_APLICADO += GastoA.Monto;
                                MensualidadEnUso.PagoAcumuladoGastoAdmon += GastoA.Monto;
                            }
                            //guardar mensualidad
                            MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                            db.Entry(MensualidadEnUso).State = EntityState.Modified;
                            db.SaveChanges();
                            //liquidar    
                            MManagement.LiquidateCredit();
                            break;
                        }
                        else
                        {
                            //actualizar mensualidad
                            MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                            db.Entry(MensualidadEnUso).State = EntityState.Modified;
                            db.SaveChanges();
                            if (MONTO_CAPITAL_APLICADO > 0)
                                MManagement.ActualizarPorAbonoACApital(MensualidadEnUso.Mensualidad1, MONTO_CAPITAL_APLICADO);
                            if (MontoRemanenteDelPago <= 0)
                                break;
                        }
                    }
                }
                else
                {
                    //actualizar mensualidad
                    MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion 
            }

            AplicacionDelPago.GPS = MONTO_GPS_APLICADO;
            AplicacionDelPago.Pension = MONTO_PENSION_APLICADO;
            AplicacionDelPago.GastosAdmon = MONTO_ADMINISTRATIVO_APLICADO;
            AplicacionDelPago.GastosCobrenza = MONTO_COBRANZA_APLICADO;
            AplicacionDelPago.Seguro = MONTO_SEGURO_APLICADO;
            AplicacionDelPago.Interes = MONTO_INTERES_APLICADO;
            AplicacionDelPago.Capital = MONTO_AMORTIZACION_APLICADO + MONTO_CAPITAL_APLICADO;
            db.Entry(AplicacionDelPago).State = EntityState.Modified;
            db.SaveChanges();

        }

        public void EntersPaymentMoratoriosAdjuficado(decimal MONTOPAGOINGRESADO, DateTime fechaDePago, List<PagoMora> pagosMoras, String Concepto, int cuenta, DateTime FechaAdjudicacion)
        {
            #region Crear pago
            //Respaldar Amortización antes del Pago
            AplicacionPago AplicacionDelPago = new AplicacionPago();
            AplicacionDelPago.IdCredito = Credit.id;
            AplicacionDelPago.FechaPago = fechaDePago;
            AplicacionDelPago.FechaCaptura = DateTime.Today;
            AplicacionDelPago.IdTipoPago = (int)Enums.TipoDePago.PagoNormal;
            AplicacionDelPago.Monto = MONTOPAGOINGRESADO;
            AplicacionDelPago.Concepto = Concepto;
            AplicacionDelPago.IdCuentasPagos = cuenta;
            db.AplicacionPago.Add(AplicacionDelPago);
            db.SaveChanges();
            RespaldarAmortizacionPrePago(AplicacionDelPago.Id);

            //Aplicar Moras en caso de existir
            //Pagar Moratorios
            decimal montoMora = 0;
            if (pagosMoras.Count > 0)
            {
                montoMora = PagarMoratorios(MONTOPAGOINGRESADO, pagosMoras, AplicacionDelPago.Id);

            }
            AplicacionDelPago.Moratorios = montoMora;
            MONTOPAGOINGRESADO -= montoMora;

            #endregion

            #region Activacion de mensualidad en caso de adelanto a capital
            //Se activa la mensualidad en caso de que el pago pertenesca a adelanto de capital.
            var mes = Credit.Mensualidad.Where(e => e.Activo == false).OrderByDescending(e => e.Mensualidad1).FirstOrDefault();
            if (mes != null)
            {
                if (mes.FechaPago.AddDays(10) >= FechaAdjudicacion)
                {
                    mes.Activo = true;
                    db.Entry(mes).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            #endregion

            #region Declaracion de variables

            decimal MontoRemanenteDelPago = MONTOPAGOINGRESADO;
            //Valor acumulado por concepto aplicado al pago
            decimal MONTO_ADMINISTRATIVO_APLICADO = 0;
            decimal MONTO_COBRANZA_APLICADO = 0;
            decimal MONTO_GPS_APLICADO = 0;
            decimal MONTO_PENSION_APLICADO = 0;
            decimal MONTO_SEGURO_APLICADO = 0;
            decimal MONTO_INTERES_APLICADO = 0;
            decimal MONTO_AMORTIZACION_APLICADO = 0;
            decimal MONTO_CAPITAL_APLICADO = 0;

            #endregion

            while (MontoRemanenteDelPago > 0)
            {

                /*Order of application:
                 * GPSAmount, Pension, Administrative Expenditure, Insurance, Interest, Moratory Interest, Amortization and Capital
                 */
                //Se busca la mensualidad activa siguiente
                var MensualidadEnUso = Credit.Mensualidad.Where(e => e.Activo == true).OrderBy(e => e.Mensualidad1).FirstOrDefault();

                //Si es el crédito es sin pensión
                #region Aplicación de GPS
                if (Credit.TipoDeCredito.Pensiona == false)
                {
                    /*Pago GPS:
                     * Calcular monto que falta por cubrir: Pago normal de gps - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado del GPS
                     */
                    //Monto deudor de GPS    

                    decimal GPS_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.GPS).Sum(s => s.MontoAplicado);

                    decimal FaltanteGPS = MensualidadEnUso.GPS - MensualidadEnUso.PagoAcumuladoGPS - GPS_Condonado;
                    //Actualización de monto por aplicar
                    MontoRemanenteDelPago -= FaltanteGPS;
                    //Actualización de monto aplicado para el PAGO
                    decimal MontoNetoGps = (MontoRemanenteDelPago < 0) ? FaltanteGPS - Math.Abs(MontoRemanenteDelPago) : FaltanteGPS;
                    MONTO_GPS_APLICADO += MontoNetoGps;
                    //Actualización de monto aplicado para el acumulado de la mensualidad
                    MensualidadEnUso.PagoAcumuladoGPS += MontoNetoGps;
                    //Se agrega el detalle del Pago
                    if (MontoNetoGps > 0)
                    {
                        AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GPS, MontoNetoGps, "GPS " + MensualidadEnUso.FechaPago.ToShortDateString());
                    }

                    if (MontoRemanenteDelPago <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(MensualidadEnUso).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                #endregion

                #region Aplicacion de la pensión
                else
                {

                    /*Pay Pensión:
                     * Calcular monto que falta por cubrir: Pago normal de pensión - el pago acumulado
                     * restar a el monto por aplicar el monto pagado al pago acumulado de la pensión
                     */

                    decimal Pension_Condonada = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Pensión).Sum(s => s.MontoAplicado);

                    decimal FaltantePension = MensualidadEnUso.Pension - MensualidadEnUso.PagoAcumuladoPension - Pension_Condonada;
                    MontoRemanenteDelPago -= FaltantePension;
                    decimal MontoNetoPension = (MontoRemanenteDelPago < 0) ? FaltantePension - Math.Abs(MontoRemanenteDelPago) : FaltantePension;
                    MONTO_PENSION_APLICADO += MontoNetoPension;
                    //Add detalle de pago pension
                    if (MontoNetoPension > 0)
                    {
                        AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Pension, MontoNetoPension, "Pensión " + MensualidadEnUso.FechaPago.ToShortDateString());
                    }


                    MensualidadEnUso.PagoAcumuladoPension += (MontoRemanenteDelPago < 0) ? FaltantePension - Math.Abs(MontoRemanenteDelPago) : FaltantePension;
                    if (MontoRemanenteDelPago <= 0)
                    {
                        //actualizar mensualidad
                        db.Entry(MensualidadEnUso).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    }
                }
                #endregion

                #region Aplicación de gastos Administrativos
                decimal FaltanteGastosADMON = MensualidadEnUso.Cargo.Where(e => e.TipoCargo.Administrativo == true).Sum(e => e.Monto) - MensualidadEnUso.PagoAcumuladoGastoAdmon;
                MontoRemanenteDelPago -= FaltanteGastosADMON;
                decimal MontoNetoGastosADMON = (MontoRemanenteDelPago < 0) ? FaltanteGastosADMON - Math.Abs(MontoRemanenteDelPago) : FaltanteGastosADMON;
                MONTO_ADMINISTRATIVO_APLICADO += MontoNetoGastosADMON;

                if (MontoNetoGastosADMON > 0)
                {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosAdministrativos, MontoNetoGastosADMON, "Gastos Administrativos " + MensualidadEnUso.FechaPago.ToShortDateString());
                }


                MensualidadEnUso.PagoAcumuladoGastoAdmon += MontoNetoGastosADMON;
                if (MontoRemanenteDelPago <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicación de gastos Cobranza
                decimal FaltanteGastosCobranza = MensualidadEnUso.Cargo.Where(e => e.TipoCargo.Id == (int)Enums.CatTipoCargo.GastosCobranza).Sum(e => e.Monto) - MensualidadEnUso.PagoAcumuladoGastosCobrenza;
                MontoRemanenteDelPago -= FaltanteGastosCobranza;
                decimal MontoNetoGastosCobranza = (MontoRemanenteDelPago < 0) ? FaltanteGastosCobranza - Math.Abs(MontoRemanenteDelPago) : FaltanteGastosCobranza;
                MONTO_COBRANZA_APLICADO += MontoNetoGastosCobranza;

                if (MontoNetoGastosCobranza > 0)
                {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosCobrenza, MontoNetoGastosCobranza, "Gastos de cobranza " + MensualidadEnUso.FechaPago.ToShortDateString());
                }


                MensualidadEnUso.PagoAcumuladoGastosCobrenza += MontoNetoGastosCobranza;
                if (MontoRemanenteDelPago <= 0)
                {
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicacion a pago de seguro
                //PAgo a Seguro

                decimal Seguro_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Seguro).Sum(s => s.MontoAplicado);

                decimal FaltantePagoASeguro = MensualidadEnUso.Seguro - MensualidadEnUso.PagoAcumuladoSeguro - Seguro_Condonado;
                MontoRemanenteDelPago -= FaltantePagoASeguro;
                decimal MontoNetoSeguro = (MontoRemanenteDelPago < 0) ? FaltantePagoASeguro - Math.Abs(MontoRemanenteDelPago) : FaltantePagoASeguro;
                MONTO_SEGURO_APLICADO += MontoNetoSeguro;
                if (MontoNetoSeguro > 0)
                {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Seguro, MontoNetoSeguro, "Seguro " + MensualidadEnUso.FechaPago.ToShortDateString());
                }

                MensualidadEnUso.PagoAcumuladoSeguro += MontoNetoSeguro;
                if (MontoRemanenteDelPago <= 0)
                {
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                #region Aplicación de Pago a intereses
                //Pay Interest

                decimal Interes_Condonado = MensualidadEnUso.Condonacion.Where(e => e.TipoCondonacion.Id == (int)Enums.CatTipoCondonacion.Interes).Sum(s => s.MontoAplicado);

                decimal FaltantePagoAInteres = MensualidadEnUso.Interes - MensualidadEnUso.PagoAcumuladoInteres - Interes_Condonado;
                MontoRemanenteDelPago -= FaltantePagoAInteres;
                decimal MontoNetoInteres = (MontoRemanenteDelPago < 0) ? FaltantePagoAInteres - Math.Abs(MontoRemanenteDelPago) : FaltantePagoAInteres;
                MONTO_INTERES_APLICADO += MontoNetoInteres;

                if (MontoNetoInteres > 0)
                {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Interes, MontoNetoInteres, "Interes " + MensualidadEnUso.FechaPago.ToShortDateString());
                }

                MensualidadEnUso.PagoAcumuladoInteres += MontoNetoInteres;
                if (MontoRemanenteDelPago <= 0)
                {
                    if (!Credit.TipoDeCredito.Amortiza)
                    {
                        if (MensualidadEnUso.Interes == MensualidadEnUso.PagoAcumuladoInteres)
                        {
                            MensualidadEnUso.Activo = false;
                        }
                    }
                    //actualizar mensualidad
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion

                var cont_plaso = Credit.Mensualidad.Where(z => z.CapitalDeudor <= 0).Count();
                /*----Pay Amortization----*/
                //Si es crédito con amortización o es la última mensualidad de un crédito sin amortización
                var PlazoTotal = (((Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses) + Credit.getPlazoMes) - cont_plaso;

                #region Aplicación de pago a Amortización
                decimal FaltantePagoAmortizacion = MensualidadEnUso.Amortizacion - MensualidadEnUso.PagoAcumuladoAmortizacion;
                MontoRemanenteDelPago -= FaltantePagoAmortizacion;
                decimal montoNetoAmortizacion = (MontoRemanenteDelPago < 0) ? FaltantePagoAmortizacion - Math.Abs(MontoRemanenteDelPago) : FaltantePagoAmortizacion;
                MONTO_AMORTIZACION_APLICADO += montoNetoAmortizacion;
                if (montoNetoAmortizacion > 0)
                {
                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Amortización, montoNetoAmortizacion, "Amortización " + MensualidadEnUso.FechaPago.ToShortDateString());
                }

                MensualidadEnUso.PagoAcumuladoAmortizacion += montoNetoAmortizacion;



                //instanciar manejo de la mensualidad
                MounthlyManagement MManagement = new MounthlyManagement(ref Credit, ref db);
                //---si se cubre el monto de la amortización
                if (MensualidadEnUso.Amortizacion == MensualidadEnUso.PagoAcumuladoAmortizacion)
                {
                    //Dejar inactiva la mensualidad
                    MensualidadEnUso.Activo = false;
                    MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();

                    if (MontoRemanenteDelPago > 0)
                    {
                        //calcular deuda después de amortización
                        decimal CapitalAmount = MensualidadEnUso.CapitalDeudor - MensualidadEnUso.Amortizacion - MensualidadEnUso.PagoCapitalDeudor;

                        //Si es la última mensualidad o no existen más mensualidades con mora
                        if (MensualidadEnUso.Mensualidad1 == PlazoTotal || FechaAdjudicacion <= MensualidadEnUso.FechaPago.AddDays(10) || FechaAdjudicacion == MensualidadEnUso.FechaPago)
                        {
                            //Pay Capital
                            MontoRemanenteDelPago -= CapitalAmount; 
                                                       
                            decimal montoCapitalneto = (MontoRemanenteDelPago < 0) ? CapitalAmount - Math.Abs(MontoRemanenteDelPago) : CapitalAmount;                            
                            MONTO_CAPITAL_APLICADO += montoCapitalneto;
                            MensualidadEnUso.PagoCapitalDeudor += montoCapitalneto;
                            if (montoCapitalneto > 0)
                            {
                                AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.Capital, montoCapitalneto, "Pago a Capital " + MensualidadEnUso.FechaPago.ToShortDateString());
                            }
                            CapitalAmount -= MensualidadEnUso.PagoCapitalDeudor;
                        }

                        //si es la última mensualidad se reparte el sobrante como gasto administrativo
                        if (CapitalAmount <= 0)
                        {
                            if (MontoRemanenteDelPago > 0)
                            {
                                //agregar a gasto administrativo: crear gasto administrativo
                                var GastoA = new Cargo();
                                GastoA.IdMensualidad = MensualidadEnUso.Id;
                                GastoA.Monto = MontoRemanenteDelPago;
                                GastoA.Tipo = 1;
                                GastoA.Descripcion = "Remanente de la liquidación de crédito";
                                db.Cargo.Add(GastoA);
                                db.SaveChanges();

                                if (MontoRemanenteDelPago > 0)
                                {
                                    AddDetallePago(AplicacionDelPago.Id, MensualidadEnUso.Id, (int)Enums.TipoDetallePago.GastosAdministrativos, MontoRemanenteDelPago, "Pago generado por remanente de liquidación " + MensualidadEnUso.FechaPago.ToShortDateString());
                                }


                                MontoRemanenteDelPago = 0;
                                MONTO_ADMINISTRATIVO_APLICADO += GastoA.Monto;
                                MensualidadEnUso.PagoAcumuladoGastoAdmon += GastoA.Monto;
                            }
                            //guardar mensualidad
                            MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                            db.Entry(MensualidadEnUso).State = EntityState.Modified;
                            db.SaveChanges();
                            //liquidar    
                            MManagement.LiquidateCredit();
                            break;
                        }
                        else
                        {
                            //actualizar mensualidad
                            MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                            db.Entry(MensualidadEnUso).State = EntityState.Modified;
                            db.SaveChanges();
                            if (MONTO_CAPITAL_APLICADO > 0)
                                MManagement.ActualizarPorAbonoACApital(MensualidadEnUso.Mensualidad1, MONTO_CAPITAL_APLICADO);
                            if (MontoRemanenteDelPago <= 0)
                                break;
                        }
                    }
                }
                else
                {
                    //actualizar mensualidad
                    MensualidadEnUso = NormalizarMensualidad(MensualidadEnUso);
                    db.Entry(MensualidadEnUso).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
                #endregion 
            }

            AplicacionDelPago.GPS = MONTO_GPS_APLICADO;
            AplicacionDelPago.Pension = MONTO_PENSION_APLICADO;
            AplicacionDelPago.GastosAdmon = MONTO_ADMINISTRATIVO_APLICADO;
            AplicacionDelPago.GastosCobrenza = MONTO_COBRANZA_APLICADO;
            AplicacionDelPago.Seguro = MONTO_SEGURO_APLICADO;
            AplicacionDelPago.Interes = MONTO_INTERES_APLICADO;
            AplicacionDelPago.Capital = MONTO_AMORTIZACION_APLICADO + MONTO_CAPITAL_APLICADO;
            db.Entry(AplicacionDelPago).State = EntityState.Modified;
            db.SaveChanges();

        }

        public decimal PagarMoratorios(decimal motoPago, List<PagoMora> pagosMoras, int idPago)
        {
            foreach (PagoMora p in pagosMoras.ToList()) {
                //Se agrega la cantidad a PagoAcumuladoInteresMoratorio en la mensualidad.
                Mensualidad mensualidad = Credit.Mensualidad.Where(m => m.Id == p.idMensualidad).FirstOrDefault();
                mensualidad.PagoAcumuladoInteresMoratorio += p.monto;
                db.Entry(mensualidad).State = EntityState.Modified;
                db.SaveChanges();

                //Agregar a mora
                Mora mora = db.Mora.Where(a => a.IdMensualidad == mensualidad.Id).FirstOrDefault();
                mora.MontoAplicado += p.monto;
                mora.Activo = false;
                mora.Contabilizado = true;
                db.Entry(mensualidad).State = EntityState.Modified;
                db.SaveChanges();

                //Se agrega detalle del pago                 
                AddDetallePago(idPago, mensualidad.Id, (int)Enums.TipoDetallePago.Moratorios, p.monto, p.conceptoMora);


            }
            return pagosMoras.Sum(s => s.monto);
        }

        public bool AddDetallePago(int idPago, int idMensualidad, int tipoDetallePago, decimal monto, string conceptoMora) {
            bool result = true;
            DetalleDePago detallePago = new DetalleDePago();
            detallePago.IdPago = idPago;
            detallePago.IdMensualidad = idMensualidad;
            detallePago.IdTipoDetallePago = tipoDetallePago;
            detallePago.Monto = monto;
            detallePago.Concepto = conceptoMora;
            db.DetalleDePago.Add(detallePago);
            db.SaveChanges();
            return result;
        }

        public void RespaldarAmortizacionPrePago(int idPago)
        {
            var mensualidades = Credit.Mensualidad.ToList();
            List<MensualidadPago> lista = new List<MensualidadPago>();
            foreach (Mensualidad mount in mensualidades) {
                MensualidadPago mountRespaldo = new MensualidadPago();
                mountRespaldo.Id_AplicacionPago = idPago;
                mountRespaldo.Id = mount.Id;
                mountRespaldo.IdCredito = mount.IdCredito;
                mountRespaldo.Mensualidad = mount.Mensualidad1;
                mountRespaldo.FechaPago = mount.FechaPago;
                mountRespaldo.CapitalDeudor = mount.CapitalDeudor;
                mountRespaldo.GPS = mount.GPS;
                mountRespaldo.Pension = mount.Pension;
                mountRespaldo.Seguro = mount.Seguro;
                mountRespaldo.Interes = mount.Interes;
                mountRespaldo.Amortizacion = mount.Amortizacion;
                mountRespaldo.PagoAcumuladoGastoAdmon = mount.PagoAcumuladoGastoAdmon;
                mountRespaldo.PagoAcumuladoGPS = mount.PagoAcumuladoGPS;
                mountRespaldo.PagoAcumuladoPension = mount.PagoAcumuladoPension;
                mountRespaldo.PagoAcumuladoSeguro = mount.PagoAcumuladoSeguro;
                mountRespaldo.PagoAcumuladoInteres = mount.PagoAcumuladoInteres;
                mountRespaldo.PagoAcumuladoInteresMoratorio = mount.PagoAcumuladoInteresMoratorio;
                mountRespaldo.PagoAcumuladoAmortizacion = mount.PagoAcumuladoAmortizacion;
                mountRespaldo.PagoCapitalDeudor = mount.PagoCapitalDeudor;
                mountRespaldo.Mora = mount.Mora;
                mountRespaldo.Contabilizado = mount.Contabilizado;
                mountRespaldo.Activo = mount.Activo;
                mountRespaldo.adjudicado = mount.adjudicado;
                mountRespaldo.PagoAcumuladoGastosCobrenza = mount.PagoAcumuladoGastosCobrenza;
                lista.Add(mountRespaldo);
            }
            db.MensualidadPago.AddRange(lista);
            db.SaveChanges();
        }

        public void RegenerarAmortizacionPostPago(int idPago) {
            var mensualidades = db.MensualidadPago.Where(mp => mp.Id_AplicacionPago == idPago);

            foreach (MensualidadPago mount in mensualidades)
            {
                Mensualidad mountRespaldo = new Mensualidad();
                mountRespaldo.Id = mount.Id;
                mountRespaldo.IdCredito = mount.IdCredito;
                mountRespaldo.Mensualidad1 = mount.Mensualidad;
                mountRespaldo.FechaPago = mount.FechaPago;
                mountRespaldo.CapitalDeudor = mount.CapitalDeudor;
                mountRespaldo.GPS = mount.GPS;
                mountRespaldo.Pension = mount.Pension;
                mountRespaldo.Seguro = mount.Seguro;
                mountRespaldo.Interes = mount.Interes;
                mountRespaldo.Amortizacion = mount.Amortizacion;
                mountRespaldo.PagoAcumuladoGastoAdmon = mount.PagoAcumuladoGastoAdmon;
                mountRespaldo.PagoAcumuladoGPS = mount.PagoAcumuladoGPS;
                mountRespaldo.PagoAcumuladoPension = mount.PagoAcumuladoPension;
                mountRespaldo.PagoAcumuladoSeguro = mount.PagoAcumuladoSeguro;
                mountRespaldo.PagoAcumuladoInteres = mount.PagoAcumuladoInteres;
                mountRespaldo.PagoAcumuladoInteresMoratorio = mount.PagoAcumuladoInteresMoratorio;
                mountRespaldo.PagoAcumuladoAmortizacion = mount.PagoAcumuladoAmortizacion;
                mountRespaldo.PagoCapitalDeudor = mount.PagoCapitalDeudor;
                mountRespaldo.Mora = mount.Mora;
                mountRespaldo.Contabilizado = mount.Contabilizado;
                mountRespaldo.Activo = mount.Activo;
                mountRespaldo.adjudicado = mount.adjudicado;
                mountRespaldo.PagoAcumuladoGastosCobrenza = mount.PagoAcumuladoGastosCobrenza;
                mountRespaldo = NormalizarMensualidad(mountRespaldo);
                var mora = db.Mora.Where(m => m.IdMensualidad == mount.Id).FirstOrDefault();
                if (mora != null) {
                    mora.MontoAplicado = mount.PagoAcumuladoInteresMoratorio;
                    db.Entry(mora).State = EntityState.Modified;
                }
                db.Entry(mountRespaldo).State = EntityState.Modified;                
            }
            db.SaveChanges();            
        }
    }
}
