using System.Web.Mvc;
using System.Web;
using Fildela.Web.Helpers;
using Fildela.Resources;
using Fildela.Business.Domains.News;
using Fildela.Business.Domains.News.Models;
using System.Configuration;

namespace Fildela.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly INewsService _newsService;

        public HomeController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            HttpCookie getCookie = Request.Cookies["CookieConsent"];
            if (getCookie == null && TempData["IndexMessage"] == null)
            {
                CookieExtensions.createCookie("CookieConsent", 7, "CookieConsent", true);
                TempData["IndexMessage"] = @"<i class=""fa fa-info-circle popup-icon""></i>" +
                    Resource.This_site_uses_cookies + @" <span class=""popup-block""><a href=""/Home/Information"" id=""IndexPopupLink"">" + Resource.Read_more + "</a>.</span>";
            }

            NewsDTOModel newsDTO = _newsService.GetLatestNewsFromCacheOrDB();

            return View(newsDTO);
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Information()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Information;

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult UserAgreement()
        {
            NewsDTOModel newsDTO = _newsService.GetLatestNewsFromCacheOrDB();

            return View(newsDTO);
        }
    }
}