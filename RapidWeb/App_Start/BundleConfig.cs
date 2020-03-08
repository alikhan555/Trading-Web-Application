using System.Web;
using System.Web.Optimization;

namespace RapidWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerymask").Include(
                "~/Scripts/jquery.maskedinput*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themeStyle").Include(
                "~/Content/DataTables/css/jquery.dataTables.min.css",
                "~/Content/vendor/nucleo/css/themeFont.css",
                "~/Content/vendor/nucleo/css/nucleo.css",
                "~/Content/vendor/fortawesome/fontawesome-free/css/all.min.css",
                "~/Content/css/argon.min.css",
                "~/Content/vendor/nucleo/css/theme.css"));

            bundles.Add(new StyleBundle("~/Content/themeJavaScript").Include(
                "~/Content/vendor/jquery/dist/jquery.min.js",
                "~/Content/vendor/bootstrap/dist/js/bootstrap.bundle.min.js",
                "~/Content/vendor/anchor-js/anchor.min.js",
                "~/Content/vendor/clipboard/dist/clipboard.min.js",
                "~/Content/vendor/holderjs/holder.min.js",
                "~/Content/vendor/prismjs/prism.js",
                "~/Content/vendor/chart.js/dist/Chart.min.js",
                "~/Content/vendor/chart.js/dist/Chart.extension.js",
                "~/Content/vendor/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                "~/Content/js/argon.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryDataTable").Include(
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/dataTables.bootstrap.js"));
        }
    }
}
