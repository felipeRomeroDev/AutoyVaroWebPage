using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Models
{
    public class ValidacionCampo
    {
        ModelStateDictionary modelState;
        public ValidacionCampo()
        {

        }
        public ValidacionCampo(ModelStateDictionary modelState)
        {
            this.modelState = modelState;
        }
        public String Id { get; set; }
        public String Mensaje { get; set; }

        public List<ValidacionCampo> getListaErrores(List<ValidacionCampo> lista)
        {
            List<ValidacionCampo> result = lista;

            foreach (var modelStateKey in modelState.Keys)
            {
                int cont = 0;
                var modelStateVal = modelState[modelStateKey];
                ValidacionCampo nuevo = new ValidacionCampo();
                nuevo.Id = modelStateKey;
                nuevo.Mensaje = "";
                foreach (var error in modelStateVal.Errors)
                {
                    if (error.ErrorMessage.Length > 0)
                    {
                        nuevo.Mensaje = nuevo.Mensaje + " \n " + error.ErrorMessage;
                        cont++;
                    }

                    // You may log the errors if you want
                }
                if (cont > 0)
                {
                    result.Add(nuevo);
                }

            }




            return result;
        }

    }

    public class MapValidacion
    {
        public Boolean Error { get; set; }
        public String Mensaje { get; set; }
        public object data { get; set; }
        public int id { get; set; }

    }
}