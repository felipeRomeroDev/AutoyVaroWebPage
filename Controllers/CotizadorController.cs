using AutoyVaro.com.libroazul.www;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using AutoyVaro.Models;
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace AutoyVaro.Controllers
{
    public class CotizadorController : Controller
    {
        private Entities db = new Entities();
        private String llave = "";
        private String llave1 = "";
        // GET: Page
        public ActionResult Index()
        {
            int IdentityUserId = 1;
            User user = db.User.Find(IdentityUserId);
            bool autorizado = true;

            if (user.Sucursal == 7 || user.Sucursal == 14 || user.Sucursal == 15)
            {
                autorizado = false;
                ViewBag.autorizado = autorizado;
                return View();
            }

            ViewBag.autorizado = autorizado;

            var wsLibrozul = new wslibroazulSoapClient();
            //llave1 = wsLibrozul.IniciarSesion("demoAutoVaro5221", "remo3672");

            try
            {
                llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "12werwerwe1234");
                Session["llaveTemp"] = llave;
                var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);
                ViewBag.IdAnio = new SelectList(listaAnios, "Clave", "Nombre");

                Thread ThreadActualizarMarcas =
                 new Thread(
                   unused => ActualizarAnios(listaAnios)
                 );
                ThreadActualizarMarcas.Start();
            }
            catch (Exception e)
            {
                int anio = DateTime.Now.Year - 10; 
                ViewBag.IdAnio = new SelectList(db.LA_Anios.Where(w=> w.Id >= anio).Distinct().OrderByDescending(a=> a.Id), "Id", "Nombre");
                return View();
            }
            return View();
        }

        private void ActualizarAnios(Catalogo[] listaAnios)
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

        public JsonResult GetMarcaSelect(int IdAnio)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "eeyrtye*8541");
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
            List<LA_MarcasPorAnio> listaLA_MarcasPorAnio = new List<LA_MarcasPorAnio>();
            foreach (Catalogo marcas in listaMarcas)
            {
                int claveMarca = int.Parse(marcas.Clave);
                if (db.LA_Marcas.Find(claveMarca) == null)
                {
                    LA_Marcas mar = new LA_Marcas();
                    mar.Active = true;
                    mar.Id = int.Parse(marcas.Clave);
                    mar.Nombre = marcas.Nombre;
                    db.LA_Marcas.Add(mar);
                    db.SaveChanges();

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
                db.LA_MarcasPorAnio.AddRange(listaLA_MarcasPorAnio);
                db.SaveChanges();
            }
        }

        public JsonResult GetModeloSelect(int IdAnio, int IdMarca)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "datewrta*wertwer8541");
            var listaModelos = wsLibrozul.ObtenerModelosPorAnioMarca(llave, 0, IdAnio, IdMarca, 0);

            Thread ThreadActualizarMarcas =
              new Thread(
                unused => ActualizarModelos(listaModelos, IdAnio, IdMarca)
              );
            ThreadActualizarMarcas.Start();

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



        public PartialViewResult GetVersionSelect(int IdAnio, int IdMarca, int IdModelo, String Marca, String Modelo)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "wertwert*8541");
            Catalogo[] listaVersiones = wsLibrozul.ObtenerVersionesPorAnioMarcaModelo(llave, 0, IdAnio, IdMarca, IdModelo, 0);
            List<DetalleVersion> detallesVersiones = new List<DetalleVersion>();

            foreach (Catalogo cat in listaVersiones)
            {
                String marcaTemp = Marca.Substring(0, 3);
                String Mod = Modelo.Replace(" ", "");
                DetalleVersion item = new DetalleVersion();
                item.Anio = IdAnio;
                item.Clave = cat.Clave;
                item.IdMarca = IdMarca;
                item.IdModelo = IdModelo;
                item.Marca = Marca;
                item.Modelo = Modelo;
                item.Nombre = cat.Nombre;
                item.Puertas = "";
                item.URLFRENTE = "https://www.libroazul.com/thumb.php?h=600&imagen=ebc_admin/catalogos/CHRCOMPASSLATITUDA2014FRENTE.jpg";
                item.URLTRAS = "";
                item.Version = "";
                detallesVersiones.Add(item);
            }

            Thread ThreadActualizarMarcas =
            new Thread(
              unused => ActualizarVersion(listaVersiones, IdAnio, IdMarca, IdModelo)
            );
            ThreadActualizarMarcas.Start();

            return PartialView("_Version", detallesVersiones);
        }

        public JsonResult GetVersionComboBox(int IdAnio, int IdMarca, int IdModelo, String Marca, String Modelo)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "wertwertert*8541");
            Catalogo[] listaVersiones = wsLibrozul.ObtenerVersionesPorAnioMarcaModelo(llave, 0, IdAnio, IdMarca, IdModelo, 0);
            List<DetalleVersion> detallesVersiones = new List<DetalleVersion>();

            foreach (Catalogo cat in listaVersiones)
            {
                String marcaTemp = Marca.Substring(0, 3);
                String Mod = Modelo.Replace(" ", "");
                DetalleVersion item = new DetalleVersion();
                item.Anio = IdAnio;
                item.Clave = cat.Clave;
                item.IdMarca = IdMarca;
                item.IdModelo = IdModelo;
                item.Marca = Marca;
                item.Modelo = Modelo;
                item.Nombre = cat.Nombre;
                item.Puertas = "";
                item.URLFRENTE = "https://www.libroazul.com/thumb.php?h=600&imagen=ebc_admin/catalogos/CHRCOMPASSLATITUDA2014FRENTE.jpg";
                item.URLTRAS = "";
                item.Version = "";
                detallesVersiones.Add(item);
            }

            Thread ThreadActualizarMarcas =
            new Thread(
              unused => ActualizarVersion(listaVersiones, IdAnio, IdMarca, IdModelo)
            );
            ThreadActualizarMarcas.Start();

            return Json(detallesVersiones, JsonRequestBehavior.AllowGet);
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

        public PartialViewResult GetPrecioDeCompra(String Clave, string version)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("wert", "wert*wertw");
            ViewBag.version = version;

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

                return PartialView("_Precio", precioHistorico);
            }
            else
            {
                var precio = wsLibrozul.ObtenerPrecioVersionPorClave(llave, 0, Clave, 0);

                Thread ThreadActualizarVersion =
                new Thread(
                  unused => ActualizarHistorial(precio, Clave)
                );
                ThreadActualizarVersion.Start();

                return PartialView("_Precio", precio);
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


        public CatalogoDetalles[] GetDetalleVersion(String Clave)
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "dawertwert41");
            return wsLibrozul.ObtenerDetallesVersionPorClave(llave, Clave);
        }


        public JsonResult ActualizarAnios()
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "dawerwerwer1");
            var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);

            List<LA_MarcasPorAnio> listaLA_MarcasPorAnio = new List<LA_MarcasPorAnio>();
            List<LA_ModelosXMarcaAnio> listaLA_ModelosXMarcaAnio = new List<LA_ModelosXMarcaAnio>();

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
                var listaMarcas = wsLibrozul.ObtenerMarcasPorAnio(llave, 0, int.Parse(anios.Clave), 0);





                foreach (Catalogo marcas in listaMarcas)
                {
                    int claveMarca = int.Parse(marcas.Clave);
                    if (db.LA_Marcas.Find(claveMarca) == null)
                    {
                        LA_Marcas mar = new LA_Marcas();
                        mar.Active = true;
                        mar.Id = int.Parse(marcas.Clave);
                        mar.Nombre = marcas.Nombre;
                        db.LA_Marcas.Add(mar);
                        db.SaveChanges();

                        LA_MarcasPorAnio marcasPorAnio = new LA_MarcasPorAnio
                        {
                            IdAnioLA = claveAnio,
                            IdMarcaLA = mar.Id
                        };
                        listaLA_MarcasPorAnio.Add(marcasPorAnio);
                    }

                    var listaModelos = wsLibrozul.ObtenerModelosPorAnioMarca(llave, 0, int.Parse(anios.Clave), int.Parse(marcas.Clave), 0);

                    foreach (Catalogo modelos in listaModelos)
                    {
                        int claveModelo = int.Parse(modelos.Clave);
                        if (db.LA_Modelos.Find(claveModelo) == null)
                        {
                            LA_Modelos mod = new LA_Modelos();
                            mod.Active = true;
                            mod.Id = int.Parse(modelos.Clave);
                            mod.Nombre = modelos.Nombre;
                            mod.IdMarcasLA = db.LA_Marcas.Find(claveMarca).Id;
                            db.LA_Modelos.Add(mod);
                            db.SaveChanges();

                            LA_ModelosXMarcaAnio modelosXMarcaAnio = new LA_ModelosXMarcaAnio
                            {
                                IdAnioLA = claveAnio,
                                IdMarcaLA = mod.IdMarcasLA,
                                IdModeloLA = mod.Id
                            };
                            listaLA_ModelosXMarcaAnio.Add(modelosXMarcaAnio);

                        }

                        //Catalogo[] listaVersiones = wsLibrozul.ObtenerVersionesPorAnioMarcaModelo(llave, 0, int.Parse(anios.Clave), int.Parse(marcas.Clave), int.Parse(modelos.Clave), 0);

                        //foreach (Catalogo versiones in listaVersiones)
                        //{
                        //    if (db.LA_Version.Where(a => a.Id == versiones.Clave).Count() == 0)
                        //    {
                        //        LA_Version ver = new LA_Version();
                        //        ver.Active = true;
                        //        ver.Id = versiones.Clave;
                        //        ver.Nombre = versiones.Nombre;
                        //        ver.IdModeloLA = db.LA_Modelos.Where(a => a.Id == claveModelo).FirstOrDefault().Id;
                        //        db.LA_Version.Add(ver);
                        //        db.SaveChanges();
                        //    }                            
                        //}

                    }
                }




            }


            if (listaLA_MarcasPorAnio.Count() > 0)
            {
                db.LA_MarcasPorAnio.AddRange(listaLA_MarcasPorAnio);
                db.SaveChanges();
            }

            if (listaLA_ModelosXMarcaAnio.Count() > 0)
            {
                db.LA_ModelosXMarcaAnio.AddRange(listaLA_ModelosXMarcaAnio);
                db.SaveChangesAsync();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult ActualizarAnios2()
        {
            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "dwerwwwer1");
            var listaAnios = wsLibrozul.ObtenerAnios(llave, 0, 0);

            List<LA_MarcasPorAnio> listaLA_MarcasPorAnio = new List<LA_MarcasPorAnio>();
            List<LA_ModelosXMarcaAnio> listaLA_ModelosXMarcaAnio = new List<LA_ModelosXMarcaAnio>();

            foreach (Catalogo anios in listaAnios)
            {
                int claveAnio = int.Parse(anios.Clave);

                var listaMarcas = wsLibrozul.ObtenerMarcasPorAnio(llave, 0, int.Parse(anios.Clave), 0);

                foreach (Catalogo marcas in listaMarcas)
                {
                    int claveMarca = int.Parse(marcas.Clave);

                    var listaModelos = wsLibrozul.ObtenerModelosPorAnioMarca(llave, 0, int.Parse(anios.Clave), int.Parse(marcas.Clave), 0);

                    foreach (Catalogo modelos in listaModelos)
                    {
                        int claveModelo = int.Parse(modelos.Clave);

                        LA_ModelosXMarcaAnio modelosXMarcaAnio = new LA_ModelosXMarcaAnio
                        {
                            IdAnioLA = claveAnio,
                            IdMarcaLA = claveMarca,
                            IdModeloLA = claveModelo
                        };
                        listaLA_ModelosXMarcaAnio.Add(modelosXMarcaAnio);

                    }
                }
            }




            if (listaLA_ModelosXMarcaAnio.Count() > 0)
            {
                db.LA_ModelosXMarcaAnio.AddRange(listaLA_ModelosXMarcaAnio);
                db.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult Detalle()
        {

            var lista = db.User.Where(a => a.LA_HistotialConsulastas.Count() > 0).Select(h => new UsuarioHistorialLA
            {
                IdUsuario = h.UserId,
                Nombre = h.FullName,
                Sucursal = h.Sucursal1.Descripción,
                Consultas = h.LA_HistotialConsulastas.Count()
            }).ToList();

            return PartialView("_Detalle", lista);
        }

        //Consultas Por usuario
        public PartialViewResult Consultas(int Id)
        {
            var usuario = db.User.Find(Id);

            var lista = usuario.LA_HistotialConsulastas.OrderByDescending(f => f.FechaConsulta).Select(h => new ListaCounsultasLA
            {
                Anio = h.IdAnioLA,
                Marca = h.LA_Marcas.Nombre,
                Modelo = h.LA_Modelos.Nombre,
                Version = h.LA_Version.Nombre,
                Compra = h.Compra,
                Venta = h.Venta,
                Moneda = h.Moneda
            }).ToList();

            return PartialView("_Consultas", lista);
        }

        // POST: ListaNegras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult CreateSolicitud([Bind(Include = "Id,FechaSolicitud,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NombreCopleto,Telefono,Email, Nombre, ApellidoPaterno, ApellidoMaterno")] SolicitudCotizacion solicitudCotizacion)
        {
            var seed = Environment.TickCount;
            var random = new Random(seed);
            var codigo = random.Next(1000, 9999);

            SendSMS(solicitudCotizacion.Telefono, codigo.ToString());

            solicitudCotizacion.FechaSolicitud = DateTime.Now;
            solicitudCotizacion.CodigoValidacionNumero = codigo.ToString();
            solicitudCotizacion.CodigoValidado = false;
            solicitudCotizacion.EstadoSolicitudCotizacionId = 1;
            solicitudCotizacion.Compra = 0;
            solicitudCotizacion.Venta = 0;
            db.SolicitudCotizacion.Add(solicitudCotizacion);

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                var error = new
                {
                    Error = true,
                    Messag = e.Message
                };
                return Json(error, JsonRequestBehavior.AllowGet);
            }

            solicitudCotizacion.CodigoValidacionNumero = "";
            return Json(solicitudCotizacion, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidarSolicitud([Bind(Include = "Id,FechaSolicitud,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NombreCopleto,Telefono,Email,CodigoValidacionNumero,CodigoValidado,EstadoSolicitudCotizacionId")] SolicitudCotizacion solicitudCotizacion)
        {

            var wsLibrozul = new wslibroazulSoapClient();
            llave = wsLibrozul.IniciarSesion("prodAutoVaro5221", "dawerwerwe41");

            var precio = new Precio();


            var solicitud = db.SolicitudCotizacion.Find(solicitudCotizacion.Id);

            if (solicitud.CodigoValidacionNumero != solicitudCotizacion.CodigoValidacionNumero)
            {
                var error = new
                {
                    Error = true,
                    Mensaje = "Tu código no coincide, puedes volver a intentarlo."
                };
                return Json(error, JsonRequestBehavior.AllowGet);                
            }

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
                ActualizarHistorial(precio, solicitudCotizacion.ClaveVercion);
            }

            solicitud.CodigoValidado = true;
            solicitud.EstadoSolicitudCotizacionId = 2;
            solicitud.Compra = Decimal.Parse(precio.Compra);
            solicitud.Venta = Decimal.Parse(precio.Venta);
            db.Entry(solicitud).State = EntityState.Modified;
            db.SaveChanges();

            var result = new {
                Error = false,
                Mensaje = "El registro se realizó con éxito."
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
            
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

    }
}