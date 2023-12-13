using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class PublicidadController : Controller
    {
        // GET: Publicidad
        public PartialViewResult Principal()
        {
            return PartialView();
        }

        // GET: Publicidad/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Publicidad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Publicidad/Create
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

        // GET: Publicidad/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Publicidad/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Publicidad/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Publicidad/Delete/5
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

        public PartialViewResult CarrucelFacebook() { 
            return PartialView("_CarrucelFacebook");
        }
    }
}
