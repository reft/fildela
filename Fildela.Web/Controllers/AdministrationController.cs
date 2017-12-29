using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Fildela.Web.Models;
using Fildela.Web.Helpers;
using Fildela.Resources;
using Fildela.Web.Models.AdministrationModels;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.News;
using Fildela.Business.Domains.Administration.Models;
using Fildela.Business.Domains.Category.Models;
using Fildela.Business.Domains.Category;
using Fildela.Business.Domains.News.Models;
using System.Configuration;

namespace Fildela.Web.Controllers
{
    [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Support", "Publisher" })]
    public class AdministrationController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly INewsService _newsService;
        private readonly IAdministrationService _administrationService;
        private readonly ICategoryService _categoryService;

        public AdministrationController(IAdministrationService administrationService, INewsService newsService, ICategoryService categoryService)
        {
            _administrationService = administrationService;
            _newsService = newsService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Users");
            else if (User.IsInRole("Publisher"))
                return RedirectToAction("News");
            else if (User.IsInRole("Support"))
                return RedirectToAction("Email");
            else
                return RedirectToAction("Forbidden", "Error");
        }

        [HttpGet]
        [AuthorizeClaim(ClaimTypes.Role, "Admin")]
        public ActionResult Users()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Administration;

            AdministrationAccountsDTOModel usersDTO = _administrationService.GetUsersAdministration();

            AdministrationAccountsViewModel administrationUsersViewModel = new AdministrationAccountsViewModel()
            {
                EmailCount = usersDTO.EmailCount,
                NewsCount = usersDTO.NewsCount,
                UserCount = usersDTO.UserCount,
                Accounts = usersDTO.Accounts.ToList()
            };

            return View(administrationUsersViewModel);
        }

        [HttpGet]
        [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Publisher" })]
        public ViewResult News()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Administration;

            AdministrationNewsDTOModel newsDTO = _administrationService.GetNewsAdministration();

            List<CategoryModel> newsCategories = _categoryService.GetNewsCategoriesFromCacheOrDB().ToList();

            AdministrationNewsViewModel administrationNewsViewModel = new AdministrationNewsViewModel()
            {
                EmailCount = newsDTO.EmailCount,
                NewsCount = newsDTO.NewsCount,
                UserCount = newsDTO.UserCount,
                News = newsDTO.News.ToList(),
                Categories = newsCategories.ToList()
            };

            return View(administrationNewsViewModel);
        }

        [HttpGet]
        [AjaxOnly]
        [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Publisher" })]
        public PartialViewResult UpdateNewsContent()
        {
            AdministrationNewsDTOModel newsDTO = _administrationService.GetNewsAdministration();

            List<CategoryModel> newsCategories = _categoryService.GetNewsCategoriesFromCacheOrDB().ToList();

            AdministrationNewsViewModel administrationNewsViewModel = new AdministrationNewsViewModel()
            {
                EmailCount = newsDTO.EmailCount,
                NewsCount = newsDTO.NewsCount,
                UserCount = newsDTO.UserCount,
                News = newsDTO.News.ToList(),
                Categories = newsCategories.ToList()
            };

            return PartialView("~/Views/Administration/Partials/_NewsContent.cshtml", administrationNewsViewModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Publisher" })]
        public JsonResult InsertNews(InsertNewsViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                if (model.Image.ContentType.ToLower() == "image/jpg" || model.Image.ContentType.ToLower() == "image/jpeg")
                {
                    ClaimsIdentityModel currentUser = ClaimsIdentityExtensions.GetCurrentClaimsIdentity();

                    //Upload text to Azure
                    string textBlobName = _newsService.UploadNewsText(currentUser.AccountEmail, model.Title, model.Text);
                    //Upload image to Azure
                    string imageBlobURL = _newsService.UploadNewsImage(currentUser.AccountEmail, model.Title, ImageExtensions.ResizeImage(model.Image, 727, 291));

                    NewsModel newsDTO = new NewsModel()
                    {
                        TextBlobName = textBlobName,
                        ImageBlobURL = imageBlobURL,
                        PublishedByID = currentUser.AccountID,
                        PublishedByFullName = _administrationService.GetUserFullName(currentUser.AccountID),
                        PublishedByEmail = User.Identity.Name,
                        Title = model.Title.Trim(),
                        DatePublished = TimeZoneExtensions.GetCurrentDate(),
                        CategoryID = model.CategoryID,
                        PreviewText = StringExtensions.ShortenWord(model.Text, 350, 375)
                    };

                    //Insert metadata to SQL
                    _newsService.InsertNews(newsDTO);

                    message = Resource.News_uploaded;
                    success = true;
                }
                else
                    message = Resource.An_unexpected_error_has_occurred_Please_try_again;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success
            });
        }

        [HttpGet]
        [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Support" })]
        public ViewResult Emails()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Administration;

            AdministrationEmailsDTOModel administrationViewModelDTO = _administrationService.GetEmailAdministration();

            AdministrationEmailViewModel administrationViewModel = new AdministrationEmailViewModel()
            {
                EmailCount = administrationViewModelDTO.EmailCount,
                NewsCount = administrationViewModelDTO.NewsCount,
                UserCount = administrationViewModelDTO.UserCount
            };

            return View(administrationViewModel);
        }

        [HttpGet]
        [AuthorizeAnyClaim(ClaimTypes.Role, new String[] { "Admin", "Support" })]
        public ViewResult Logs()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Administration;

            AdministrationLogsDTOModel administrationViewModelDTO = _administrationService.GetLogsAdministration();

            AdministrationLogsViewModel administrationViewModel = new AdministrationLogsViewModel()
            {
                EmailCount = administrationViewModelDTO.EmailCount,
                NewsCount = administrationViewModelDTO.NewsCount,
                UserCount = administrationViewModelDTO.UserCount
            };

            return View(administrationViewModel);
        }
    }
}
