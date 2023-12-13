using AutoyVaro;
using AutoyVaro.Models;
using System;
using System.Data.Entity;
using System.Linq;
using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Helpers.EnumHelper;

namespace AutoyVaro.Management
{
    public class MounthlyManagement:IDisposable
    {
        bool disposed = false;
        //instanciar crédito a tratar
        private Credito Credit;
        //Instanciar las entidades
        private Entities db;
        public MounthlyManagement(ref Credito Credit, ref Entities db)
        {
            this.Credit = Credit;
            this.db = db;
        }
        public MounthlyManagement(int Id)
        {
            this.db = new Entities();
            //this.db = db;
            this.Credit = db.Credito.Find(Id);

        }
        #region
        public void CreateMonthlyPaymentTable()
        {
            var TablaAmortizacion = Credit.GetAmortizacion;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            int ContadorMes = 1;
            foreach (Amortizacion Amorti in TablaAmortizacion)
            {
                //instanciar mensualidad
                Mensualidad mountly = new Mensualidad();
                //llenar mensualidad
                mountly.IdCredito = Credit.id;
                mountly.Mensualidad1 = ContadorMes;
                mountly.FechaPago = Amorti.FechaDePago;
                mountly.CapitalDeudor = Amorti.Capital;

                if (Credit.TipoDeCredito.Pensiona == true)
                    mountly.Pension = Amorti.GastoAdministrativo;
                else
                    mountly.GPS = Amorti.GastoAdministrativo;

                mountly.Seguro = Amorti.Seguro;
                mountly.Interes = Amorti.Interes;

                if (Credit.TipoDeCredito.Amortiza == true)
                    mountly.Amortizacion = Amorti.valor - Amorti.Interes;
                else if (ContadorMes == TablaAmortizacion.Count)
                    mountly.Amortizacion = Amorti.valor - Amorti.Interes;
                else if (ContadorMes != TablaAmortizacion.Count)
                    mountly.Amortizacion = 0;

                if (ContadorMes <= NumeroMesesPromocion)
                    mountly.Activo = false;
                else
                    mountly.Activo = true;

                //Guardar mensualidad
                db.Mensualidad.Add(mountly);
                db.SaveChanges();
                //aumentar contador
                ContadorMes++;
            }
        }
        public void RegenerateMonthlyPaymentTable()
        {
            //Seleccionar mensualidades
            var mensualidades = Credit.Mensualidad.OrderBy(e => e.Mensualidad1);
            var TablaAmortizacion = Credit.GetAmortizacion;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            int ContadorMes = 1;

            foreach (Amortizacion Amorti in TablaAmortizacion)
            {
                var mounthly = mensualidades.Where(e => e.Mensualidad1.Equals(ContadorMes)).FirstOrDefault();
                //Restaurar mensualidad
                mounthly.CapitalDeudor = Amorti.Capital;

                if (Credit.TipoDeCredito.Pensiona == true)
                {
                    mounthly.Pension = Amorti.GastoAdministrativo;
                    mounthly.GPS = 0;
                }
                else
                {
                    mounthly.GPS = Amorti.GastoAdministrativo;
                    mounthly.Pension = 0;
                }

                mounthly.Seguro = Amorti.Seguro;
                mounthly.Interes = Amorti.Interes;

                if (Credit.TipoDeCredito.Amortiza == true)
                    mounthly.Amortizacion = Amorti.valor - Amorti.Interes;
                else if (ContadorMes == TablaAmortizacion.Count)
                    mounthly.Amortizacion = Amorti.valor - Amorti.Interes;
                else if (ContadorMes != TablaAmortizacion.Count)
                    mounthly.Amortizacion = 0;

                mounthly.PagoAcumuladoGastoAdmon = 0;
                mounthly.PagoAcumuladoGastosCobrenza = 0;
                mounthly.PagoAcumuladoGPS = 0;
                mounthly.PagoAcumuladoPension = 0;
                mounthly.PagoAcumuladoSeguro = 0;
                mounthly.PagoAcumuladoInteres = 0;
                mounthly.PagoAcumuladoInteresMoratorio = 0;
                mounthly.PagoAcumuladoAmortizacion = 0;
                mounthly.PagoCapitalDeudor = 0;
                mounthly.Mora = 0;
                mounthly.Contabilizado = false;
                mounthly.adjudicado = false;

                eliminarDependencias(mounthly.Id);
                if (ContadorMes <= NumeroMesesPromocion)
                    mounthly.Activo = false;
                else
                    mounthly.Activo = true;

                //actualizar mensualidad
                db.Entry(mounthly).State = EntityState.Modified;
                db.SaveChanges();
                //aumentar contador
                ContadorMes++;
            }

            var pagos = db.AplicacionPago.Where(a => a.IdCredito == Credit.id);
            if (pagos != null)
            {
                foreach (AplicacionPago m in pagos.ToList())
                {
                    db.DetalleDePago.RemoveRange(m.DetalleDePago);
                    db.SaveChanges();
                    db.AplicacionPago.Remove(m);
                }
                db.SaveChanges();
            }

        }
        public void eliminarDependencias(int idMensualidad) {
            var moras = db.Mora.Where(a => a.IdMensualidad == idMensualidad);
            if (moras != null) {
                foreach (Mora m in moras.ToList()) {
                    db.Mora.Remove(m);                    
                }
                db.SaveChanges();
            }
            var cargos = db.Cargo.Where(a => a.IdMensualidad == idMensualidad);
            if (cargos != null)
            {
                foreach (Cargo c in cargos.ToList())
                {
                    db.Cargo.Remove(c);
                }
                db.SaveChanges();
            }
            var condonaciones = db.Condonacion.Where(a => a.IdMensualidad == idMensualidad);
            if (condonaciones != null)
            {
                foreach (Condonacion co in condonaciones.ToList())
                {
                    db.Condonacion.Remove(co);
                }
                db.SaveChanges();
            }
            var CPM = db.CrucePagoMensualidad.Where(a => a.IdMensualidad == idMensualidad);
            if (CPM != null)
            {
                foreach (CrucePagoMensualidad cp in CPM.ToList())
                {
                    db.CrucePagoMensualidad.Remove(cp);
                }
                db.SaveChanges();
            }
        }
        
        public void ActualizarPorAbonoACApital(int NumberMonthly, decimal Amount) {

            var Mensualidades = Credit.Mensualidad.Where(e => e.Mensualidad1 > NumberMonthly).OrderBy(e => e.Mensualidad1);


            var Mensualidad = new Mensualidad();/// Mensualidades==null ? nul-l : Mensualidades.First();

            if (Mensualidades.Count() == 0)
            {
                Mensualidad = Credit.Mensualidad.Where(e => e.Mensualidad1 == NumberMonthly).OrderBy(e => e.Mensualidad1).First();
            }
            else {
                Mensualidad = Mensualidades.First();
            }

            decimal CapitalDeudor = Mensualidad.CapitalDeudor - Amount;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            //calcular el número de mensualidades restantes, considerando las mensualidades de la promoción                
            int PlazoTotal = NumeroMesesPromocion + Credit.getPlazoMes;
            int PlazoRestante = PlazoTotal - NumberMonthly;



            if (Credit.TipoDeCredito.Amortiza == true)
            {
                var PagoMensual = (decimal)Credit.getPagoMensual;
                foreach (var mounthly in Mensualidades)
                {
                    if (CapitalDeudor <= 0)
                    {
                        mounthly.CapitalDeudor = 0;
                        mounthly.Interes = 0;
                        mounthly.Amortizacion = 0;
                    }
                    else {
                        mounthly.CapitalDeudor = CapitalDeudor;

                        if (mounthly.Mensualidad1 > NumeroMesesPromocion) {
                            mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;

                            if ((PagoMensual - mounthly.Interes) < CapitalDeudor)
                            {
                                mounthly.Amortizacion = PagoMensual - mounthly.Interes;
                            }
                            else
                            {
                                mounthly.Amortizacion = CapitalDeudor;
                            }
                        }

                        CapitalDeudor -= mounthly.Amortizacion;
                    }
                    
                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    NormalizarMensualidad(mounthly);
                    db.SaveChanges();
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }
            }
            else
            {
                foreach (var mounthly in Mensualidades)
                {
                    //Regenerate Monthly Payment (capital deudor, interés y amortización)

                    //if (CapitalDeudor <= 0)
                    //{
                    //    mounthly.CapitalDeudor = 0;
                    //    mounthly.Interes = 0;
                    //}
                    //else
                    //{
                    mounthly.CapitalDeudor = CapitalDeudor;
                    mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;

                    //}






                    //mounthly.CapitalDeudor = CapitalDeudor;
                    //mounthly.Interes = CapitalDeudor * Credit.PorcentajeInteres;
                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    //Si es la última mensualidad..
                    if (mounthly.Mensualidad1 == PlazoTotal)
                    {
                        mounthly.Amortizacion = CapitalDeudor;
                    }
                    else
                    {
                        mounthly.Amortizacion = 0;
                    }
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }

            }
        }



        public void ActualizarCapitalAlargarPlazo(int NumberMonthly, decimal Amount)
        {

            var Mensualidades = Credit.Mensualidad.Where(e => e.Mensualidad1 > NumberMonthly).OrderBy(e => e.Mensualidad1);


            var Mensualidad = new Mensualidad();/// Mensualidades==null ? nul-l : Mensualidades.First();

            if (Mensualidades.Count() == 0)
            {
                Mensualidad = Credit.Mensualidad.Where(e => e.Mensualidad1 == NumberMonthly).OrderBy(e => e.Mensualidad1).First();
            }
            else
            {
                Mensualidad = Mensualidades.First();
            }

            decimal CapitalDeudor = Mensualidad.CapitalDeudor - Amount;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            //calcular el número de mensualidades restantes, considerando las mensualidades de la promoción                
            int PlazoTotal = NumeroMesesPromocion + Credit.getPlazoMes;
            int PlazoRestante = PlazoTotal - NumberMonthly;

            if (Credit.TipoDeCredito.Amortiza == true)
            {
                var PagoMensual = (decimal)Credit.getPagoMensual;
                foreach (var mounthly in Mensualidades)
                {
                    if (CapitalDeudor <= 0)
                    {
                        mounthly.CapitalDeudor = 0;
                        mounthly.Interes = 0;
                        mounthly.Amortizacion = 0;
                    }
                    else
                    {
                        mounthly.CapitalDeudor = CapitalDeudor;
                        mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;

                        if ((PagoMensual - mounthly.Interes) < CapitalDeudor)
                        {
                            mounthly.Amortizacion = PagoMensual - mounthly.Interes;
                        }
                        else
                        {
                            mounthly.Amortizacion = CapitalDeudor;
                        }



                        CapitalDeudor -= mounthly.Amortizacion;
                    }

                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    NormalizarMensualidad(mounthly);
                    db.SaveChanges();
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }
            }
            else
            {
                foreach (var mounthly in Mensualidades)
                {
                    //Regenerate Monthly Payment (capital deudor, interés y amortización)

                    //if (CapitalDeudor <= 0)
                    //{
                    //    mounthly.CapitalDeudor = 0;
                    //    mounthly.Interes = 0;
                    //}
                    //else
                    //{
                    mounthly.CapitalDeudor = CapitalDeudor;
                    mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;

                    //}






                    //mounthly.CapitalDeudor = CapitalDeudor;
                    //mounthly.Interes = CapitalDeudor * Credit.PorcentajeInteres;
                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    //Si es la última mensualidad..
                    if (mounthly.Mensualidad1 == PlazoTotal)
                    {
                        mounthly.Amortizacion = CapitalDeudor;
                    }
                    else
                    {
                        mounthly.Amortizacion = 0;
                    }
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }

            }
        }


        public void ActualizarCapitalPorEliminacion(int NumberMonthly, decimal Amount)
        {

            var Mensualidades = Credit.Mensualidad.Where(e => e.Mensualidad1 > NumberMonthly).OrderBy(e => e.Mensualidad1);
            var Mensualidad = Mensualidades.First();
            decimal CapitalDeudor = Mensualidad.CapitalDeudor - Amount;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            //calcular el número de mensualidades restantes, considerando las mensualidades de la promoción                
            int PlazoTotal = NumeroMesesPromocion + Credit.getPlazoMes;
            int PlazoRestante = PlazoTotal - NumberMonthly;

            if (Credit.TipoDeCredito.Amortiza == true)
            {
                var PagoMensual = (decimal)Credit.getPagoMensual;
                foreach (var mounthly in Mensualidades)
                {
                    if (CapitalDeudor <= 0)
                    {
                        mounthly.CapitalDeudor = 0;
                        mounthly.Interes = 0;
                        mounthly.Amortizacion = 0;
                    }
                    else
                    {
                        mounthly.CapitalDeudor = CapitalDeudor;
                        mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;

                        if ((PagoMensual - mounthly.Interes) < CapitalDeudor)
                        {
                            mounthly.Amortizacion = PagoMensual - mounthly.Interes;
                        }
                        else
                        {
                            mounthly.Amortizacion = CapitalDeudor;
                        }



                        CapitalDeudor -= mounthly.Amortizacion;
                    }

                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    NormalizarMensualidad(mounthly);
                    db.SaveChanges();
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }
            }
            else
            {
                foreach (var mounthly in Mensualidades)
                {
                    //Regenerate Monthly Payment (capital deudor, interés y amortización)
                    mounthly.CapitalDeudor = CapitalDeudor;
                    mounthly.Interes = (CapitalDeudor * Credit.PorcentajeInteres) * (decimal)1.16;
                    //Limpiar pagos                    

                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    //Si es la última mensualidad..
                    if (mounthly.Mensualidad1 == PlazoTotal)
                    {
                        mounthly.Amortizacion = CapitalDeudor;
                    }
                    else
                    {
                        mounthly.Amortizacion = 0;
                    }
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }

            }
        }




        public void regenerarTabla(int NumberMonthly, decimal Amount)
        {
            var Mensualidades = Credit.Mensualidad.Where(e => e.Mensualidad1 > NumberMonthly).OrderBy(e => e.Mensualidad1);
            var Mensualidad = Mensualidades.First();
            decimal CapitalDeudor = Mensualidad.CapitalDeudor - Amount;
            var NumeroMesesPromocion = (Credit.PromocionSucursal == null) ? 0 : Credit.PromocionSucursal.Promocion.NumeroMeses;
            //calcular el número de mensualidades restantes, considerando las mensualidades de la promoción                
            int PlazoTotal = NumeroMesesPromocion + Credit.getPlazoMes;
            int PlazoRestante = PlazoTotal - NumberMonthly;

            if (Credit.TipoDeCredito.Amortiza == true)
            {
                var PagoMensual = CapitalDeudor / GetFactor(PlazoRestante);
                foreach (var mounthly in Mensualidades)
                {
                    mounthly.CapitalDeudor = CapitalDeudor;
                    mounthly.Interes = CapitalDeudor * Credit.PorcentajeInteres;
                    mounthly.Amortizacion = PagoMensual - mounthly.Interes;
                    CapitalDeudor -= mounthly.Amortizacion;
                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    NormalizarMensualidad(mounthly);
                    db.SaveChanges();
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }
            }
            else
            {
                foreach (var mounthly in Mensualidades)
                {
                    //Regenerate Monthly Payment (capital deudor, interés y amortización)
                    mounthly.CapitalDeudor = CapitalDeudor;
                    mounthly.Interes = CapitalDeudor * Credit.PorcentajeInteres;
                    //Limpiar pagos                    
                    mounthly.PagoAcumuladoGPS = 0;
                    mounthly.PagoAcumuladoPension = 0;
                    mounthly.PagoAcumuladoGastoAdmon = 0;
                    mounthly.PagoAcumuladoSeguro = 0;
                    mounthly.PagoAcumuladoInteres = 0;
                    mounthly.PagoAcumuladoInteresMoratorio = 0;
                    mounthly.PagoAcumuladoAmortizacion = 0;
                    mounthly.PagoCapitalDeudor = 0;
                    mounthly.Contabilizado = false;
                    mounthly.Activo = true;
                    //Si es la última mensualidad..
                    if (mounthly.Mensualidad1 == PlazoTotal)
                    {
                        mounthly.Amortizacion = CapitalDeudor;
                    }
                    else
                    {
                        mounthly.Amortizacion = 0;
                    }
                    //actualizar mensualidad
                    db.Entry(mounthly).State = EntityState.Modified;
                    db.SaveChanges();
                    //Eliminar Cargos de condonaciones vencidas
                    int TipoCargo = Convert.ToInt32(AutoyVaro.Enums.TipoCargo.CondonacionVencida);
                    var cargos = mounthly.Cargo.Where(e => e.Tipo.Equals(TipoCargo));
                    db.Cargo.RemoveRange(cargos);
                    db.SaveChanges();
                }

            }

        }



        public void LiquidateCredit()
        {
            
            var mensualidades = Credit.Mensualidad.Where(a => a.Activo==true).OrderBy(e => e.Mensualidad1);
            foreach (Mensualidad mounthly in mensualidades) {
                mounthly.CapitalDeudor = 0;
                mounthly.GPS = 0;
                mounthly.Pension = 0;
                mounthly.Seguro = 0;
                mounthly.Interes = 0;
                mounthly.Amortizacion = 0;                
                //Limpiar pagos                    
                mounthly.PagoAcumuladoGPS = 0;
                mounthly.PagoAcumuladoPension = 0;
                mounthly.PagoAcumuladoGastoAdmon = 0;
                mounthly.PagoAcumuladoGastosCobrenza = 0;
                mounthly.PagoAcumuladoSeguro = 0;
                mounthly.PagoAcumuladoInteres = 0;
                mounthly.PagoAcumuladoInteresMoratorio = 0;
                mounthly.PagoAcumuladoAmortizacion = 0;
                mounthly.PagoCapitalDeudor = 0;
                mounthly.Contabilizado = true;
                mounthly.Activo = false;
                NormalizarMensualidad(mounthly);
                db.SaveChanges();
                //actualizar mensualidad
                db.Entry(mounthly).State = EntityState.Modified;
            }
        }

        public decimal GetFactor(int MesesRestantes)
        {
            decimal factorPotencia = 0;
            decimal factorA = 0;
            decimal factor;

            factorA = 1 + AutoyVaro.Helpers.UtilitiesHelper.Utilities.MontoMasIVA(Credit.PorcentajeInteres);
            factorPotencia = Convert.ToDecimal(1 - Math.Pow(Convert.ToDouble(factorA), Convert.ToDouble(-(MesesRestantes))));
            factor = factorPotencia / Convert.ToDecimal(AutoyVaro.Helpers.UtilitiesHelper.Utilities.MontoMasIVA(Credit.PorcentajeInteres));
            return factor;
        }
        #endregion


        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
                ((IDisposable)db).Dispose();
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~MounthlyManagement()
        {
            Dispose(false);
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



    }
}