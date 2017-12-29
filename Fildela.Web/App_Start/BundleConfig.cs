using System.Web.Optimization;

namespace Fildela.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //---------Styles---------//

            string[] styles = new string[]
            {
                "~/Content/metro-bootstrap.css",
                "~/Content/css/select2.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/font-awesome.css",
                "~/Content/formValidation.css",
                "~/Content/animate.css"
            };

            string[] customStyles = new string[]
            {
                "~/Content/fildela-spinner.css",
                "~/Content/fildela-all.css",
                "~/Content/fildela-modals.css",
                "~/Content/fildela-news.css",
                "~/Content/fildela-contact.css",
                "~/Content/fildela-account.css",
                "~/Content/fildela-administration.css",
            };

            bundles.Add(new StyleBundle("~/Content/Styles")
                .Include(styles)
                .Include(customStyles));

            //---------Scripts before pageload---------//

            string[] modernizr = new string[]
            {
                "~/Scripts/modernizr-*"
            };

            //---------Scripts after pageload---------//

            string[] jQuery = new string[]
            {
                "~/Scripts/jquery-{version}.js"
            };

            string[] scripts = new string[]
            {
                "~/Scripts/jquery.signalR-2.2.0.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.highlight.js",
                "~/Scripts/formValidation/js/formValidation.js",
                "~/Scripts/formValidation/js/framework/bootstrap.js",
                "~/Scripts/formValidation/js/language/sv_SE.js",
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js",
                "~/Scripts/select2.js",
                "~/Scripts/Select2-locales/select2_locale_sv.js",
                "~/Scripts/knockout-3.3.0.js",
                "~/Scripts/bootstrap-notify.js",
                "~/Scripts/highcharts.js"
            };

            string[] customScripts = new string[]
            {
                "~/Scripts/fildela-all.js",
                "~/Scripts/fildela-shared-modals.js",
                "~/Scripts/fildela-input-file.js",
                "~/Scripts/fildela-upload-directly.js",
                "~/Scripts/fildela-news.js",
                "~/Scripts/fildela-account-all.js",
                "~/Scripts/fildela-account-files.js",
                "~/Scripts/fildela-account-files-upload.js",
                "~/Scripts/fildela-account-guestaccounts.js",
                "~/Scripts/fildela-account-logs.js",
                "~/Scripts/fildela-account-settings.js",
                "~/Scripts/fildela-account-links.js",
                "~/Scripts/fildela-signalr-activeusers.js",
                "~/Scripts/fildela-SortedTable.js",
                "~/Scripts/fildela-SortedTableEvent.js",
                "~/Scripts/fildela-account-searchtable.js",
                "~/Scripts/fildela-contact.js",
                "~/Scripts/fildela-administration-all.js",
                "~/Scripts/fildela-administration-news.js",
                "~/Scripts/fildela-external-authentication.js"
            };

            bundles.Add(new ScriptBundle("~/bundles/Modernizr")
                 .Include(modernizr));

            bundles.Add(new ScriptBundle("~/bundles/Scripts")
                .Include(jQuery)
                .Include(scripts)
                .Include(customScripts));
        }
    }
}