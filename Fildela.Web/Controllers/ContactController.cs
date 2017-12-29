using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fildela.Resources;
using Fildela.Web.Helpers;
using Fildela.Web.Models.ContactModels;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Category;
using Fildela.Business.Domains.Category.Models;
using Fildela.Business.Domains.Administration.Models;
using Fildela.Business.Domains.User;
using System.Configuration;

namespace Fildela.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IAdministrationService _administrationService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public ContactController(IAdministrationService administrationService, ICategoryService categoryService, IUserService userService)
        {
            _administrationService = administrationService;
            _categoryService = categoryService;
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Contact;

            List<CategoryModel> contactCategories = _categoryService.GetContactCategoriesFromCacheOrDB().ToList();

            return View(contactCategories);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(true)]
        public ActionResult Index(ContactViewModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Contact;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                //Validate ipaddress
                bool adminLogExist = _administrationService.AdminLogExist(ipAddress, "Contact");
                if (!adminLogExist)
                {
                    //Get category
                    string category = _categoryService.GetContactCategoryName(model.CategoryID);

                    //Send email
                    _userService.SendEmailContact(model.Name, model.Email, category, model.Message);
                    string rowKey = "contact-" + ipAddress.Replace(".", "") + "-" + GuidExtensions.DateAndGuid();

                    //Insert adminLog
                    AdminLogModel adminLog = new AdminLogModel(
                    rowKey, 0, string.Empty, 0, string.Empty, ipAddress, "Contact");

                    _administrationService.InsertAdminLog(adminLog);

                    TempData["ContactSuccess"] = Resource.The_message_has_been_sent;

                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError("ContactError", Resource.You_have_already_sent_a_message_try_again_later);
            }
            else
                ModelState.AddModelError("ContactError", Resource.An_unexpected_error_has_occurred_Please_try_again);

            List<CategoryModel> contactCategories = _categoryService.GetContactCategoriesFromCacheOrDB().ToList();

            return View("Index", contactCategories);
        }
    }
}
