using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        private Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Emprender()
        {
            return View();
        }

        public ActionResult Estrategias()
        {
            return View();

        }
        public ActionResult Prestamo()
        {
            return View();
        }
        public ActionResult Tablero()
        {
            return View();
        }
        public ActionResult Datos()
        {
            return View();
        }

        public ActionResult Lenguaje()
        {
            return View();
        }
        public ActionResult Facturas()
        {
            return View();
        }
        public ActionResult Modelos()
        {
            return View();
        }

        public ActionResult Obtener()
        {
            return View();
        }

        public ActionResult Enfriamiento()
        {
            return View();
        }
        public ActionResult Afinacion()
        {
            return View();
        }

        public ActionResult Autoyvaro()
        {
            return View();
        }


    }
}