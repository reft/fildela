using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using Fildela.Business.Domains.Account;
using Fildela.Business.Domains.Account.Models;
using Ninject;

namespace Fildela.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CompressFilter());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 404;
                filterContext.Result = new HttpNotFoundResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }

    public class AuthorizePermissionAttribute : AuthorizeAttribute
    {
        [Inject]
        public IAccountService _accountService { get; set; }

        private String[] _permissions { get; set; }

        public AuthorizePermissionAttribute(params String[] permissions)
        {
            _permissions = permissions;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.IsInRole("AccountOwner"))
                base.OnAuthorization(filterContext);
            else
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                List<AccountLinkPermissionModel> accountLinkPermissions = new List<AccountLinkPermissionModel>();

                int accountOwnerID = 0;
                Int32.TryParse(claimsIdentity.Claims.Where(c => c.Type == "AccountOwnerID").Select(c => c.Value).SingleOrDefault(), out accountOwnerID);
                int guestID = 0;
                Int32.TryParse(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault(), out guestID);

                accountLinkPermissions = _accountService.GetAccountLinkPermissions(accountOwnerID, guestID).ToList();

                if (accountLinkPermissions != null)
                {
                    List<string> permissionNames = accountLinkPermissions.Select(m => m.Permission.Name).ToList();
                    int hits = permissionNames.Where(m => _permissions.Contains(m)).Count();

                    if (hits > 0)
                    {
                        base.OnAuthorization(filterContext);
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new EmptyResult();
                        }
                        else
                        {
                            filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {
                            { "action", "AccessDenied" },
                            { "controller", "Account" }});
                        }
                    }
                }
                else
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new EmptyResult();
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {
                            { "action", "AccessDenied" },
                            { "controller", "Account" }});
                    }
                }
            }
        }
    }

    public class AuthorizeClaimAttribute : AuthorizeAttribute
    {
        private string claimType;
        private string claimValue;
        public AuthorizeClaimAttribute(string type, string value)
        {
            this.claimType = type;
            this.claimValue = value;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;
            if (user.HasClaim(claimType, claimValue))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "action", "AccessDenied" },
                    { "controller", "Account" }
                });
            }
        }
    }

    public class AuthorizeNotClaimAttribute : AuthorizeAttribute
    {
        private string claimType;
        private string claimValue;
        public AuthorizeNotClaimAttribute(string type, string value)
        {
            this.claimType = type;
            this.claimValue = value;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;
            if (user.HasClaim(claimType, claimValue))
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                                { "action", "AccessDenied" },
                                { "controller", "Account" }
                });
            }
            else
            {
                base.OnAuthorization(filterContext);
            }
        }
    }

    public class AuthorizeAnyClaimAttribute : AuthorizeAttribute
    {
        private string claimType;
        private String[] claimValues { get; set; }

        public AuthorizeAnyClaimAttribute(string type, params String[] values)
        {
            this.claimValues = values;
            this.claimType = type;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = HttpContext.Current.User as ClaimsPrincipal;

            int authorizeCount = 0;

            foreach (var item in claimValues)
            {
                if (user.HasClaim(claimType, item))
                {
                    authorizeCount++;
                }
            }

            if (authorizeCount > 0)
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "action", "AccessDenied" },
                    { "controller", "Account" }
                }
                );
            }
        }
    }

    public class CompressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}