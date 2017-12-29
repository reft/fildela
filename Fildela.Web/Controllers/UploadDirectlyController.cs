using System;
using System.Web.Mvc;
using Fildela.Resources;
using Fildela.Web.Helpers;
using Fildela.Web.Models.UploadDirectlyModels;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Account;
using Fildela.Business.Domains.UploadDirectly;
using Fildela.Business.Domains.UploadDirectly.Models;
using System.Configuration;

namespace Fildela.Web.Controllers
{
    public class UploadDirectlyController : Controller
    {
        private readonly string ProductNameWithDomain = ConfigurationManager.AppSettings["ProductNameWithDomain"];

        private readonly IAdministrationService _administrationService;
        private readonly IAccountService _accountService;
        private readonly IUploadDirectlyService _uploadDirectlyService;

        public UploadDirectlyController(IAccountService accountService, IAdministrationService administrationService, IUploadDirectlyService uploadDirectlyService)
        {
            _accountService = accountService;
            _administrationService = administrationService;
            _uploadDirectlyService = uploadDirectlyService;
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public JsonResult GetUploadSASURI(UploadDirectlySASURIViewModel model)
        {
            string message = string.Empty;
            string blobSASURI = string.Empty;
            string blobName = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                string ipAddress = IpAddressExtensions.GetIpAddress();

                UploadDirectlyUsageDTOModel uploadDirectlyUsage = _uploadDirectlyService.GetUploadDirectlyUsage(ipAddress);

                //Validate file count for all recent uploads
                if (uploadDirectlyUsage.FileCount < uploadDirectlyUsage.AllowedTotalFileCount)
                {
                    //Validate bytes for all recent uploads
                    if (uploadDirectlyUsage.StoredBytes + model.FileSize <= uploadDirectlyUsage.AllowedTotalStoredBytes)
                    {
                        blobName = ipAddress.Replace(".", "") + "-" + Fildela.Web.Helpers.GuidExtensions.DateAndGuid();
                        blobSASURI = _uploadDirectlyService.GetUploadSASURI(blobName);

                        success = true;
                    }
                    else
                        message = Resource.You_have_exceeded_the_allowed_stored_bytes;
                }
                else
                    message = Resource.You_have_exceeded_the_allowed_number_of_files;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new { success = success, blobSASURI = blobSASURI, blobName = blobName, message = message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult InsertUploadDirectly(InsertUploadDirectlyViewModel model)
        {
            string message = string.Empty;
            string viewString = string.Empty;
            bool success = false;

            UploadDirectlyCompleteViewModel uploadDirectlyCompleteViewModel = new UploadDirectlyCompleteViewModel();

            if (ModelState.IsValid)
            {
                bool blobExist = _uploadDirectlyService.BlobExist(model.BlobName);

                //Validate if blob was uploaded successfully
                if (blobExist)
                {
                    bool isFileOwnedByAnyone = _uploadDirectlyService.FileOwnedByAnyone(model.BlobName);
                    if (!isFileOwnedByAnyone)
                    {
                        string ipAddress = IpAddressExtensions.GetIpAddress();

                        string authenticatedEmail = null;
                        if (User.Identity.IsAuthenticated)
                            authenticatedEmail = User.Identity.Name;

                        UploadDirectlyUsageDTOModel uploadDirectlyUsage = _uploadDirectlyService.GetUploadDirectlyUsage(ipAddress);
                        UploadDirectlyDTOModel uploadDirectlyBlobModel = _uploadDirectlyService.GetBlobAttributes(model.BlobName);

                        //Validate file count storage
                        if (uploadDirectlyUsage.FileCount < uploadDirectlyUsage.AllowedTotalFileCount)
                        {
                            //Validate file size storage
                            if (uploadDirectlyUsage.StoredBytes + uploadDirectlyBlobModel.FileSize <= uploadDirectlyUsage.AllowedTotalStoredBytes)
                            {
                                string uploadDirectlyRowKey = ipAddress.Replace(".", "") + GuidExtensions.DateAndGuid().ToUpper();
                                string SASURI = _uploadDirectlyService.GetDownloadSASURI(model.BlobName, model.FileName, uploadDirectlyUsage.FileLifetimeInHours);

                                uploadDirectlyCompleteViewModel.DownloadLink = this.Url.Action("DownloadUploadDirectly", "UploadDirectly", new { RowKey = uploadDirectlyRowKey + "/" }, this.Request.Url.Scheme);
                                uploadDirectlyCompleteViewModel.BlobName = model.BlobName;
                                uploadDirectlyCompleteViewModel.StartDate = TimeZoneExtensions.GetCurrentDate();
                                uploadDirectlyCompleteViewModel.EndDate = TimeZoneExtensions.GetCurrentDate().AddHours(uploadDirectlyUsage.FileLifetimeInHours);
                                uploadDirectlyCompleteViewModel.FileName = model.FileName;
                                uploadDirectlyCompleteViewModel.FileSize = uploadDirectlyBlobModel.FileSize;

                                UploadDirectlyModel uploadDirectly = new UploadDirectlyModel(
                                     uploadDirectlyRowKey, ipAddress, uploadDirectlyBlobModel.FileSize, model.BlobName, model.FileName,
                                     SASURI, uploadDirectlyUsage.FileLifetimeInHours);

                                //Insert upload directly, (file meta data)
                                _uploadDirectlyService.InsertUploadDirectly(uploadDirectly);

                                success = true;
                            }
                            else
                            {
                                if (!isFileOwnedByAnyone)
                                    _uploadDirectlyService.DeleteBlob(model.BlobName);

                                message = message = Resource.You_have_exceeded_the_allowed_stored_bytes;
                            }
                        }
                        else
                        {
                            if (!isFileOwnedByAnyone)
                                _uploadDirectlyService.DeleteBlob(model.BlobName);

                            message = Resource.You_have_exceeded_the_allowed_number_of_files;
                        }
                    }
                    else
                        message = Resource.You_have_no_rights_for_that_file;
                }
                else
                    message = Resource.An_unexpected_error_has_occurred_Please_try_again;
            }
            else
                message = Resource.An_unexpected_error_has_occurred_Please_try_again;

            return Json(new
            {
                message = message,
                success = success,
                viewString = RenderViewToStringExtensions.RenderViewToString(this, "~/Views/Shared/_UploadDirectlyCompletePartial.cshtml", uploadDirectlyCompleteViewModel)
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownloadUploadDirectly(DownloadUploadDirectlyModel model)
        {
            ViewBag.Title = ProductNameWithDomain + " - " + Resource.Download_of_file;
            ViewBag.MessageTitle = Resource.Download_of_file;

            if (ModelState.IsValid)
            {
                UploadDirectlyModel uploadDirectly = _uploadDirectlyService.GetUploadDirectlyFromRowKey(model.RowKey);

                if (uploadDirectly != null && !String.IsNullOrEmpty(uploadDirectly.BlobSASURI))
                    return Redirect(uploadDirectly.BlobSASURI);
                else
                {
                    string message = Resource.The_file_could_not_be_downloaded + "<p style='margin-bottom:20px; font-size:16px;'>" + Resource.This_may_be_because_the_file_could_not_be_found_or_the_expiration_date_has_passed + "</p>";

                    ModelState.AddModelError("Error", message);

                    return View("~/Views/Shared/Message.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError("Error", Resource.An_unexpected_error_has_occurred_Please_try_again);

                return View("~/Views/Shared/Message.cshtml");
            }
        }

        [HttpPost]
        [AjaxOnly]
        [AllowAnonymous]
        [ValidateInput(true)]
        public JsonResult SendDownloadLinkToEmail(UploadDirectlyDownloadLinkViewModel model)
        {
            string message = string.Empty;
            bool success = false;

            if (ModelState.IsValid)
            {
                //Validate if blob exists
                bool blobExists = _uploadDirectlyService.BlobExist(model.BlobName);

                if (blobExists)
                {
                    string ipAddress = IpAddressExtensions.GetIpAddress();

                    //Validate send emails
                    UploadDirectlyEmailDTOModel uploadDirectlyEmailmodel = _uploadDirectlyService.GetUploadDirectlyEmailModel(model.BlobName, ipAddress);

                    if (uploadDirectlyEmailmodel.EmailCountForCurrentFile < uploadDirectlyEmailmodel.AllowedEmailCountPerFile)
                    {
                        bool downloadLinkSentToEmail = _uploadDirectlyService.DownloadLinkSentToEmail(model.Email, model.BlobName);
                        if (!downloadLinkSentToEmail)
                        {
                            string fromEmail = string.Empty;
                            if (User.Identity.IsAuthenticated)
                                fromEmail = User.Identity.Name;

                            _uploadDirectlyService.SendEmailDownloadLink(model.Email, model.FileName, model.FileSize, model.URI, model.StartDate, model.EndDate, fromEmail);

                            string rowKey = ipAddress.Replace(".", "") + GuidExtensions.DateAndGuid();

                            UploadDirectlyModel uploadDirectly = new UploadDirectlyModel(
                                 rowKey, ipAddress, 0, model.BlobName, String.Empty,
                                 String.Empty, 0);
                            uploadDirectly.Email = model.Email.ToLower().Trim();

                            //Insert sent email upload directly
                            _uploadDirectlyService.InsertUploadDirectly(uploadDirectly);

                            message = Resource.Link_has_been_sent_to_the_specified_email_address;
                            success = true;
                        }
                        else
                            message = Resource.You_have_already_sent_a_link_to_the_specified_email_address;
                    }
                    else
                        message = Resource.You_have_exceeded_the_number_of_allowed_email_distribution;
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
    }
}
