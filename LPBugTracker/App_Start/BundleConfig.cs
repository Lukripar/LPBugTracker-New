using System.Web;
using System.Web.Optimization;

namespace LPBugTracker
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

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/Content/dataTables.bootstrap.min.css",
                "~/Content/buttons.bootstrap.min.css",
                "~/Content/fixedHeader.bootstrap.min.css",
                "~/Content/responsive.bootstrap.min.css",
                "~/Content/scroller.bootstrap.min.css"
                //"~/Content/pnotify/pnotify.css",
                //"~/Content/pnotify/pnotify.buttons.css",
                //"~/Content/pnotify/pnotify.nonblock.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/gentelella").Include(
                "~/Scripts/fastclick.js",
                "~/Scripts/nprogress.js",
                "~/Scripts/Chart.min.js",
                "~/Scripts/gauge.min.js",
                "~/Scripts/bootstrap-progressbar.min.js",
                "~/Scripts/icheck.min.js",
                "~/Scripts/skycons.js",
                "~/Scripts/jquery.flot.js",
                "~/Scripts/jquery.flot.pie.js",
                "~/Scripts/jquery.flot.time.js",
                "~/Scripts/jquery.flot.stack.js",
                "~/Scripts/jquery.flot.resize.js",
                "~/Scripts/jquery.flot.orderBars.js",
                "~/Scripts/jquery.flot.spline.min.js",
                "~/Scripts/curvedLines.js",
                "~/Scripts/date.js",
                "~/Scripts/jquery.vmap.js",
                "~/Scripts/jquery.vmap.world.js",
                "~/Scripts/jquery.vmap.sampledata.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/daterangepicker.js",
                "~/Libs/Gentelella/js/custom.min.js",
                "~/Scripts/echarts.min.js"

                //"~/Scripts/pnotify/pnotify.js",
                //"~/Scripts/pnotify/pnotify.buttons.js",
                //"~/Scripts/pnotify/pnotify.nonblock.js"

                ));
        }
    }
}
