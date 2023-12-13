using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AutoyVaro
{  
    [MetadataType(typeof(MetadataPlantillaWeb))]
    public partial class PlantillaWeb
    {

        public String GetPlantilla
        {
            get {
                var result = Code;
                var detallePlantilla = PlantillaWebItem.Where(p=> p.Activo).ToList();
                var numItem = 0;
                foreach (var plantilla in detallePlantilla) {
                    switch (plantilla.TipoItemWebId)
                    {
                        case 1:
                            // Numero
                            result = result.Replace("{" + numItem + "}", plantilla.CodeLite);
                            break;
                        case 2:
                            // Cadena
                            result = result.Replace("{" + numItem + "}", plantilla.CodeLite);
                            break;
                        case 3:
                            // HTML
                            result = result.Replace("{" + numItem + "}", plantilla.CodeLite);
                            break;
                        case 4:
                            // Plantilla web
                            result = result.Replace("{" + numItem + "}", plantilla.PlantillaWeb1.GetPlantilla);
                            break;
                        case 5:
                            var itemsHijos = plantilla.PlantillaWebItemDetalle.ToList();
                            var codeTemporal = "";
                            //var numItemHijos = 0;
                            
                            foreach (var itemHijo in itemsHijos) {
                                
                                if (itemHijo.PlantillaWebItem1.Activo) {
                                    
                                
                                
                                switch (itemHijo.PlantillaWebItem1.TipoItemWebId)
                                {
                                    case 1:
                                        // Numero
                                        codeTemporal +=  itemHijo.PlantillaWebItem1.CodeLite;
                                        break;
                                    case 2:
                                        // Cadena
                                        codeTemporal += itemHijo.PlantillaWebItem1.CodeLite;
                                        break;
                                    case 3:
                                        // HTML
                                        codeTemporal += itemHijo.PlantillaWebItem1.Code;
                                        break;
                                    case 4:
                                        // Plantilla web
                                        codeTemporal += itemHijo.PlantillaWebItem1.PlantillaWeb1.GetPlantilla;
                                        break;                                    
                                }


                                //numItemHijos++;
                                }
                            }

                            result = result.Replace("{" + numItem + "}", codeTemporal);

                            break;
                        // code block
                        default:
                            // code block
                            result = Code;
                            break;
                    }
                    numItem++;
                }
                return result;
            }
        }

    }

    public class MetadataPlantillaWeb 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Code { get; set; }
        public Nullable<int> NumeroItem { get; set; }
    }
}