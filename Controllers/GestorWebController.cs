using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoyVaro.Helpers.UtilitiesHelper;
using AutoyVaro.Models;

namespace AutoyVaro.Controllers
{
    public class GestorWebController : Controller
    {

        private Entities db = new Entities();
        
        // GET: GestorWeb
        public ActionResult Index()
        {
            return View();
        }

        // GET: GestorWeb/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GestorWeb/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GestorWeb/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GestorWeb/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GestorWeb/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public JsonResult Edit(int Id, PlantillaWebItem plantillaWebItem)
        {
            MapValidacion result = new MapValidacion();
            List<ValidacionCampo> listaErrores = new List<ValidacionCampo>();
            if (ModelState.IsValid)
            {
                result.Error = false;
                result.data = null;
                result.Mensaje = "El registro se realizó con éxito.";
                db.Entry(plantillaWebItem).State = EntityState.Modified;
                db.SaveChanges();

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

        // GET: GestorWeb/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GestorWeb/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



        public JsonResult GetPlantillas(String search="") {
            var data = db.PlantillaWeb.Where(p => p.Descripcion.Contains(search));
            var result = data.Select(a=> new { 
                a.Id,
                a.Nombre,
                a.Descripcion,
                a.NumeroItem
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPlantallaItemsByIdPlantilla(int Id)
        {
            var plantilla = db.PlantillaWeb.Find(Id);
            var result = plantilla.PlantillaWebItem.Select(a => new {
                a.Id,
                a.PlantillaWebId,                
                a.Nombre,
                a.Descripcion,
                a.Code,
                a.CodeLite,
                Tipo= a.TipoItemWeb.Nombre,
                a.TipoItemWebId,
                a.Activo
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Obtiene los items que corresponden a un item de la plantilla
        public JsonResult GetPlantallaItemsDeItemByIdPlantilla(int Id)
        {
            var plantilla = db.PlantillaWebItem.Find(Id);
            var result = plantilla.PlantillaWebItemDetalle.Select(a => new {
                a.Id,
                a.PlantillaWebItemId,
                a.PlantillaWebItemHijoId,
                a.PlantillaWebItem1.Nombre,
                a.PlantillaWebItem1.Descripcion,
                a.PlantillaWebItem1.Code,
                a.PlantillaWebItem1.CodeLite,
                a.PlantillaWebItem1.TipoItemWebId,
                plantilla = a.PlantillaWebItem1.PlantillaWeb != null ? a.PlantillaWebItem1.PlantillaWeb.Nombre : "",
                Tipo = a.PlantillaWebItem1.TipoItemWeb.Nombre,
                a.PlantillaWebItem1.Activo
            });;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult GetPlantillaById(int id=0) {
            var model = db.PlantillaWeb.Find(id);
            return PartialView("_PlantillaById", model);
        }

        public JsonResult DuplicarItem(int Id) {

            var itemWeb = db.PlantillaWebItem.Find(Id);
            PlantillaWebItem nuevoItem = new PlantillaWebItem();
            nuevoItem.Nombre = itemWeb.Nombre + "_COPIA";
            nuevoItem.Descripcion = itemWeb.Descripcion;
            nuevoItem.Code = itemWeb.Code;
            nuevoItem.CodeLite = itemWeb.CodeLite;
            nuevoItem.TipoItemWebId = itemWeb.TipoItemWebId;
            nuevoItem.PlantillaWebId = itemWeb.PlantillaWebId;
            nuevoItem.Activo = itemWeb.Activo;
            db.PlantillaWebItem.Add(nuevoItem);
            db.SaveChanges();


            if (itemWeb.PlantillaWebItemDetalle.Count > 0)
            {
                foreach (var item in itemWeb.PlantillaWebItemDetalle)
                {                    
                        PlantillaWebItem itemHijo = new PlantillaWebItem();
                        itemHijo.Nombre = item.PlantillaWebItem1.Nombre;
                        itemHijo.Descripcion = item.PlantillaWebItem1.Descripcion;
                        itemHijo.Code = item.PlantillaWebItem1.Code;
                        itemHijo.CodeLite = item.PlantillaWebItem1.CodeLite;
                        itemHijo.TipoItemWebId = item.PlantillaWebItem1.TipoItemWebId;
                        itemHijo.PlantillaWebId = item.PlantillaWebItem1.PlantillaWebId;
                        itemHijo.Activo = item.PlantillaWebItem1.Activo;
                        db.PlantillaWebItem.Add(itemHijo);
                        db.SaveChanges();

                        PlantillaWebItemDetalle plantillaWebItemDetalle = new PlantillaWebItemDetalle();
                        plantillaWebItemDetalle.PlantillaWebItemId = nuevoItem.Id;
                        plantillaWebItemDetalle.PlantillaWebItemHijoId = itemHijo.Id;
                        db.PlantillaWebItemDetalle.Add(plantillaWebItemDetalle);
                        db.SaveChanges();

                    
                }
            }

            var Result = new
            {
                Error = false,
                Mensaje = "Se duplico con éxito"
            };
            //1   Número Número entero o decimal
            //2   Cadena Cadena de texto plano
            //3   HTML Codigo HTML
            //4   PlantillaWeb Plantilla Web
            //5   Multiple Vector de items

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DuplicarItemSimple(int Id)
        {

            var itemWeb = db.PlantillaWebItem.Find(Id);

            //Item Hijo
            PlantillaWebItem nuevoItem = new PlantillaWebItem();
            nuevoItem.Nombre = itemWeb.Nombre + "_COPIA";
            nuevoItem.Descripcion = itemWeb.Descripcion;
            nuevoItem.Code = itemWeb.Code;
            nuevoItem.CodeLite = itemWeb.CodeLite;
            nuevoItem.TipoItemWebId = itemWeb.TipoItemWebId;
            nuevoItem.PlantillaWebId = itemWeb.PlantillaWebId;
            nuevoItem.Activo = itemWeb.Activo;
            db.PlantillaWebItem.Add(nuevoItem);
            db.SaveChanges();

            PlantillaWebItemDetalle plantillaWebItemDetalle = new PlantillaWebItemDetalle();
            plantillaWebItemDetalle.PlantillaWebItemId = itemWeb.PlantillaWebItemDetalle1.FirstOrDefault().PlantillaWebItemId;
            plantillaWebItemDetalle.PlantillaWebItemHijoId = nuevoItem.Id;
            db.PlantillaWebItemDetalle.Add(plantillaWebItemDetalle);
            db.SaveChanges();



            var Result = new
            {
                Error = false,
                Mensaje = "Se duplico con éxito"
            };
            //1   Número Número entero o decimal
            //2   Cadena Cadena de texto plano
            //3   HTML Codigo HTML
            //4   PlantillaWeb Plantilla Web
            //5   Multiple Vector de items

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult EditarItemHtml(int Id) { 
            PlantillaWebItem model = new PlantillaWebItem();
            model = db.PlantillaWebItem.Find(Id);
            return PartialView("_formEditarItem", model);
        }

        public JsonResult CreateItemHtml(int Id)
        {
            PlantillaWebItem model = new PlantillaWebItem();
            model.Nombre = "Nuevo item";
            model.Descripcion = "Descripción item";
            model.Code = "";
            model.TipoItemWebId = 3;
            model.Activo = true;
            db.PlantillaWebItem.Add(model);
            db.SaveChanges();
            PlantillaWebItemDetalle plantillaWebItemDetalle = new PlantillaWebItemDetalle();
            plantillaWebItemDetalle.PlantillaWebItemId = Id;
            plantillaWebItemDetalle.PlantillaWebItemHijoId = model.Id;
            db.PlantillaWebItemDetalle.Add(plantillaWebItemDetalle);
            db.SaveChanges();

            PlantillaWebItem modelResul= new PlantillaWebItem();
            modelResul = db.PlantillaWebItem.Find(model.Id);
            var result = new
            {
                error=false,
                mensaje="Se creo un nuevo item"
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UploadImage(HttpPostedFileBase ArchivoExpediente)
        {

            if (ArchivoExpediente != null)
            {
                try
                {
                    var nombre = token(9);
                    String imagen = SubirArchivo.UploadFile(ArchivoExpediente, nombre);

                    var result = new
                    {
                        error = false,
                        mensaje = "Archivo cargado con éxito",
                        imagen = imagen
                    };

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
