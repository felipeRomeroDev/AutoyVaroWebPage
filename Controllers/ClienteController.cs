using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class ClienteController : Controller
    {
        private Entities db = new Entities();
        // GET: Cliente
        public ActionResult Index()
        {
            return View();
        }

        // GET: Fit_Cliente/Edit/5
        public PartialViewResult Edit(int? id)
        {            
            Fit_Cliente fit_Cliente = db.Fit_Cliente.Find(id);            
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", fit_Cliente.IdUser);
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion", fit_Cliente.IdEstado);
            ViewBag.IdEstadoCivil = new SelectList(db.Estado_civil, "id", "Descripcion", fit_Cliente.IdEstadoCivil);
            return PartialView("_Edit",fit_Cliente);
        }

        // POST: Fit_Cliente/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,ApellidoPaterno,ApellidoMaterno,RFC,CURP,NumeroIdentificacion,Calle,Colonia,NumeroCasa,EntreCalle,YCalle,CodigoPostal,Ciudad,Municipio,IdEstado,TelefonoCasa,TelefonoOficina,Ext,Celular,CorreoElectronico,FechaCreacion,FechaNacimiento,IdUser,IdTipoDeIdentificacion,ActividadPreponderante,IdEstadoCivil,Nombre1,ApellidoPaterno1,ApellidoMaterno1,ParentescoR1,DireccionR1,TelefonoR1,Nombre2,ApellidoPaterno2,ApellidoMaterno2,TelefonoR2,Nombre3,ApellidoPaterno3,ApellidoMaterno3,TelefonoR3")] Fit_Cliente fit_Cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fit_Cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "Email", fit_Cliente.IdUser);
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion", fit_Cliente.IdEstado);
            ViewBag.IdEstadoCivil = new SelectList(db.Estado_civil, "id", "Descripcion", fit_Cliente.IdEstadoCivil);
            return View(fit_Cliente);
        }

    }
}