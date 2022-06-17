using System.Web;
using System.Web.Optimization;

namespace HRMMVC
{

    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
            "~/Scripts/jquery-{version}.js",
            "~/Scripts/umd/popper.min.js",
            "~/Scripts/bootstrap.min.js",
            "~/Scripts/jquery.validate.min.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js",
            "~/Scripts/toastr.min.js"
            ));





            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Assets/css/careerstyle.css"));







            // User Layout, =====================================================================================================
            // Css
            bundles.Add(new StyleBundle("~/bundles/usercss").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.min.css",
                "~/Content/Chart.min.css",
                "~/Content/chosen.css",
                 "~/Content/DataTables/css/dataTables.jqueryui.min.css",
                  "~/Content/DataTables/css/dataTables.foundation.css",
                 "~/Content/toastr.min.css",
                "~/Assets/css/StyleUser.css"));
            //js
            bundles.Add(new ScriptBundle("~/bundles/userjs").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                 "~/Scripts/DataTables/jquery.dataTables.min.js",
                "~/Scripts/Chart.min.js",
                "~/Scripts/chosen.jquery.min.js",
                "~/Scripts/toastr.min.js",
                "~/Assets/js/CreditCardValidator.js",
                "~/Assets/js/JavaScriptUser.js"));









        }




    }
}
