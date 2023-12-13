using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class PromocionController : Controller
    {

        private Entities db;
        // GET: Promocion
        public ActionResult Index(int id)
        {
            Cliente cliente = new Cliente();
            db.Cliente.Find(1);
            return View(cliente);
        }

        public ActionResult Create() { 
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }


        public ActionResult Edit()
        {
            return View();
        }


    }
}
