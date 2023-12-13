using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;
using System.Data.Entity;

namespace AutoyVaro.Controllers
{
    public class SolicitudController : Controller
    {
        private Entities db = new Entities();
        // GET: Solicitud

        public ActionResult Index(int IdSolicitud)
        {           

            var solicitud = db.Fin_Solicitud.Find(IdSolicitud);

            if (solicitud.Fin_DocumentacionSolicitud.Count == 0) {

                int[] argDoc = new int[] { 1, 5, 7, 8, 4, 6, 9,10 };
                foreach (int item in argDoc)
                {
                    Fin_DocumentacionSolicitud doc = new Fin_DocumentacionSolicitud();
                    doc.IdSolicitud = solicitud.Id;
                    doc.IdDocumentacionRequerida = item;
                    db.Fin_DocumentacionSolicitud.Add(doc);
                    db.SaveChanges();
                }
            }

            solicitud = db.Fin_Solicitud.Find(IdSolicitud);

            ViewModelSolicitudCredito model = new ViewModelSolicitudCredito();            
            Credito credito = ConstruirSimulacionPorEsquemeMonto(solicitud.IdEsquema, solicitud, solicitud.MontoSolicitado??0);

            ViewBag.Nombre = solicitud.Fit_Cliente.Nombre;
            ViewBag.ApellidoPaterno = solicitud.Fit_Cliente.ApellidoPaterno;
            ViewBag.ApellidoMaterno = solicitud.Fit_Cliente.ApellidoMaterno;
            ViewBag.Telefono = solicitud.Fit_Cliente.Celular;
            ViewBag.Email = solicitud.Fit_Cliente.CorreoElectronico;
            ViewBag.Anio = solicitud.Fit_Vehiculo.LA_Anios.Nombre;
            ViewBag.Marca = solicitud.Fit_Vehiculo.LA_Marcas.Nombre;
            ViewBag.Modelo = solicitud.Fit_Vehiculo.LA_Modelos.Nombre;
            ViewBag.Version = solicitud.Fit_Vehiculo.LA_Version.Nombre;
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion");

            model.creditos = credito;
            model.solicitud = solicitud;    

            return View(model); 
        }

        
        public Credito ConstruirSimulacionPorEsquemeMonto(int IdEsquema, Fin_Solicitud solicitud, decimal Monto = 0)
        {
            Esquema esquema = new Esquema();
            esquema = db.Esquema.Find(IdEsquema);
            Credito credito = new Credito();
            decimal precioLibroAzul = solicitud.Compra ?? 0;
            credito.PorcentajeAvaluo = (solicitud.Fit_Vehiculo.LA_Marcas.LA_ControlPorcentajeAvaluo.FirstOrDefault().PorcentajeAvaluo ?? 1) / 100;
            credito.PrecioDeCompra = precioLibroAzul;
            credito.EntregaLiquida = (precioLibroAzul * credito.PorcentajeAvaluo);
            var aperturaTemp = Utilities.MontoMasIVA(credito.EntregaLiquida * SettingsHelper.getPorcentajeApertura(IdEsquema, credito.ClientePreferente ?? false));
            credito.EntregaLiquida -= aperturaTemp;

            if (Monto <= credito.EntregaLiquida)
            {
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


        public ActionResult Solicitudes() {
            string IdentityUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(IdentityUserId);
            return View(user.Fit_Cliente.FirstOrDefault().Fin_Solicitud);

        }


                
        public ActionResult AddFile(int id) {
            var solicitud = db.Fin_Solicitud.Find(id);

            if (solicitud.Fin_DocumentacionSolicitud.Count == 0)
            {

                int[] argDoc = new int[] { 1, 5, 7, 8, 4, 6, 9, 10 };
                foreach (int item in argDoc)
                {
                    Fin_DocumentacionSolicitud doc = new Fin_DocumentacionSolicitud();
                    doc.IdSolicitud = solicitud.Id;
                    doc.IdDocumentacionRequerida = item;
                    db.Fin_DocumentacionSolicitud.Add(doc);
                    db.SaveChanges();
                }
            }

            solicitud = db.Fin_Solicitud.Find(id);

            ViewModelSolicitudCredito model = new ViewModelSolicitudCredito();
            Credito credito = ConstruirSimulacionPorEsquemeMonto(solicitud.IdEsquema, solicitud, solicitud.MontoSolicitado ?? 0);

            ViewBag.Nombre = solicitud.Fit_Cliente.Nombre;
            ViewBag.ApellidoPaterno = solicitud.Fit_Cliente.ApellidoPaterno;
            ViewBag.ApellidoMaterno = solicitud.Fit_Cliente.ApellidoMaterno;
            ViewBag.Telefono = solicitud.Fit_Cliente.Celular;
            ViewBag.Email = solicitud.Fit_Cliente.CorreoElectronico;
            ViewBag.Anio = solicitud.Fit_Vehiculo.LA_Anios.Nombre;
            ViewBag.Marca = solicitud.Fit_Vehiculo.LA_Marcas.Nombre;
            ViewBag.Modelo = solicitud.Fit_Vehiculo.LA_Modelos.Nombre;
            ViewBag.Version = solicitud.Fit_Vehiculo.LA_Version.Nombre;
            ViewBag.IdSolicitud = id;
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion");

            model.creditos = credito;
            model.solicitud = solicitud;

            return View(model);
        }


        public ActionResult EnviarMailDocumentos(int id) {
            enviarMailDocs(id);

            Task.WaitAll(Task.Delay(3000));

            return RedirectToAction("Index","Cotizador");            
        }

        public void enviarMail() {
            MailMessage mail = new MailMessage();
            var datos = db.Settings.FirstOrDefault();
            mail.From = new MailAddress(datos.CorreoSalida);
            mail.To.Add(datos.CorreoDestino);
            mail.To.Add(datos.CorreoDestino);
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.Subject = "Nueva dispersión para pagar";
            mail.Body = "<center>" +
            "<div style=\"max-width: 700px;\" align=\"center\">" +
            "<p><em><img style=\"float: right;\" src=\"https://localhost:44332/Content/assets3/img/LOGO-5.png\" alt=\"logo\" width=\"160\" height=\"72\"> </em></p>" +
            "<p>&nbsp;</p>" +
            "<p>&nbsp;</p>" +
            "<p>&nbsp;</p>" +
            "<p><em>Apreciable Sra: Martha Sahag&uacute;n de Fox</em></p>" +
            "<p>&nbsp;</p>" +
            "<p style=\"text-align: justify;\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; Es para nosotros un honor informarle a trav&eacute;s de la presente que hemos recibido su solicitud de cr&eacute;dito, mismo que ha sido autorizado por la cantidad de $666,000.00 (Seiscientos sesenta y seis mil pesos 00/100 mn), con un plazo a 24 meses y con una tasa de 7.5% mensual, sobre la unidad en garant&iacute;a: MITSUBISHI L-200, Modelo 2020, No de serie MMBNL4568LH007683, con placas de circulaci&oacute;n XU6112A.</p>" +
            "<p style=\"text-align: justify;\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dicho importe puede ser sujeto a cambios y restricciones en caso de que sea detectada alguna anomal&iacute;a en su documentaci&oacute;n o unidad.&nbsp;</p>" +
            "<p style=\"text-align: justify;\">&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Sin m&aacute;s que agregar a la presente masiva, auto y varo le env&iacute;a un cordial saludo y felicitaci&oacute;n por su cr&eacute;dito aprobado.</p>" +
            "<p style=\"text-align: justify;\"><br><br><br></p>" +
            "<p style=\"text-align: center;\">Atentamente</p>" +
            "<p style=\"text-align: center;\">AUTO Y VARO SA DE CV</p>" +
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

            db.SaveChanges();            

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


        public PartialViewResult EventosSolicitud(int id) {
            Fin_Solicitud solicitud = db.Fin_Solicitud.Find(id);
            return PartialView("_EventoSolicitud", solicitud);
        }


        public JsonResult IniciarValidacion(int id) {
            Fin_Solicitud solicitud = db.Fin_Solicitud.Find(id);
            if (solicitud == null)
            {

                var result = new
                {
                    Error = true,
                    Mensaje = "No se encontro la solicitud buscada"
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else {

                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        var result = new
                        {
                            Error = true,
                            Mensaje = "Tu documentación se envió con éxito"
                        };
                        solicitud.IdEstadoSolicitud = 1;
                        db.Entry(solicitud).State = EntityState.Modified;

                        ObservacionSolicitud observacionSolicitud = new ObservacionSolicitud();
                        observacionSolicitud.FechaObservacion = DateTime.Now;
                        observacionSolicitud.Observacion = "El cliente envió la documentación inicial para validar su crédito.";
                        observacionSolicitud.IdUsuario = 1;
                        observacionSolicitud.Publicar = true;
                        observacionSolicitud.IdSolicitud = id;
                        db.ObservacionSolicitud.Add(observacionSolicitud);
                        db.SaveChanges();                        

                        transaction.Complete();
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }

                    catch (Exception ex)
                    {
                        var resultError = new
                        {
                            Error = true,
                            Mensaje = "Error al procesar su solicitud inténtelo más tarde."
                        };
                        return Json(resultError, JsonRequestBehavior.AllowGet);
                    }
                }
                
            }

        }

        public void enviarMailDocs(int id)
        {

            Fin_Solicitud solicitud= db.Fin_Solicitud.Find(id);

            MailMessage mail = new MailMessage();
            var datos = db.Settings.FirstOrDefault();
            mail.From = new MailAddress("gerencia@autoyvaro.com", "Auto y Varo SA de CV");
            var mailCliente = solicitud.Fit_Cliente.CorreoElectronico; 


            mail.To.Add(mailCliente);
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.Subject = "Envío de documentos";
            mail.Body = "<div class=\"adM\" style=\"height: auto; background: #030734; width: 70%; margin: 40px auto;\">" +
"    <br>" +
"    <table style=\"border-collapse: collapse; width: 100.05%; height: 86px;\" border=\"0\">" +
"        <colgroup><col style=\"width: 30.1773%;\"><col style=\"width: 38.2006%;\"><col style=\"width: 31.6246%;\"></colgroup>" +
"        <tbody>" +
"            <tr style=\"height: 86px;\">" +
"                <td style=\"height: 86px;\"><img src=\"https://autoyvaro.mx/Content/IMG/Logo.png\" width=\"175\" height=\"83\"></td>" +
"                <td style=\"height: 86px;\">" +
"                    <p><strong><span style=\"font-size: 18pt; color: rgb(236, 240, 241);\">TE PRESTAMOS M&Aacute;S</span></strong></p>" +
"                    <p style=\"text-align: right;\"><span style=\"color: rgb(230, 126, 35);\"><span style=\"font-size: 18pt;\"><a style=\"color: rgb(230, 126, 35);\" href=\"https://autoyvaro.mx/\">autoyvaro.mx</a></span></span></p>" +
"                </td>" +
"                <td style=\"height: 86px;\"><img style=\"float: right;\" src=\"https://autoyvaro.mx/Content/IMG/rodada.png\" width=\"181\" height=\"86\"></td>" +
"            </tr>" +
"        </tbody>" +
"    </table>" +
"</div>" +
"<div style=\"margin: auto;\">" +
"    <p style=\"color: #516766; font-size: 24px; font-weight: bold; padding: 0 15%; margin-top: 35px;\"><span style=\"font-size: 24pt;\">Env&iacute;anos tus documentos</span></p>" +
"    <div style=\"padding: 0 15%; margin-top: 40px;\">" +
"        <p style=\"font-weight: 400; font-size: 20px; line-height: 28px; text-align: justify;\"><span style=\"color: rgb(0, 0, 0);\">&iexcl;Hola "+ solicitud.Fit_Cliente.Nombre + "!, Para continuar con la autorizaci&oacute;n de tu cr&eacute;dito necesitamos que nos compartas los siguientes documentos:</span></p>" +
"    </div>" +
"    <div style=\"padding: 0px 15%; margin-top: 40px; text-align: center;\">" +
"        <table style=\"border-collapse: collapse; width: 100.045%; height: 54px;\" border=\"0\">" +
"            <colgroup><col style=\"width: 100%;\"></colgroup>" +
"            <tbody>" +
"                <tr style=\"height: 18px;\">" +
"                    <td style=\"height: 18px; text-align: center;\">" +
"                        <h3 class=\"mb-0 text-truncated\" style=\"box-sizing: border-box; margin-top: 0px; font-family: Lora, serif; line-height: 1.5; color: rgb(0, 0, 0); font-size: 1.75rem; text-shadow: rgba(0, 0, 0, 0.15) 0px 0px 1px; background-color: rgb(255, 255, 255); margin-bottom: 0px;\">Factura o refactura</h3>" +
"                        <p style=\"box-sizing: border-box; margin-top: 0px; margin-bottom: 1rem; font-size: 16px; line-height: 1.7; color: rgb(102, 102, 102); font-family: Barlow, sans-serif; background-color: rgb(255, 255, 255);\">Factura original y/ o refactura.</p>" +
"                    </td>" +
"                </tr>" +
"                <tr style=\"height: 18px;\">" +
"                    <td style=\"height: 18px; text-align: center;\">" +
"                        <h3 class=\"mb-0 text-truncated\" style=\"box-sizing: border-box; margin-top: 0px; font-family: Lora, serif; line-height: 1.5; color: rgb(0, 0, 0); font-size: 1.75rem; text-shadow: rgba(0, 0, 0, 0.15) 0px 0px 1px; background-color: rgb(255, 255, 255); margin-bottom: 0px;\">Tarjeta de Circulaci&oacute;n</h3>" +
"                        <p style=\"box-sizing: border-box; margin-top: 0px; margin-bottom: 1rem; font-size: 16px; line-height: 1.7; color: rgb(102, 102, 102); font-family: Barlow, sans-serif; background-color: rgb(255, 255, 255);\">Tarjeta de circulaci&oacute;n a nombre del cliente.&nbsp;</p>" +
"                    </td>" +
"                </tr>" +
"                <tr style=\"height: 18px;\">" +
"                    <td style=\"height: 18px; text-align: center;\">" +
"                        <h3 class=\"mb-0 text-truncated\" style=\"box-sizing: border-box; margin-top: 0px; font-family: Lora, serif; line-height: 1.5; color: rgb(0, 0, 0); font-size: 1.75rem; text-shadow: rgba(0, 0, 0, 0.15) 0px 0px 1px; background-color: rgb(255, 255, 255); margin-bottom: 0px;\">Comprobante de Domicilio</h3>" +
"                        <p style=\"box-sizing: border-box; margin-top: 0px; margin-bottom: 1rem; font-size: 16px; line-height: 1.7; color: rgb(102, 102, 102); font-family: Barlow, sans-serif; background-color: rgb(255, 255, 255);\">Comprobante de domicilio con antig&uuml;edad no mayor a 3 meses.</p>" +
"                    </td>" +
"                </tr>" +
"                <tr>" +
"                    <td style=\"text-align: center;\">" +
"                        <h3 class=\"mb-0 text-truncated\" style=\"box-sizing: border-box; margin-top: 0px; font-family: Lora, serif; line-height: 1.5; color: rgb(0, 0, 0); font-size: 1.75rem; text-shadow: rgba(0, 0, 0, 0.15) 0px 0px 1px; background-color: rgb(255, 255, 255); margin-bottom: 0px;\">Identificacion Oficial</h3>" +
"                        <p style=\"box-sizing: border-box; margin-top: 0px; margin-bottom: 1rem; font-size: 16px; line-height: 1.7; color: rgb(102, 102, 102); font-family: Barlow, sans-serif; background-color: rgb(255, 255, 255);\">Identificaci&oacute;n oficial INE o Pasaporte (vigente).</p>" +
"                    </td>" +
"                </tr>" +
"                <tr>" +
"                    <td style=\"text-align: center;\">" +
"                        <h3 class=\"mb-0 text-truncated\" style=\"box-sizing: border-box; margin-top: 0px; font-family: Lora, serif; line-height: 1.5; color: rgb(0, 0, 0); font-size: 1.75rem; text-shadow: rgba(0, 0, 0, 0.15) 0px 0px 1px; background-color: rgb(255, 255, 255); margin-bottom: 0px;\">Tenencias Pagadas</h3>" +
"                        <p style=\"box-sizing: border-box; margin-top: 0px; margin-bottom: 1rem; font-size: 16px; line-height: 1.7; color: rgb(102, 102, 102); font-family: Barlow, sans-serif; background-color: rgb(255, 255, 255);\">Tenencias pagadas (última),si tiene adeudo se puede pagar del mismo crédito, como pago a terceros o demeritar.</p>" +
"                    </td>" +
"                </tr>" +
"            </tbody>" +
"        </table>" +
"        <p style=\"text-align: justify;\">&nbsp;</p>" +
"        <p style=\"text-align: justify;\">Una vez que nos compartas tus archivos el equipo de validaci&oacute;n podr&aacute; agilizar el tr&aacute;mite de tu pr&eacute;stamo.</p>" +
"        <p>&nbsp;</p>" +
"        <p style=\"text-align: center;\">Da click en el siguiente bot&oacute;n para subir su documentaci&oacute;n:</p>" +
"        <a style=\"font-weight: 400; font-size: 16px; background-color: #ee7e3c; color: #1f2d27; padding: 2px 12px; border-radius: 8px; text-decoration: none; text-align: center; height: 48px; display: inline-block; width: 270px;\" href=\"https://autoyvaro.mx/Solicitud/AddFile/" + id+"\" target=\"_blank\" rel=\"noopener\" aria-invalid=\"true\"><strong style=\"line-height: 50px;\"><span style=\"color: rgb(236, 240, 241);\">Subir</span><span style=\"color: rgb(236, 240, 241);\"> documentos</span></strong></a>" +
"        <p style=\"text-align: center;\">&nbsp;</p>" +
"        <p style=\"text-align: center;\">&nbsp;</p>" +
"    </div>" +
"    <div style=\"height: auto; background: #030734; width: 70%; margin: 40px auto;\">" +
"        <table style=\"border-collapse: collapse; width: 100.05%; height: 160px;\" border=\"0\">" +
"            <colgroup><col style=\"width: 49.921%;\"></colgroup>" +
"            <tbody>" +
"                <tr style=\"height: 102px;\">" +
"                    <td style=\"height: 102px;\"><img style=\"display: block; margin-left: auto; margin-right: auto;\" src=\"https://autoyvaro.mx/Content/IMG/logo-white.png\" width=\"204\" height=\"102\"></td>" +
"                </tr>" +
"                <tr style=\"height: 40px;\">" +
"                    <td style=\"height: 40px;\">" +
"                        <div style=\"padding: 0 15%; margin-top: 10px;\">" +
"                            <p style=\"font-weight: 300; font-size: 12px; line-height: 16px; color: #596b64; text-align: center;\"><span style=\"color: rgb(236, 240, 241);\">Consulta nuestros</span> <a href=\"https://autoyvaro.mx/Home/TerminosyCondiciones\"><span style=\"color: rgb(230, 126, 35);\">T&eacute;rminos</span><span style=\"color: rgb(230, 126, 35);\"> y Condiciones</span></a>&nbsp;<span style=\"color: rgb(236, 240, 241);\">y nuestro</span> <a href=\"https://autoyvaro.mx/Home/AvisoPrivacidad\"><span style=\"color: rgb(230, 126, 35);\">Aviso</span><span style=\"color: rgb(230, 126, 35);\"> de Privacidad</span></a></p>" +
"                        </div>" +
"                    </td>" +
"                </tr>" +
"                <tr style=\"height: 18px;\">" +
"                    <td style=\"height: 18px;\">" +
"                        <div style=\"padding: 0 15%; margin: 10px 0; display: flex;\">" +
"                            <div style=\"display: flex; margin: auto;\">&nbsp;</div>" +
"                        </div>" +
"                    </td>" +
"                </tr>" +
"            </tbody>" +
"        </table>" +
"    </div>" +
"</div>";
            //manejar el cuerpo como HTML
            mail.IsBodyHtml = true;
            //send the message         
            SmtpClient smtp = new SmtpClient("mail.autoyvaro.com", Convert.ToInt32(datos.Port));
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("autoyvar", "9Jns@6Gwx#a27");
            //smtp.Send(mail);

            db.SaveChanges();

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


    }


    

}