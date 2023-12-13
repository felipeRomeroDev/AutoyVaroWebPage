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

        public ActionResult Bujias()
        {
            return View();
        }

        public ActionResult Acelerar()
        {
            return View();

        }
        public ActionResult Filtro()
        {
            return View();
        }
        public ActionResult Tablero()
        {
            return View();
        }
        public ActionResult Motor()
        {
            return View();
        }

        public ActionResult Combustible()
        {
            return View();
        }
        public ActionResult Goteras()
        {
            return View();
        }
        public ActionResult Ventilacion()
        {
            return View();
        }

        public ActionResult Exterior()
        {
            return View();
        }

        public ActionResult Enfriamiento()
        {
            return View();
        }
        public ActionResult Llanta()
        {
            return View();
        }


    }
}