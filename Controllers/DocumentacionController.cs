using AutoyVaro.Helpers.UtilitiesHelper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AutoyVaro.Controllers
{
    public class DocumentacionController : Controller
    {
        private Entities db = new Entities();
        private string FilePath = SettingsHelper.getRutaDirectorioWeb();
        // GET: Documentacion
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult AddArchivoExpediente(int id)
        {
            Fin_DocumentacionSolicitud DC = db.Fin_DocumentacionSolicitud.Find(id);
            ViewBag.TipoArchivo = new SelectList(db.TipoDeArchivo.Where(c => c.id == 0), "id", "Descripcion", "Extension");
            return PartialView("_AddArchivoExpediente", DC);
        }

        public JsonResult NuevoArchivoExpediente(int id, String descripcion, HttpPostedFileBase ArchivoExpediente)
        {
            var result = new
            {
                error = false,
                mensaje = "Archivo cargado con éxito"
            };
            if (ArchivoExpediente != null)
            {
                try
                {
                    Fin_DocumentacionSolicitud DC = db.Fin_DocumentacionSolicitud.Find(id);
                    Fin_Solicitud solicitud = DC.Fin_Solicitud;
                    Fin_DocumentacionSolicitudDetalle expediente = new Fin_DocumentacionSolicitudDetalle();                    

                    string IdentityUserId = User.Identity.GetUserId();                    

                    
                    AspNetUsers user = db.AspNetUsers.Find(IdentityUserId);

                    var nombre = DC.DocumentacionRequerida.nombre + "-" + token(7);
                    expediente.IdDocumentacionSolicitud = DC.Id;
                    expediente.Ruta = FilePath;
                    expediente.Fecha = DateTime.Today;
                    expediente.Nombre = SubirArchivo.UploadFileFTP(ArchivoExpediente, solicitud.Id, solicitud.Fit_Cliente.Id, nombre);
                    expediente.Descripcion = descripcion;
                    expediente.Version = 1;
                    expediente.Activo = true;
                    expediente.IdEstadoDocumentoSolicitud = 1;
                    db.Fin_DocumentacionSolicitudDetalle.Add(expediente);
                    db.SaveChanges();


                    return Json(result, JsonRequestBehavior.AllowGet);


                }
                catch (NullReferenceException e)
                {
                    var result3 = new
                    {
                        error = true,
                        mensaje = "Error al cargar el archivo " + e.Message
                    };
                    return Json(result3, JsonRequestBehavior.AllowGet);
                }

            }
            var result4 = new
            {
                error = true,
                mensaje = "Es necesario seleccionar un archivo"
            };

            return Json(result4, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ListaDocumentos(int IdSolicitud = 0)
        {
            //if (limit == 100) limit = 1000;
            int totalRows = 0;
            var resultado = db.Fin_DocumentacionSolicitud.Where(a => a.IdSolicitud == IdSolicitud).ToList();
            totalRows = resultado.Count;
            var documentos = from d in resultado
                             select new
                             {
                                 d.Id,
                                 idSolicitud = d.Fin_Solicitud.Id,
                                 d.DocumentacionRequerida.nombre,
                                 d.DocumentacionRequerida.descripción,
                                 NumeroArchivos = d.DocumentacionRequerida.NumeroArchivos
                                 

                             };
            return Json(documentos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaDocumentosLite(int IdSolicitud = 0)
        {
            //if (limit == 100) limit = 1000;
            int totalRows = 0;
            var resultado = db.Fin_DocumentacionSolicitud.Where(a => 
            a.IdSolicitud == IdSolicitud
            && a.DocumentacionRequerida.Fase == 1
            ).ToList();
            totalRows = resultado.Count;
            var documentos = from d in resultado
                             select new
                             {
                                 d.Id,
                                 idSolicitud = d.Fin_Solicitud.Id,
                                 d.DocumentacionRequerida.nombre,
                                 d.DocumentacionRequerida.descripción,
                                 NumeroArchivos = d.DocumentacionRequerida.NumeroArchivos,
                                 NumArchivosAdjuntos = d.Fin_DocumentacionSolicitudDetalle.Where(don=> don.IdDocumentacionSolicitud == d.Id).Count()

                             };
            return Json(documentos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaDocumentosPorTipoDocumento(int IdDC = 0)
        {
            //if (limit == 100) limit = 1000;
            int totalRows = 0;
            var resultado = db.Fin_DocumentacionSolicitudDetalle.Where(d => d.IdDocumentacionSolicitud == IdDC).ToList();
            totalRows = resultado.Count;
            var documentos = from d in resultado
                             select new
                             {
                                 d.Id,
                                 d.IdDocumentacionSolicitud,
                                 d.Version,
                                 d.Descripcion,
                                 d.Fecha,
                                 d.Nombre,
                                 d.Ruta,
                                 d.IdEstadoDocumentoSolicitud,                                 
                                 Documento = d.Fin_DocumentacionSolicitud.DocumentacionRequerida.nombre                                 
                             };
            return Json(documentos, JsonRequestBehavior.AllowGet);
        }


        
        public FileContentResult ViewFile(String ruta = "", String nombre = "")
        {
            if (String.IsNullOrEmpty(ruta)) return null;
            MemoryStream workStream = SubirArchivo.DownloadFileFTP(ruta + nombre);
            byte[] fileBytes = workStream.ToArray();

            string[] nombreSplit = nombre.Split('.');

            if (nombreSplit[nombreSplit.Length-1].Equals("pdf") || nombreSplit[nombreSplit.Length-1].Equals("PDF"))
            {
                return new FileContentResult(fileBytes, "application/pdf");
            }
            else {
                return new FileContentResult(fileBytes, "image/jpeg");
            }
            
        }


        #region funciones
        public string token(int longitud)
        {
            Guid miGuid = Guid.NewGuid();
            string token = Convert.ToBase64String(miGuid.ToByteArray());
            token = token.Replace("=", "").Replace("+", "").Replace("/", "");
            return token.Substring(0, longitud);
        }
        #endregion




    }
}