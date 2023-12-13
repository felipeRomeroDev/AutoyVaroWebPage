using AutoyVaro.com.libroazul.www;
using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using AutoyVaro.Extenciones;

namespace AutoyVaro.Controllers
{
    public class SimuladorController : Controller
    {
        Entities db = new Entities();        
        private String llave = "";

        // GET: Simulador
        public ActionResult Index(string telefono, string codigo, decimal monto=0, decimal montoMaximo=0)
        {

            var wsLibrozul = new wslibroazulSoapClient();
            //llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "data*8541");
            //Session["llaveTemp"] = llave;

            ViewBag.IsNull = false;
            SolicitudCotizacion solicitud = new SolicitudCotizacion();
            solicitud = db.SolicitudCotizacion.Where(s => s.Telefono == telefono && s.CodigoValidacionNumero == codigo).FirstOrDefault();

            if (solicitud == null) {
                ViewBag.IsNull = true;
                return View();
            }

            ViewModelCreditos model = new ViewModelCreditos();
            model.solicitudCotizacion = solicitud;
            



            Credito credito24 = ConstruirSimulacionPorEsqueme(2015, solicitud, monto);

            if (montoMaximo == 0)
            {
                ViewBag.MontoMaximo = credito24.EntregaLiquida;
            }
            else {
                ViewBag.MontoMaximo = montoMaximo;
            }
            
            
            List<Credito> listCreditos = new List<Credito>();
            listCreditos.Add(credito24);            

            model.creditos = listCreditos;

            return View(model);
        }


        public PartialViewResult ActualizarCotizacion(string telefono, string codigo, decimal monto = 0, decimal montoMaximo = 0)
        {

            var wsLibrozul = new wslibroazulSoapClient();
            //llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "data*8541");
            //Session["llaveTemp"] = llave;

            ViewBag.IsNull = false;
            SolicitudCotizacion solicitud = new SolicitudCotizacion();
            solicitud = db.SolicitudCotizacion.Where(s => s.Telefono == telefono && s.CodigoValidacionNumero == codigo).FirstOrDefault();

            if (solicitud == null)
            {
                ViewBag.IsNull = true;
                return PartialView("_Cotizacion", null);
            }

            ViewModelCreditos model = new ViewModelCreditos();
            model.solicitudCotizacion = solicitud;




            Credito credito24 = ConstruirSimulacionPorEsqueme(2015, solicitud, monto);

            if (montoMaximo == 0)
            {
                ViewBag.MontoMaximo = credito24.EntregaLiquida;
            }
            else
            {
                ViewBag.MontoMaximo = montoMaximo;
            }


            List<Credito> listCreditos = new List<Credito>();
            listCreditos.Add(credito24);

            model.creditos = listCreditos;

            return PartialView("_Cotizacion",model);
        }

        public Credito ConstruirSimulacionPorEsqueme(int IdEsquema, SolicitudCotizacion solicitud, decimal monto) {
            Esquema esquema = new Esquema();
            esquema = db.Esquema.Find(IdEsquema);
            Credito credito = new Credito();

            var precioLibroAzul = Decimal.Parse(GetPrecioDeCompra(solicitud.ClaveVercion).Compra);

            credito.PorcentajeAvaluo = (solicitud.LA_Marcas.LA_ControlPorcentajeAvaluo.FirstOrDefault().PorcentajeAvaluo ?? 1) / 100;

            

            credito.PrecioDeCompra = precioLibroAzul;

            if (monto >= 30000)
            {
                credito.EntregaLiquida = monto;
            }
            else {
                credito.EntregaLiquida = (precioLibroAzul * credito.PorcentajeAvaluo);
            }

            
            var aperturaTemp = Utilities.MontoMasIVA(credito.EntregaLiquida * SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));
            credito.EntregaLiquida -= aperturaTemp;



            credito.TipoDeCredito = db.TipoDeCredito.Where(e => e.id.Equals(1)).Single();

            credito.PorcentajeInteres = SettingsHelper.getPorcentajeIntereses(IdEsquema);

            credito.CargoGPSAdelantado = esquema.GastosAdministrativosPostEntregaLiquida;
            credito.IVA = SettingsHelper.getIVA();
            credito.AjustePorKilometraje = 0;

            var sumAdeudos = credito.AdeudoTenencia + credito.AdeudoOtros;

            decimal porcentajeDeApertura = Utilities.MontoMasIVA(SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));

            decimal reglaAvaluo = 1 - porcentajeDeApertura;
            var avaluoProcasa = (credito.EntregaLiquida + sumAdeudos + esquema.GastosAdministrativosPostEntregaLiquida) / reglaAvaluo;

            var aperturaCredito = Utilities.MontoMasIVA(avaluoProcasa * SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));

            credito.Esquema = esquema;
            credito.idEsquema = esquema.id;
            credito.Plazo = esquema.Dias;
            credito.AvaluoMercado = credito.AvaluoDeMercadoSuma;
            credito.AvaluoProcasa = avaluoProcasa;
            credito.AperturaCredito = aperturaCredito;
            credito.FechaCredito = DateTime.Today;

            return credito;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CreateSolicitud(ViewModelSolicitud model) {


            string IdentityUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(IdentityUserId);

            Fin_Solicitud solicitud = new Fin_Solicitud();
            Fit_Cliente cliente = new Fit_Cliente();
            Fit_Vehiculo vehiculo = new Fit_Vehiculo();

            SolicitudCotizacion solicitudCotizacion = new SolicitudCotizacion();
            solicitudCotizacion = db.SolicitudCotizacion.Find(model.IdSolicitud);

            if (solicitudCotizacion.IdUsuarioSolicitud == null) {
                solicitudCotizacion.IdUsuarioSolicitud = user.Id;
                db.Entry(solicitudCotizacion).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (user.Fit_Cliente.Count > 0) {
                solicitud.IdCliente = user.Fit_Cliente.FirstOrDefault().Id;
                cliente = user.Fit_Cliente.FirstOrDefault();
            }
            else {                
                cliente.Nombre = model.Nombre;
                cliente.ApellidoPaterno = model.ApellidoPaterno;
                cliente.ApellidoMaterno = model.ApellidoMaterno;
                cliente.RFC=model.RFC;
                cliente.CURP=model.CURP;
                cliente.IdTipoDeIdentificacion = model.IdTipoDeIdentificacion;
                cliente.NumeroIdentificacion = model.NumeroIdentificacion;
                cliente.Calle = model.Calle;
                cliente.Colonia = model.Colonia;
                cliente.NumeroCasa = model.NumeroCasa;
                cliente.EntreCalle = model.EntreCalle;
                cliente.YCalle = model.YCalle;
                cliente.CodigoPostal= model.CodigoPostal;
                cliente.Ciudad = model.Ciudad;
                cliente.Municipio = model.Municipio;
                cliente.IdEstado = model.IdEstado;
                cliente.TelefonoCasa = model.TelefonoCasa;
                cliente.TelefonoOficina = model.TelefonoOficina;
                cliente.Ext = model.Ext; ;
                cliente.Celular = model.Celular;
                cliente.CorreoElectronico = model.CorreoElectronico;
                cliente.FechaCreacion = DateTime.Now;
                cliente.FechaNacimiento = model.FechaNacimiento;
                cliente.IdUser = user.Id;
                cliente.IdEstadoCivil = model.IdEstadoCivil;
                cliente.ActividadPreponderante = model.ActividadPreponderante;
                
                cliente.Nombre1 = model.Nombre1;    
                cliente.ApellidoMaterno1 = model.ApellidoMaterno1;  
                cliente.ApellidoPaterno1 = model.ApellidoPaterno1;  
                cliente.ParentescoR1 = model.ParentescoR1;  
                cliente.DireccionR1 = model.DireccionR1;
                cliente.TelefonoR1 = model.TelefonoR1;


                cliente.Nombre2 = model.Nombre2;
                cliente.ApellidoMaterno2 = model.ApellidoMaterno2;
                cliente.ApellidoPaterno2 = model.ApellidoPaterno2;
                cliente.TelefonoR2 = model.TelefonoR2;

                cliente.Nombre3 = model.Nombre2;
                cliente.ApellidoMaterno3 = model.ApellidoMaterno2;
                cliente.ApellidoPaterno3 = model.ApellidoPaterno2;
                cliente.TelefonoR3 = model.TelefonoR2;

                db.Fit_Cliente.Add(cliente);
                db.SaveChanges();
                solicitud.IdCliente = cliente.Id;
            }


            vehiculo.IdAnioLA = solicitudCotizacion.IdAnioLA;
            vehiculo.IdMarcaLA = solicitudCotizacion.IdMarcaLA;
            vehiculo.IdModeloLA = solicitudCotizacion.IdModeloLA;
            vehiculo.ClaveVercion = solicitudCotizacion.ClaveVercion;
            vehiculo.Compra = solicitudCotizacion.Compra;
            vehiculo.Venta = solicitudCotizacion.Venta;
            vehiculo.NumeroDeSerie = model.NumeroDeSerie;
            vehiculo.NumeroDeMotor = model.NumeroDeMotor;
            vehiculo.Placas = model.Placas;
            vehiculo.TarjetaDeCirculacion = model.TarjetaDeCirculacion;
            vehiculo.IdEstado = model.IdEstadoPlacas;
            vehiculo.FechaSolicitud = DateTime.Now;
            vehiculo.Color = model.Color;
            vehiculo.RFCFactura = model.RFCFactura;
            vehiculo.NumeroFactura = model.NumeroFactura;
            vehiculo.FechaFactura = model.FechaFactura;
            vehiculo.NumeroDeTarjetaDeCirculacion = model.NumeroDeTarjetaDeCirculacion;
            vehiculo.TarjetaDeCirculacion = model.NumeroDeTarjetaDeCirculacion;
            vehiculo.NumeroDePuertas = model.NumeroDePuertas;
            vehiculo.NumeroDeAsientos = model.NumeroDeAsientos;
            vehiculo.NumeroDeCilindros = model.NumeroDeCilindros;
            vehiculo.IdTipoTransmision = model.IdTipoTransmision;
            vehiculo.IdCliente = cliente.Id;
            db.Fit_Vehiculo.Add(vehiculo);
            db.SaveChanges();


            solicitud.IdVehiculo = vehiculo.Id;



            solicitud.FechaSolicitud = DateTime.Now;
            solicitud.Compra = solicitudCotizacion.Compra;
            solicitud.Venta = solicitudCotizacion.Venta;
            solicitud.IdTipoSolicitud = 1;//Basica por defecto
            solicitud.IdEstadoSolicitud = 8;//Basica por defecto
            solicitud.IdEsquema = 2015; //Unico por el momento
            solicitud.MontoSolicitado = model.Monto;
            solicitud.tipoDeSeguiniento = model.tipoDeSeguiniento;
            db.Fin_Solicitud.Add(solicitud);
            db.SaveChanges();


            ObservacionSolicitud observacion = new ObservacionSolicitud();
            observacion.FechaObservacion = DateTime.Now;
            observacion.Observacion = "Se registro una solicitud de crédito a nombre de " + cliente.Nombre + " " + cliente.ApellidoPaterno + " " + cliente.ApellidoMaterno;
            observacion.IdSolicitud = solicitud.Id;
            observacion.Publicar = true;
            observacion.IdUsuario = 1;
            db.ObservacionSolicitud.Add(observacion);
            db.SaveChanges();


            var result = new {
                error = false,
                Mensaje = "Solicitud Enviada con éxito",
                Id = solicitud.Id
            };            

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EnviarRespuesta(int id) {

            Fin_Solicitud solicitud = db.Fin_Solicitud.Find(id);

            Thread ThreadEnviarSolicitud =
              new Thread(
                unused => EnviarMailPreautorizado(solicitud.Id, solicitud.Fit_Vehiculo)
              );
            ThreadEnviarSolicitud.Start();            
            return Content("");
        }


        public void EnviarMailPreautorizado(int  IdSolicitud, Fit_Vehiculo vehiculo) {

            Fin_Solicitud solicitud = db.Fin_Solicitud.Find(IdSolicitud);                        

            MailMessage mail = new MailMessage();
            var datos = db.Settings.FirstOrDefault();
            mail.From = new MailAddress("gerencia@autoyvaro.com", "Auto y Varo SA de CV");            
            
            mail.To.Add(solicitud.Fit_Cliente.CorreoElectronico);
            
            mail.To.Add("areyes@autoyvaro.com");
            mail.To.Add("aserena@autoyvaro.com");
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.To.Add("areyes@autoyvaro.com");
            mail.To.Add("asesor@autoyvaro.com");
            mail.To.Add("aespindola@autoyvaro.com");
            mail.To.Add("gerencia@autoyvaro.com");


            mail.Subject = "Crédito preautorizado";
            mail.Body = "<center>" +
            "<div style=\"max-width: 600px;\" align=\"center\">" +
            "<p>&nbsp;</p>" +
            "<p>&nbsp;</p>" +
            "<table style=\"border-collapse: collapse; width: 100%; height: 90px; background-color: #eee;\" border=\"0\"><colgroup><col style=\"width: 50%;\"><col style=\"width: 50%;\"></colgroup>" +
            "<tbody>" +
            "<tr style=\"height: 72px;\">" +
            "<td style=\"height: 72px;\">&nbsp;</td>" +
            "<td style=\"height: 72px;\"><span style=\"color: rgb(0, 0, 0);\"><em><img style=\"float: right;\" src=\"http://mhuerta85-001-site3.btempurl.com/Content/assets3/img/logo3.png\" alt=\"logo\" width=\"162\" height=\"73\"></em></span></td>" +
            "</tr>" +
            "</tbody>" +
            "</table>" +
            "<p>&nbsp;</p>" +
            "<p style=\"text-align: left;\"><span style=\"font-size: 18pt; color: rgb(0, 0, 0);\"><em>Apreciable Sr(a): <strong>" + solicitud.Fit_Cliente.Nombre + " " + solicitud.Fit_Cliente.ApellidoPaterno + " "+ solicitud.Fit_Cliente.ApellidoMaterno + "</strong> </em></span></p>" +
            "<p>&nbsp;</p>" +
            "<p style=\"text-align: justify;\"><span style=\"font-size: 18pt; color: rgb(0, 0, 0);\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <span style=\"font-family: arial, helvetica, sans-serif;\">Es para nosotros un honor informarle a trav&eacute;s de la presente que hemos recibido su solicitud de cr&eacute;dito, mismo que ha sido preautorizado por la cantidad de " + (solicitud.MontoSolicitado ?? 0).ToCurrencyFormat() + "(" +(solicitud.MontoSolicitado.ToString().MontoALetra()) + "), con un plazo a 24 meses y con una tasa de 7.5% mensual, sobre la unidad en garant&iacute;a: " + db.LA_Marcas.Find(vehiculo.IdMarcaLA).Nombre + " " + db.LA_Modelos.Find(vehiculo.IdModeloLA).Nombre + ", Modelo  "+  db.LA_Anios.Find(vehiculo.IdAnioLA).Nombre +"  , No de serie " + vehiculo.NumeroDeSerie + ", con placas de circulaci&oacute;n " + vehiculo.Placas + ".</span></span></p>" +
            "<p style=\"text-align: justify;\"><span style=\"font-size: 18pt; font-family: arial, helvetica, sans-serif; color: rgb(0, 0, 0);\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dicho importe puede ser sujeto a cambios y restricciones en caso de que sea detectada alguna anomal&iacute;a en su documentaci&oacute;n o unidad.&nbsp;</span></p>" +
            "<p style=\"text-align: justify;\"><span style=\"font-size: 18pt; font-family: arial, helvetica, sans-serif; color: rgb(0, 0, 0);\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Sin m&aacute;s que agregar a la presente masiva, auto y varo le env&iacute;a un cordial saludo y felicitaci&oacute;n por su cr&eacute;dito aprobado.</span></p>" +
            "<p style=\"text-align: justify;\"><br><br><br></p>" +
            "<p style=\"text-align: justify;\">&nbsp;</p>" +
            "<p style=\"text-align: justify;\">&nbsp;</p>" +
            "<p style=\"text-align: center;\"><span style=\"font-size: 18pt; font-family: arial, helvetica, sans-serif; color: rgb(0, 0, 0);\">Atentamente</span></p>" +
            "<p style=\"text-align: center;\"><span style=\"font-size: 18pt; font-family: arial, helvetica, sans-serif; color: rgb(0, 0, 0);\">AUTO Y VARO SA DE CV</span></p>" +
            "<p style=\"text-align: center;\">&nbsp;</p>" +
            "</div>" +
            "</center>";
            //manejar el cuerpo como HTML
            mail.IsBodyHtml = true;
            //send the message         
            SmtpClient smtp = new SmtpClient("mail.autoyvaro.com", Convert.ToInt32(datos.Port));
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("autoyvar", "9Jns@6Gwx#a27");
            //smtp.Send(mail);

            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                            ex.ToString());
            }
        }

        public Precio GetPrecioDeCompra(String Clave)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;


            int MES = DateTime.Today.Month;
            int ANIO = DateTime.Today.Year;

            DateTime fechaInicio = new DateTime(ANIO, MES, 1);
            DateTime fechaFin = fechaInicio.AddMonths(1);

            var vercionHistorial = db.LA_HistotialConsulastas.Where(h => h.ClaveVercion == Clave && (h.FechaConsulta >= fechaInicio && h.FechaConsulta < fechaFin));

            if (vercionHistorial.Count() > 0)
            {

                Precio precioHistorico = new Precio
                {
                    Compra = vercionHistorial.FirstOrDefault().Compra.ToString(),
                    Venta = vercionHistorial.FirstOrDefault().Venta.ToString(),
                    Moneda = vercionHistorial.FirstOrDefault().Moneda
                };

                return precioHistorico;
            }
            else
            {
                var precio = wsLibrozul.ObtenerPrecioVersionPorClave(llave, 0, Clave, 0);

               

                return precio;
            }
        }


        public ActionResult Configurar(int IdSolicitud, int IdEsquema, decimal Monto) {
            SolicitudCotizacion solicitud = new SolicitudCotizacion();
            solicitud = db.SolicitudCotizacion.Find(IdSolicitud);
            Credito credito = ConstruirSimulacionPorEsquemeMonto(IdEsquema, solicitud, Monto);
            
            ViewBag.Nombre = solicitud.Nombre;
            ViewBag.ApellidoPaterno = solicitud.ApellidoPaterno;
            ViewBag.ApellidoMaterno = solicitud.ApellidoMaterno;
            ViewBag.Telefono = solicitud.Telefono;
            ViewBag.Email = solicitud.Email;

            ViewBag.Anio = solicitud.LA_Anios.Nombre;
            ViewBag.Marca = solicitud.LA_Marcas.Nombre;
            ViewBag.Modelo = solicitud.LA_Modelos.Nombre;
            ViewBag.Version = solicitud.LA_Version.Nombre;

            ViewBag.IdSolicitud = IdSolicitud;

            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion");
            ViewBag.IdEstadoPlaca = new SelectList(db.Estado, "id", "Descripcion");            
            ViewBag.IdEstadoCivil = new SelectList(db.Estado_civil, "id", "Descripcion");
            ViewBag.IdTipoTransmision = new SelectList(db.TipoTransmision, "id", "Descripcion");
            

            return View(credito);        
        }


        public Credito ConstruirSimulacionPorEsquemeMonto(int IdEsquema, SolicitudCotizacion solicitud, decimal Monto = 0)
        {
            Esquema esquema = new Esquema();
            esquema = db.Esquema.Find(IdEsquema);
            Credito credito = new Credito();

            var precioLibroAzul = Decimal.Parse(GetPrecioDeCompra(solicitud.ClaveVercion).Compra);

            credito.PorcentajeAvaluo = (solicitud.LA_Marcas.LA_ControlPorcentajeAvaluo.FirstOrDefault().PorcentajeAvaluo ?? 1) / 100;



            credito.PrecioDeCompra = precioLibroAzul;

            credito.EntregaLiquida = (precioLibroAzul * credito.PorcentajeAvaluo);

            
            

            var aperturaTemp = Utilities.MontoMasIVA(credito.EntregaLiquida * SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));
            credito.EntregaLiquida -= aperturaTemp;


            if (Monto <= credito.EntregaLiquida) {
                if (Monto != 0)
                {
                    credito.EntregaLiquida = Monto;
                }
            }


            credito.TipoDeCredito = db.TipoDeCredito.Where(e => e.id.Equals(1)).Single();

            credito.PorcentajeInteres = SettingsHelper.getPorcentajeIntereses(IdEsquema);

            credito.CargoGPSAdelantado = esquema.GastosAdministrativosPostEntregaLiquida;
            credito.IVA = SettingsHelper.getIVA();
            credito.AjustePorKilometraje = 0;

            var sumAdeudos = credito.AdeudoTenencia + credito.AdeudoOtros;

            decimal porcentajeDeApertura = Utilities.MontoMasIVA(SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));

            decimal reglaAvaluo = 1 - porcentajeDeApertura;
            var avaluoProcasa = (credito.EntregaLiquida + sumAdeudos + esquema.GastosAdministrativosPostEntregaLiquida) / reglaAvaluo;

            var aperturaCredito = Utilities.MontoMasIVA(avaluoProcasa * SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));

            credito.Esquema = esquema;
            credito.idEsquema = esquema.id;
            credito.Plazo = esquema.Dias;
            credito.AvaluoMercado = credito.AvaluoDeMercadoSuma;
            credito.AvaluoProcasa = avaluoProcasa;
            credito.AperturaCredito = aperturaCredito;
            credito.FechaCredito = DateTime.Today;

            return credito;
        }

        private void ActualizarHistorial(Precio precio, string Clave)
        {
            int IdentityUserId = 1;
            User user = db.User.Find(IdentityUserId);
            var paquete = db.LA_PaqueteConsultas.Where(p => p.Active == true).First();

            paquete.CunsultasUsadas = paquete.CunsultasUsadas + 1;
            db.Entry(paquete).State = EntityState.Modified;
            db.SaveChanges();


            var version = db.LA_Version.Find(Clave);
            if (version != null)
            {
                version.Compra = Decimal.Parse(precio.Compra);
                version.Venta = Decimal.Parse(precio.Venta);
                version.FechaActualizacion = DateTime.Today;
                db.Entry(version).State = EntityState.Modified;
                db.SaveChanges();

                LA_HistotialConsulastas historial = new LA_HistotialConsulastas
                {
                    IdUser = IdentityUserId,
                    IdSucursal = user.Sucursal,
                    FechaConsulta = DateTime.Now,
                    IdAnioLA = version.IdAnioLA,
                    IdMarcaLA = version.IdMarcaLA,
                    IdModeloLA = version.IdModeloLA,
                    ClaveVercion = Clave,
                    Compra = Decimal.Parse(precio.Compra),
                    Venta = Decimal.Parse(precio.Venta),
                    Moneda = precio.Moneda,
                    IdPaqueteConsulas = paquete.Id
                };
                db.LA_HistotialConsulastas.Add(historial);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            var input = new
                            {
                                Id = ve.PropertyName,
                                Message = ve.ErrorMessage,
                            };
                        }
                    }
                }


            }
        }






    }
}