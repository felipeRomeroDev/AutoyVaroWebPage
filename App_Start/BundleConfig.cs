using System.Web;
using System.Web.Optimization;

namespace AutoyVaro
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/global/vendor/jquery/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));


            //< !--Vendor JS Files -->
            bundles.Add(new Bundle("~/bundles/JSFiles").Include(
                "~/Content/assetsv2/vendor/purecounter/purecounter.js",
                "~/Content/assetsv2/vendor/aos/aos.js",
                "~/Content/assetsv2/vendor/bootstrap/js/bootstrap.bundle.min.js",
                "~/Content/assetsv2/vendor/glightbox/js/glightbox.min.js",
                "~/Content/assetsv2/vendor/isotope-layout/isotope.pkgd.min.js",
                "~/Content/assetsv2/vendor/swiper/swiper-bundle.min.js",
                "~/Content/assetsv2/vendor/php-email-form/validate.js"
            ));



            //< !--Template Main JS File-- >
            bundles.Add(new ScriptBundle("~/bundles/TemplateMain").Include(
                "~/Content/assetsv2/js/main.js"
            ));
    




             bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/assetsv2/vendor/bootstrap/css/bootstrap.css",
                     "~/Content/global/css/bootstrap-extend.css"
            ));

            //< !--Vendor CSS Files -->
            bundles.Add(new StyleBundle("~/Content/VendorCSS").Include(
                "~/Content/assetsv2/vendor/aos/aos.css",                
                 "~/Content/assetsv2/vendor/bootstrap-icons/bootstrap-icons.css",
                 "~/Content/assetsv2/vendor/glightbox/css/glightbox.min.css",
                 "~/Content/assetsv2/vendor/remixicon/remixicon.css",
                 "~/Content/assetsv2/vendor/swiper/swiper-bundle.min.css"
           ));


            

            
            



        }
    }
}
