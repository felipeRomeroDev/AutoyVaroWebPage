using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class VehiculoController : Controller
    {
        // GET: Vehiculo
        private Entities db = new Entities();

        // GET: Fit_Vehiculo
        

        // GET: Fit_Vehiculo/Details/5
        public ActionResult Details(int? id)
        {
            
            Fit_Vehiculo fit_Vehiculo = db.Fit_Vehiculo.Find(id);
            if (fit_Vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(fit_Vehiculo);
        }

        // GET: Fit_Vehiculo/Create
        public ActionResult Create()
        {
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion");
            ViewBag.IdCliente = new SelectList(db.Fit_Cliente, "Id", "Nombre");
            ViewBag.IdAnioLA = new SelectList(db.LA_Anios, "Id", "Nombre");
            ViewBag.ClaveVercion = new SelectList(db.LA_Version, "Id", "Nombre");
            ViewBag.IdMarcaLA = new SelectList(db.LA_Marcas, "Id", "Nombre");
            ViewBag.IdModeloLA = new SelectList(db.LA_Modelos, "Id", "Nombre");
            ViewBag.IdTipoTransmision = new SelectList(db.TipoTransmision, "id", "Descripcion");
            return View();
        }

        // POST: Fit_Vehiculo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NumeroDeSerie,NumeroDeMotor,Placas,TarjetaDeCirculacion,IdEstado,FechaSolicitud,IdCliente,Color,RFCFactura,NumeroFactura,FechaFactura,NumeroDeTarjetaDeCirculacion,NumeroDePuertas,NumeroDeAsientos,NumeroDeCilindros,IdTipoTransmision")] Fit_Vehiculo fit_Vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Fit_Vehiculo.Add(fit_Vehiculo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion", fit_Vehiculo.IdEstado);
            ViewBag.IdCliente = new SelectList(db.Fit_Cliente, "Id", "Nombre", fit_Vehiculo.IdCliente);
            ViewBag.IdAnioLA = new SelectList(db.LA_Anios, "Id", "Nombre", fit_Vehiculo.IdAnioLA);
            ViewBag.ClaveVercion = new SelectList(db.LA_Version, "Id", "Nombre", fit_Vehiculo.ClaveVercion);
            ViewBag.IdMarcaLA = new SelectList(db.LA_Marcas, "Id", "Nombre", fit_Vehiculo.IdMarcaLA);
            ViewBag.IdModeloLA = new SelectList(db.LA_Modelos, "Id", "Nombre", fit_Vehiculo.IdModeloLA);
            ViewBag.IdTipoTransmision = new SelectList(db.TipoTransmision, "id", "Descripcion", fit_Vehiculo.IdTipoTransmision);
            return View(fit_Vehiculo);
        }

        // GET: Fit_Vehiculo/Edit/5
        public PartialViewResult Edit(int? id)
        {            
            Fit_Vehiculo fit_Vehiculo = db.Fit_Vehiculo.Find(id);            
            ViewBag.IdEstado = new SelectList(db.Estado.Where(w=> w.id == fit_Vehiculo.IdEstado), "id", "Descripcion", fit_Vehiculo.IdEstado);
            ViewBag.IdCliente = new SelectList(db.Fit_Cliente.Where(w=>  w.Id == fit_Vehiculo.IdCliente), "Id", "Nombre", fit_Vehiculo.IdCliente);
            ViewBag.IdAnioLA = new SelectList(db.LA_Anios.Where(w=>  w.Id == fit_Vehiculo.IdAnioLA), "Id", "Nombre", fit_Vehiculo.IdAnioLA);
            ViewBag.ClaveVercion = new SelectList(db.LA_Version.Where(w => w.Id == fit_Vehiculo.ClaveVercion), "Id", "Nombre", fit_Vehiculo.ClaveVercion);
            ViewBag.IdMarcaLA = new SelectList(db.LA_Marcas.Where(w => w.Id == fit_Vehiculo.IdMarcaLA), "Id", "Nombre", fit_Vehiculo.IdMarcaLA);
            ViewBag.IdModeloLA = new SelectList(db.LA_Modelos.Where(w => w.Id == fit_Vehiculo.IdModeloLA), "Id", "Nombre", fit_Vehiculo.IdModeloLA);
            ViewBag.IdTipoTransmision = new SelectList(db.TipoTransmision, "id", "Descripcion", fit_Vehiculo.IdTipoTransmision);
            return PartialView("_Edit", fit_Vehiculo);
        }

        // POST: Fit_Vehiculo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdAnioLA,IdMarcaLA,IdModeloLA,ClaveVercion,Compra,Venta,NumeroDeSerie,NumeroDeMotor,Placas,TarjetaDeCirculacion,IdEstado,FechaSolicitud,IdCliente,Color,RFCFactura,NumeroFactura,FechaFactura,NumeroDeTarjetaDeCirculacion,NumeroDePuertas,NumeroDeAsientos,NumeroDeCilindros,IdTipoTransmision")] Fit_Vehiculo fit_Vehiculo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fit_Vehiculo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEstado = new SelectList(db.Estado, "id", "Descripcion", fit_Vehiculo.IdEstado);
            ViewBag.IdCliente = new SelectList(db.Fit_Cliente, "Id", "Nombre", fit_Vehiculo.IdCliente);
            ViewBag.IdAnioLA = new SelectList(db.LA_Anios, "Id", "Nombre", fit_Vehiculo.IdAnioLA);
            ViewBag.ClaveVercion = new SelectList(db.LA_Version, "Id", "Nombre", fit_Vehiculo.ClaveVercion);
            ViewBag.IdMarcaLA = new SelectList(db.LA_Marcas, "Id", "Nombre", fit_Vehiculo.IdMarcaLA);
            ViewBag.IdModeloLA = new SelectList(db.LA_Modelos, "Id", "Nombre", fit_Vehiculo.IdModeloLA);
            ViewBag.IdTipoTransmision = new SelectList(db.TipoTransmision, "id", "Descripcion", fit_Vehiculo.IdTipoTransmision);
            return View(fit_Vehiculo);
        }

        // GET: Fit_Vehiculo/Delete/5
        public ActionResult Delete(int? id)
        {            
            Fit_Vehiculo fit_Vehiculo = db.Fit_Vehiculo.Find(id);
            if (fit_Vehiculo == null)
            {
                return HttpNotFound();
            }
            return View(fit_Vehiculo);
        }

        // POST: Fit_Vehiculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fit_Vehiculo fit_Vehiculo = db.Fit_Vehiculo.Find(id);
            db.Fit_Vehiculo.Remove(fit_Vehiculo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}