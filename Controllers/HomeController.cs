using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using AutoyVaro.com.libroazul.www;
using AutoyVaro.Models;
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Net;
using System.Net.Mail;
//using AutoyVaro.AutoCredito.com.libroazul.www;
namespace AutoyVaro.Controllers
{    
    public class HomeController : Controller
    {
        private Entities db = new Entities();
        private String llave = "";

        public ActionResult Index()
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "da123123123");
            Session["llaveTemp"] = llave;

            var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);
            ViewBag.IdAnio = new SelectList(listaAnios, "Clave", "Nombre");

            var anios = listaAnios;
            ViewBag.Anios = anios;

            ViewBag.Marcas = db.LA_Marcas.Where(w => w.Active).Select(s => new MarcasModel
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Logo = s.Logo
            }).ToList();


            Thread ThreadActualizarMarcas =
             new Thread(
               unused => ActualizarAnios(listaAnios)
             );
            ThreadActualizarMarcas.Start();

            return View();
        }

        public ActionResult Inicio()
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "daqwerqwer1");
            Session["llaveTemp"] = llave;

            var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);
            ViewBag.IdAnio = new SelectList(listaAnios, "Clave", "Nombre");

            var anios = listaAnios;
            ViewBag.Anios = anios;

            ViewBag.Marcas = db.LA_Marcas.Where(w => w.Active).Select(s => new MarcasModel
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Logo = s.Logo
            }).ToList();


            Thread ThreadActualizarMarcas =
             new Thread(
               unused => ActualizarAnios(listaAnios)
             );
            ThreadActualizarMarcas.Start();

            return View();
        }


        private void ActualizarAnios(Catalogo[] listaAnios)
        {
            try
            {
                foreach (Catalogo anios in listaAnios)
                {
                    int claveAnio = int.Parse(anios.Clave);
                    if (db.LA_Anios.Find(claveAnio) == null)
                    {
                        LA_Anios anio = new LA_Anios();
                        anio.Active = true;
                        anio.Id = int.Parse(anios.Clave);
                        anio.Nombre = anios.Nombre;
                        db.LA_Anios.Add(anio);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Elements()
        {
            return View();
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }




        public ActionResult Simulacion(String Clave, String Nombre, decimal Compra, string Moneda, decimal Venta)
        {
            ViewBag.MontoMinimo = 30000;
            ViewBag.MontoMaximo = Compra * (decimal)0.75;
            ViewBag.Clave = Clave;
            ViewBag.Nombre = Nombre;
            ViewBag.Compra = Compra;
            ViewBag.Moneda = Moneda;
            ViewBag.Venta = Venta;
            return View();
        }




        public JsonResult GetMarcaSelect(int IdAnio)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaMarcas = wsLibrozul.ObtenerMarcasPorAnio(llave, 0, IdAnio, 0);

            Thread ThreadActualizarMarcas =
              new Thread(
                unused => ActualizarMarcas(listaMarcas, IdAnio)
              );
            ThreadActualizarMarcas.Start();

            return Json(listaMarcas, JsonRequestBehavior.AllowGet);
        }

        private void ActualizarMarcas(Catalogo[] listaMarcas, int IdAnio)
        {
            using (Entities dbContext = new Entities()) {
                List<LA_MarcasPorAnio> listaLA_MarcasPorAnio = new List<LA_MarcasPorAnio>();
                foreach (Catalogo marcas in listaMarcas)
                {
                    int claveMarca = int.Parse(marcas.Clave);

                    var marca = dbContext.LA_Marcas.Find(claveMarca);

                    if (marca == null)
                    {
                        LA_Marcas mar = new LA_Marcas();
                        mar.Active = true;
                        mar.Id = int.Parse(marcas.Clave);
                        mar.Nombre = marcas.Nombre;
                        dbContext.LA_Marcas.Add(mar);
                        dbContext.SaveChanges();

                        LA_MarcasPorAnio marcasPorAnio = new LA_MarcasPorAnio
                        {
                            IdAnioLA = IdAnio,
                            IdMarcaLA = mar.Id
                        };
                        listaLA_MarcasPorAnio.Add(marcasPorAnio);

                    }
                }
                if (listaLA_MarcasPorAnio.Count() > 0)
                {
                    dbContext.LA_MarcasPorAnio.AddRange(listaLA_MarcasPorAnio);
                    dbContext.SaveChanges();
                }
            }         
        }

        public JsonResult GetModeloSelect(int IdAnio, int IdMarca)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaModelos = wsLibrozul.ObtenerModelosPorAnioMarca(llave, 0, IdAnio, IdMarca, 0);

            Thread ThreadActualizarModelos =
              new Thread(
                unused => ActualizarModelos(listaModelos, IdAnio, IdMarca)
              );
            ThreadActualizarModelos.Start();

            return Json(listaModelos, JsonRequestBehavior.AllowGet);
        }

        private void ActualizarModelos(Catalogo[] listaModelos, int IdAnio, int IdMarca)
        {
            List<LA_ModelosXMarcaAnio> listaLA_ModelosXMarcaAnio = new List<LA_ModelosXMarcaAnio>();
            foreach (Catalogo modelos in listaModelos)
            {
                int claveModelo = int.Parse(modelos.Clave);
                if (db.LA_Modelos.Find(claveModelo) == null)
                {
                    LA_Modelos mod = new LA_Modelos();
                    mod.Active = true;
                    mod.Id = int.Parse(modelos.Clave);
                    mod.Nombre = modelos.Nombre;
                    mod.IdMarcasLA = IdMarca;
                    db.LA_Modelos.Add(mod);
                    db.SaveChanges();

                    LA_ModelosXMarcaAnio modelosXMarcaAnio = new LA_ModelosXMarcaAnio
                    {
                        IdAnioLA = IdAnio,
                        IdMarcaLA = IdMarca,
                        IdModeloLA = mod.Id
                    };
                    listaLA_ModelosXMarcaAnio.Add(modelosXMarcaAnio);

                }
            }
            if (listaLA_ModelosXMarcaAnio.Count() > 0)
            {
                db.LA_ModelosXMarcaAnio.AddRange(listaLA_ModelosXMarcaAnio);
                db.SaveChangesAsync();
            }
        }

        public JsonResult GetVersionSelect(int IdAnio, int IdMarca, int IdModelo)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaVersiones = wsLibrozul.ObtenerVersionesPorAnioMarcaModelo(llave, 0, IdAnio, IdMarca, IdModelo, 0);

            Thread ThreadActualizarMarcas =
            new Thread(
              unused => ActualizarVersion(listaVersiones, IdAnio, IdMarca, IdModelo)
            );
            ThreadActualizarMarcas.Start();

            return Json(listaVersiones, JsonRequestBehavior.AllowGet);
        }

        private void ActualizarVersion(Catalogo[] listaVersion, int IdAnio, int IdMarca, int IdModelo)
        {
            foreach (Catalogo versiones in listaVersion)
            {
                if (db.LA_Version.Where(a => a.Id == versiones.Clave).Count() == 0)
                {
                    LA_Version ver = new LA_Version();
                    ver.Active = true;
                    ver.Id = versiones.Clave;
                    ver.Nombre = versiones.Nombre;
                    ver.IdAnioLA = IdAnio;
                    ver.IdMarcaLA = IdMarca;
                    ver.IdModeloLA = IdModelo;
                    db.LA_Version.Add(ver);
                    db.SaveChanges();
                }
            }
        }

        public JsonResult GetPrecioDeCompra(String Clave)
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

                return Json(precioHistorico, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var precio = wsLibrozul.ObtenerPrecioVersionPorClave(llave, 0, Clave, 0);

                Thread ThreadActualizarVersion =
                new Thread(
                  unused => ActualizarHistorial(precio, Clave)
                );
                ThreadActualizarVersion.Start();

                return Json(precio, JsonRequestBehavior.AllowGet);
            }
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

        
        public PartialViewResult MarcarPorAnio(int IdAnio)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaMarcas = wsLibrozul.ObtenerMarcasPorAnio(llave, 0, IdAnio, 0);

            Thread ThreadActualizarMarcas =
              new Thread(
                unused => ActualizarMarcas(listaMarcas, IdAnio)
              );
            ThreadActualizarMarcas.Start();

            var marcas = listaMarcas.Select(s => new MarcasModel
            {
                Id = int.Parse(s.Clave),
                Nombre = s.Nombre,
                Logo = db.LA_Marcas.Find(int.Parse(s.Clave)).Logo
            });

            return PartialView("_Marcas", marcas.ToList());


        }


        public PartialViewResult ModelosPorAnioMarca(int IdAnio, int IdMarca)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaModelos = wsLibrozul.ObtenerModelosPorAnioMarca(llave, 0, IdAnio, IdMarca, 0);


            Thread ThreadActualizarModelos =
              new Thread(
                unused => ActualizarModelos(listaModelos, IdAnio, IdMarca)
              );
            ThreadActualizarModelos.Start();

            return PartialView("_Modelos", listaModelos);
        }

        public PartialViewResult VersionPorAnioMarcaModelo(int IdAnio, int IdMarca, int IdModelo)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = Session["llaveTemp"] as String;
            var listaVersion = wsLibrozul.ObtenerVersionesPorAnioMarcaModelo(llave, 0, IdAnio, IdMarca, IdModelo, 0);

            Thread ThreadActualizarVersion =
            new Thread(
              unused => ActualizarVersion(listaVersion, IdAnio, IdMarca, IdModelo)
            );
            ThreadActualizarVersion.Start();

            return PartialView("_Versiones", listaVersion);
        }


        public MessageResource SendSMS(String Numero, String Codigo)
        {
            try
            {
                // Find your Account SID and Auth Token at twilio.com/console
                // and set the environment variables. See http://twil.io/secure
                string accountSid = Environment.GetEnvironmentVariable("AC84d15d39e98889c99104ada9faeff7be");
                string authToken = Environment.GetEnvironmentVariable("3f2db2ac74f908c25aeb95943fa6da6e");

                TwilioClient.Init("AC84d15d39e98889c99104ada9faeff7be", "3f2db2ac74f908c25aeb95943fa6da6e");

                var message = MessageResource.Create(
                    body: "Tu código de seguridad es: " + Codigo,
                    from: new Twilio.Types.PhoneNumber("+17404956758"),
                    to: new Twilio.Types.PhoneNumber("+52" + Numero)
                );

                return message;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }

        }


        public PartialViewResult Solicitud(int IdAnio, int IdMarca, int IdModelo, String IdVersion)
        {
            SolicitudCotizacion model = new SolicitudCotizacion();
            model.IdAnioLA = IdAnio;
            model.IdMarcaLA = IdMarca;
            model.IdModeloLA = IdModelo;
            model.ClaveVercion = IdVersion;
            model.FechaSolicitud = DateTime.Now;
            return PartialView("_Solicitud", model);
        }


        // POST: ListaNegras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateSolicitud([Bind(Include = "Id,FechaSolicitud,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NombreCopleto,Telefono,Email")] SolicitudCotizacion solicitudCotizacion)
        {
            
            MapValidacion result = new MapValidacion();
            List<ValidacionCampo> listaErrores = new List<ValidacionCampo>();
            listaErrores = getValidacionModelCreate(solicitudCotizacion);

            if (ModelState.IsValid && listaErrores.Count == 0)
            {

                var seed = Environment.TickCount;
                var random = new Random(seed);
                var codigo = random.Next(1000, 9999);

                SendSMS(solicitudCotizacion.Telefono, codigo.ToString());

                solicitudCotizacion.CodigoValidacionNumero = codigo.ToString();
                solicitudCotizacion.CodigoValidado = false;
                solicitudCotizacion.EstadoSolicitudCotizacionId = 1;
                solicitudCotizacion.Compra = 0;
                solicitudCotizacion.Venta = 0;
                db.SolicitudCotizacion.Add(solicitudCotizacion);
                db.SaveChanges();

                solicitudCotizacion.CodigoValidacionNumero = "";
                return PartialView("_ConfirmarNumero", solicitudCotizacion);
            }
            else
            {
                ValidacionCampo validator = new ValidacionCampo(ModelState);

                listaErrores = validator.getListaErrores(listaErrores);
                result.Error = true;
                result.Mensaje = "Se encontraron algunos errores, \n Favor de resolverlos para continuar con el proceso.";
                result.data = listaErrores;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //Valida el Create de una instalación
        
        public List<ValidacionCampo> getValidacionModelCreate(SolicitudCotizacion model)
        {
            using (Entities db = new Entities())
            {
                List<ValidacionCampo> errores = new List<ValidacionCampo>();

                if (!String.IsNullOrEmpty(model.Telefono))
                {
                    if (model.Telefono.Length < 10)
                    {
                        ValidacionCampo inputError = new ValidacionCampo();
                        inputError.Id = "Telefono";
                        inputError.Mensaje = "Tu número de celular no es válido.";
                        errores.Add(inputError);
                    }
                }

                return errores;
            }
        }



        [HttpPost]
        public JsonResult ValidarSolicitud([Bind(Include = "Id,FechaSolicitud,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NombreCopleto,Telefono,Email,CodigoValidacionNumero,CodigoValidado,EstadoSolicitudCotizacionId")] SolicitudCotizacion solicitudCotizacion)
        {
            
            MapValidacion result = new MapValidacion();
            List<ValidacionCampo> listaErrores = new List<ValidacionCampo>();
            listaErrores = getValidacionModelEdit(solicitudCotizacion);

            if (ModelState.IsValid && listaErrores.Count == 0)
            {
                var wsLibrozul = new wslibroazulSoapClient();
                llave = Session["llaveTemp"] as String;



                var precio = new Precio();


                int MES = DateTime.Today.Month;
                int ANIO = DateTime.Today.Year;

                DateTime fechaInicio = new DateTime(ANIO, MES, 1);
                DateTime fechaFin = fechaInicio.AddMonths(1);

                var vercionHistorial = db.LA_HistotialConsulastas.Where(h => h.ClaveVercion == solicitudCotizacion.ClaveVercion && (h.FechaConsulta >= fechaInicio && h.FechaConsulta < fechaFin));

                
                if (vercionHistorial.Count() > 0)
                {
                    precio = new Precio
                    {
                        Compra = vercionHistorial.FirstOrDefault().Compra.ToString(),
                        Venta = vercionHistorial.FirstOrDefault().Venta.ToString(),
                        Moneda = vercionHistorial.FirstOrDefault().Moneda
                    };
                }
                else
                {
                    precio = wsLibrozul.ObtenerPrecioVersionPorClave(llave, 0, solicitudCotizacion.ClaveVercion, 0);

                    Thread ThreadActualizarVersion =
                    new Thread(
                      unused => ActualizarHistorial(precio, solicitudCotizacion.ClaveVercion)
                    );
                    ThreadActualizarVersion.Start();

                    return Json(precio, JsonRequestBehavior.AllowGet);
                }

                
                solicitudCotizacion.CodigoValidado = true;
                solicitudCotizacion.EstadoSolicitudCotizacionId = 2;
                solicitudCotizacion.Compra = Decimal.Parse(precio.Compra);
                solicitudCotizacion.Venta = Decimal.Parse(precio.Venta);
                db.Entry(solicitudCotizacion).State = EntityState.Modified;                
                db.SaveChanges();
                result.Error = false;
                result.Mensaje = "El registro se realizó con éxito.";
                result.data = null;
                return Json(result);
            }
            else
            {
                ValidacionCampo validator = new ValidacionCampo(ModelState);
                listaErrores = validator.getListaErrores(listaErrores);
                result.Error = true;
                result.Mensaje = "No pudimos validar sus datos.";
                result.data = listaErrores;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<ValidacionCampo> getValidacionModelEdit(SolicitudCotizacion model)
        {
            using (Entities db = new Entities())
            {
                List<ValidacionCampo> errores = new List<ValidacionCampo>();

                var solicitud = db.SolicitudCotizacion.Find(model.Id);

                if (solicitud.CodigoValidacionNumero != model.CodigoValidacionNumero) {
                    ValidacionCampo inputError = new ValidacionCampo();
                    inputError.Id = "CodigoValidacionNumero";
                    inputError.Mensaje = "Tu código no coincide, puedes volver a intentarlo.";
                    errores.Add(inputError);
                }

                return errores;
            }
        }

        public PartialViewResult SucursalesList() {
            List<Sucursal> listSucursales = db.Sucursal.Where(a => a.Activo == true && a.Id > 1).ToList();
            return PartialView("_ListSucursales", listSucursales);
        }

        public PartialViewResult SucursalesListFooter()
        {
            List<Sucursal> listSucursales = db.Sucursal.Where(a => a.Activo == true && a.Id > 1).ToList();
            return PartialView("_ListSucursalesFooter", listSucursales);
        }
        public ActionResult Sucursal(int Id) {
            var sucursal = db.Sucursal.Find(Id);
            return View(sucursal);
        }
        public ActionResult AvisoPrivacidad() { 
            return View();
        }

        public ActionResult Contacto() {            
            return View();
        }


        public PartialViewResult ContactoPartial() {            

            var wsLibrozul = new wslibroazulSoapClient();
            try
            {
                llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "daqwerqwe41");
                Session["llaveTemp"] = llave;
                var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);
                ViewBag.IdAnioLA = new SelectList(listaAnios, "Clave", "Nombre");
                
            }
            catch (Exception e)
            {
                int anio = DateTime.Now.Year - 10;
                ViewBag.IdAnioLA = new SelectList(db.LA_Anios.Where(w => w.Id >= anio).Distinct().OrderByDescending(a => a.Id), "Id", "Nombre");
                
            }            

            ContactoWeb contacto = new ContactoWeb();
            contacto.FechaSolicitud = DateTime.Now;
            ViewBag.IdUsuarioSolicitud = new SelectList(db.AspNetUsers.Where(a=> a.Id == ""), "Id", "Email");
            ViewBag.ClaveVercion = new SelectList(db.LA_Version.Where(a=> a.IdMarcaLA==-1), "Id", "Nombre");
            ViewBag.IdMarcaLA = new SelectList(db.LA_Marcas.Where(a => a.Id == -1), "Id", "Nombre");
            ViewBag.IdModeloLA = new SelectList(db.LA_Modelos.Where(a => a.IdMarcasLA == -1), "Id", "Nombre");
            return PartialView("_Contacto", contacto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateContactoWeb([Bind(Include = "Id,FechaSolicitud,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,NombreCopleto,Telefono,Email,IdUsuarioSolicitud, Comentario")] ContactoWeb contactoWeb)
        {            
            MapValidacion result = new MapValidacion();
            List<ValidacionCampo> listaErrores = new List<ValidacionCampo>();
            listaErrores = getValidacionModelCreate(contactoWeb);

            if (ModelState.IsValid && listaErrores.Count == 0)
            {
                db.ContactoWeb.Add(contactoWeb);
                db.SaveChanges();

                Thread ThreadActualizarMarcas =
                new Thread(
                  unused => EnviarMailComentario(contactoWeb)
                );
                ThreadActualizarMarcas.Start();

                result.Error = false;
                result.Mensaje = "¡Muchas gracias por comunicarte con nosotros! Ya estamos revisando tu mensaje.";
            }
            else
            {
                ValidacionCampo validator = new ValidacionCampo(ModelState);

                listaErrores = validator.getListaErrores(listaErrores);
                result.Error = true;
                result.Mensaje = "Se encontraron algunos errores, \n Favor de resolverlos para continuar con el proceso.";
                result.data = listaErrores;
            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        public void EnviarMailComentario(ContactoWeb contactoWeb)
        {
            
            MailMessage mail = new MailMessage();
            var datos = db.Settings.FirstOrDefault();
            mail.From = new MailAddress("gerencia@autoyvaro.com", "Team: Auto y Varo");

            mail.To.Add("areyes@autoyvaro.com");
            mail.To.Add("mhuerta.r12@gmail.com");
            mail.To.Add("asesor@autoyvaro.com");
            mail.To.Add("gerencia@autoyvaro.com");
            mail.To.Add("ayuda@autoyvaro.com");

            mail.Subject = "Nuevo Mensaje: " + contactoWeb.Id;
            mail.Body = "<div class=\"adM\" style=\"height: 4px; background: #ee7e3c; border-radius: 12px; width: 70%; margin: 40px auto;\">&nbsp;</div><div style=\"margin: auto;\">" +
            "<div class=\"adM\">&nbsp;</div><div style=\"text-align: left; padding: 0 12%;\"><div class=\"adM\">&nbsp;</div><img src=\"http://mhuerta85-001-site3.btempurl.com/Content/assets3/img/LOGO-5.png\" width=\"160\" height=\"78\"></div>" +
            "<p style=\"color: #516766; font-size: 24px; font-weight: bold; padding: 0 15%; margin-top: 35px;\">Nueva Mensaje:</p>" +
            "<div style=\"padding: 0 15%; margin-top: 40px;\"><p style=\"font-weight: 400; font-size: 20px; line-height: 28px; color: 516766;\">&iexcl;Hola Team Auto y Varo! &nbsp;Se recibio el mensaje con id: <strong>" + contactoWeb.Id + "</strong>, favor de dar el seguimiento &nbsp;correspondiente al cliente:</p>" +
            "</div><div style=\"padding: 0 15%; margin-top: 40px;\"><p style=\"font-weight: bold; font-size: 36px; line-height: 53px; color: 516766; margin-bottom: 40px; margin-top: 80px; background-color: #eeeeee; text-align: center;\">Sr(a): <strong>" + contactoWeb.NombreCopleto + "</strong> </br>  </p>" +
            "<div style=\"padding: 0 15%; margin-top: 40px;\"><table style=\"width: 100%; line-height: 53px; color: 516766; margin-bottom: 40px; margin-top: 80px; background-color: #eeeeee; text-align: center;\"><tbody>" +
            "<tr><td><strong>Auto:</strong></td><td><strong>" + (db.LA_Marcas.Find(contactoWeb.IdMarcaLA).Nombre + " " + db.LA_Modelos.Find(contactoWeb.IdModeloLA).Nombre + " " + db.LA_Anios.Find(contactoWeb.IdAnioLA).Nombre + " " + db.LA_Version.Find(contactoWeb.ClaveVercion).Nombre) + "</strong></td></tr>" +
            "<tr><td><strong>Telefono:</strong></td><td>"+ contactoWeb.Telefono + "</td></tr>" +
            "<tr><td><strong>Email:</strong></td><td>" + contactoWeb.Email + "</td></tr>" +
            "<tr><td><strong>Mensaje:</strong></td><td>" + contactoWeb.Comentario + "</td></tr></tbody></table><p>&nbsp;</p></div>" +
            "</div><div style=\"padding: 0 15%; margin: 30px 0; display: flex;\">&nbsp;</div><div style=\"height: 4px; background: #ee7e3c; border-radius: 12px; width: 70%; margin: 40px auto;\">&nbsp;</div></div>";
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
        public List<ValidacionCampo> getValidacionModelCreate(ContactoWeb model)
        {
            using (Entities db = new Entities())
            {
                List<ValidacionCampo> errores = new List<ValidacionCampo>();
                return errores;
            }
        }

        public ActionResult Requisitos() { 
            return View();  
        }


        public ActionResult TerminosyCondiciones() {
            return View();
        }



        public PartialViewResult TerminosPartial() { 
            return PartialView("_TerminosyCondiciones");
        }

        public PartialViewResult AvisoPartial()
        {
            return PartialView("_AvisoPrivacidad");
        }

        public ActionResult EnConstruccion() {
            return View();
        }


    }
}