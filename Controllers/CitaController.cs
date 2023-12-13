using AutoyVaro.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class CitaController : Controller
    {
        private Entities db = new Entities();
        // GET: Cita
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int Id)
        {
            ViewBag.IdSolicitud = Id;
            return View();
        }


        public JsonResult GetEvents(DateTime start, DateTime end)
        {
            var viewModel = new EventViewModel();
            var events = new List<EventViewModel>();

            var fecha = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(16);


            var result = db.Calendario.Where(s => !s.InUse).ToList();


            var list = result.Select(i => new EventViewModel
            {
                id = i.Id,
                title = " " + "From " + i.StartDate.ToString("HH:mm:ss") + " To " + i.EndDate.ToString("HH:mm:ss"),
                start = i.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = i.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                allDay = false,
                editable = false,
                inUse = i.InUse
            });

            return Json(list.ToList(), JsonRequestBehavior.AllowGet);
        }




        // POST: WMS_Appointment/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Fin_Cita cita)
        {
            var IdentityUserId = User.Identity.GetUserId();
            AspNetUsers user = db.AspNetUsers.Find(IdentityUserId);
            MapValidacion result = new MapValidacion();
            List<ValidacionCampo> listaErrores = new List<ValidacionCampo>();
            listaErrores = null;

            if (listaErrores == null)
            {

                Calendario horario = new Calendario();

                horario = db.Calendario.Find(cita.IdCalendario);
                horario.InUse = true;
                db.Entry(horario).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                cita.IdUserCliente = IdentityUserId;
                cita.Comentario = "";
                cita.FechaRegistro = DateTime.Now;                
                cita.Activa = true;
                cita.IdTipoCita = 1;
                cita.IdEstadoCita = 1;

                
                db.Fin_Cita.Add(cita);

                db.SaveChanges();
                result.Error = false;
                result.Mensaje = "Success";
            }
            else
            {
                ValidacionCampo validator = new ValidacionCampo(ModelState);

                listaErrores = validator.getListaErrores(listaErrores);
                result.Error = true;
                result.Mensaje = "Internal Error";
                result.data = listaErrores;
            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }





    }



}