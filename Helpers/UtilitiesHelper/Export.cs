using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace AutoyVaro.Helpers.Export
{
    public static class Export
    {
        public static void ToExcel(HttpResponseBase Response, object modelo)
        {
            var gv = new System.Web.UI.WebControls.GridView();
            gv.DataSource = modelo;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Reporte-Acumulado-" + DateTime.Today + "-.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
    }
}