using AutoyVaro.Management;
using AutoyVaro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Controllers
{
    public class EstadoDeCuentaController : Controller
    {
        private Entities db = new Entities();
        // GET: EstadoDeCuenta
        public ActionResult Index()
        {
            return View();
        }

        //Retorna el estado de cuenta para mostrar en el modulo del Cliente
        public PartialViewResult getEstadoDeCuentaPeticionCliente(ClientePeticionModel obj)
        {
            Credito credito = new Credito();

            credito = db.Credito.Find(obj.NumeroCredito);
            if (!credito.NuevoModelo)
            {
                return PartialView("_EstadoDeCuentaV1", credito);
            }
            else
            {
                return PartialView("_EstadoDeCuentaV2", credito);
            }
        }

        public JsonResult Validar(ClientePeticionModel obj)
        {
            Credito credito = new Credito();
            credito = db.Credito.Find(obj.NumeroCredito);
            bool Error = false;
            string Mensaje = "Datos validos";



            if (credito == null)
            {
                Error = true;
                Mensaje = "No éxiste el número de crédito que ingreso.";
            }
            else
            {
                if (credito.Vehiculo.Cliente.RFC != obj.RFC)
                {
                    Error = true;
                    Mensaje = "El RFC no coincide con el Crédito " + obj.NumeroCredito + ".";
                }
            }


            if (!Error)
            {

                PaymentManagement p = new PaymentManagement(credito.id);
                p.RevertOverdueReceivables(DateTime.Now);

            }

            var result = new
            {
                Error = Error,
                Mensaje = Mensaje
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerarEstadoDeCuenta(int idCredito)
        {

            Credito credito = db.Credito.Find(idCredito);

            return new RazorPDF.PdfResult(credito, "PdfEstadoCuenta");
        }

    }
}