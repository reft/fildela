using System.Web.Mvc;
using System.Web.Routing;

namespace Fildela.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //------------------ Storage -----------------

            routes.MapRoute(
            "StorageUploadDirectly",
            "u/{RowKey}",
            new { controller = "UploadDirectly", action = "DownloadUploadDirectly" }
            );

            routes.MapRoute(
            "StorageLinks",
            "l/{RowKey}",
            new { controller = "Account", action = "DownloadLink" }
            );

            //------------------- News -------------------

            routes.MapRoute(
            "NewsPage",
            "News/Page/{page}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsTable",
            "News/Table/{table}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "News",
            "News/{newsID}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsCategory",
            "News/Category/{categoryID}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsOrderByTitle",
            "News/OrderByTitle/{orderByTitle}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsOrderByPublisher",
            "News/OrderByPublisher/{orderByPublisher}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsOrderByDate",
            "News/OrderByDate/{orderByDate}",
            new { controller = "News", action = "Index" }
            );

            routes.MapRoute(
            "NewsOrderByCategory",
            "News/OrderByCategory/{orderByCategory}",
            new { controller = "News", action = "Index" }
            );

            //------------------- Default -------------------
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}