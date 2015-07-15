using System.Web;
using System.Web.Optimization;

namespace SunLive
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                       
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryDependents").Include(
                         "~/Scripts/bootstrap.js"
                        , "~/Scripts/jcrop/jquery.Jcrop.js"
                        , "~/Scripts/croppic/croppic.js"
                        , "~/Scripts/Site/jquery.imagesloaded.js"
                        , "~/Scripts/Site/jquery.fancybox.js"
                        , "~/Scripts/Site/Site.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css"
                ,"~/Content/bootstrap-responsive.css"
                , "~/Content/Style.css"
                ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/Resources").IncludeDirectory("~/Resources","*.js", true));
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/angular-cookie.js",
                "~/Scripts/angular-loader.js",
                "~/Scripts/angular-resource.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-animate.js",
                "~/Scripts/highstock.js",
                "~/Scripts/highcharts-ng.js",
                "~/Scripts/loading-bar.js"
                ));

        }
    }
}