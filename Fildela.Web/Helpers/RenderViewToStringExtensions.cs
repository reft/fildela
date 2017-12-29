using System.IO;
using System.Web.Mvc;

namespace Fildela.Web.Helpers
{
    public static class RenderViewToStringExtensions
    {
        internal static string RenderViewToString(this Controller controller, string viewName, object model)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                controller.ViewData.Model = model;
                ViewContext viewCxt = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }
    }
}