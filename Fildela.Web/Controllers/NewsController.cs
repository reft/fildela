using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fildela.Web.Helpers;
using Fildela.Web.Models.NewsModels;
using Fildela.Resources;
using Fildela.Business.Domains.News;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Category.Models;
using Fildela.Business.Domains.Category;
using Fildela.Business.Domains.News.Models;
using System.Configuration;

namespace Fildela.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly INewsService _newsService;
        private readonly IAdministrationService _administrationService;
        private readonly ICategoryService _categoryService;

        public NewsController(INewsService newsService, IAdministrationService administrationService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _administrationService = administrationService;
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index(Models.NewsModels.NewsModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.News;

            List<CategoryModel> newsCategories = _categoryService.GetNewsCategoriesFromCacheOrDB().ToList();

            if (model.OrderByCategory == null && model.OrderByDate == null && model.OrderByPublisher == null && model.OrderByTitle == null)
                model.OrderByDate = 0;

            NewsViewModel newsViewModel = new NewsViewModel()
            {
                Categories = newsCategories,
                CategoryID = model.CategoryID,
                NewsID = model.NewsID,
                NewsTitle = model.NewsTitle,
                OrderByCategory = model.OrderByCategory,
                OrderByDate = model.OrderByDate,
                OrderByPublisher = model.OrderByPublisher,
                OrderByTitle = model.OrderByTitle
            };

            HttpCookie getCookie = Request.Cookies["Table"];

            //Return table display format
            if (model.Table == null && getCookie != null && getCookie.Value == "True")
            {
                NewsPagedListDTOModel newsResultDTO = _newsService.GetNews(10, model.NewsID, model.Page, model.NewsTitle, model.CategoryID, model.OrderByTitle, model.OrderByPublisher, model.OrderByDate, model.OrderByCategory);

                //Get full news content if a specific news has been choosen
                if (model.NewsID != null && newsResultDTO.NewsDTOModel.Count() == 1)
                    newsResultDTO.NewsDTOModel.FirstOrDefault().Text = _newsService.GetNewsBlob(newsResultDTO.NewsDTOModel.FirstOrDefault().TextBlobName);

                newsViewModel.NewsPagedListDTO = newsResultDTO;

                return View("Table", newsViewModel);
            }
            else
            {
                if (model.Table != null && model.Table == true)
                {
                    NewsPagedListDTOModel newsResultDTO = _newsService.GetNews(10, model.NewsID, model.Page, model.NewsTitle, model.CategoryID, model.OrderByTitle, model.OrderByPublisher, model.OrderByDate, model.OrderByCategory);

                    //Get full news content if a specific news has been choosen
                    if (model.NewsID != null && newsResultDTO.NewsDTOModel.Count() == 1)
                        newsResultDTO.NewsDTOModel.FirstOrDefault().Text = _newsService.GetNewsBlob(newsResultDTO.NewsDTOModel.FirstOrDefault().TextBlobName);

                    newsViewModel.NewsPagedListDTO = newsResultDTO;

                    //Add cookie
                    CookieExtensions.createCookie("Table", 7, "True", true);

                    //Return table display format
                    return View("Table", newsViewModel);
                }
                else
                {
                    NewsPagedListDTOModel newsResultDTO = _newsService.GetNews(4, model.NewsID, model.Page, model.NewsTitle, model.CategoryID, model.OrderByTitle, model.OrderByPublisher, model.OrderByDate, model.OrderByCategory);

                    //Get full news content if a specific news has been choosen
                    if (model.NewsID != null && newsResultDTO.NewsDTOModel.Count() == 1)
                        newsResultDTO.NewsDTOModel.FirstOrDefault().Text = _newsService.GetNewsBlob(newsResultDTO.NewsDTOModel.FirstOrDefault().TextBlobName);

                    newsViewModel.NewsPagedListDTO = newsResultDTO;

                    //Remove cookie
                    if (Request.Cookies["Table"] != null)
                        Response.Cookies["Table"].Expires = DateTime.Now.AddDays(-7);

                    //Return standard display format
                    return View("Default", newsViewModel);
                }
            }
        }
    }
}
