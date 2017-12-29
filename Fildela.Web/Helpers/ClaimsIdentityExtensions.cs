using Fildela.Resources;
using Fildela.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Fildela.Web.Helpers
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetAccountOwnerEmailShort()
        {
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;

            if (HttpContext.Current.User.IsInRole("AccountOwner"))
                return StringExtensions.FirstCharToUpperRemoveTextAfterAtSign(HttpContext.Current.User.Identity.Name);
            else
                return StringExtensions.FirstCharToUpperRemoveTextAfterAtSign(claimsIdentity.Claims.Where(c => c.Type == "AccountOwnerEmail").Select(c => c.Value).SingleOrDefault());
        }

        public static ClaimsIdentityModel GetCurrentClaimsIdentity()
        {
            int userID = 0;
            string userEmail = string.Empty;
            string userRolesToString = string.Empty;

            int accountOwnerID = 0;
            string accountOwnerEmail = string.Empty;

            string userType = string.Empty;

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)HttpContext.Current.User.Identity;

            userEmail = HttpContext.Current.User.Identity.Name;
            Int32.TryParse(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault(), out userID);
            List<string> userRoles = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            foreach (var item in userRoles)
            {
                userRolesToString += item;
            }

            if (HttpContext.Current.User.IsInRole("AccountOwner"))
            {
                accountOwnerEmail = userEmail;
                accountOwnerID = userID;
                userType = Resource.User;
            }
            else if (HttpContext.Current.User.IsInRole("Guest"))
            {
                userType = Resource.Guest;
                accountOwnerEmail = claimsIdentity.Claims.Where(c => c.Type == "AccountOwnerEmail").Select(c => c.Value).SingleOrDefault();
                Int32.TryParse(claimsIdentity.Claims.Where(c => c.Type == "AccountOwnerID").Select(c => c.Value).SingleOrDefault(), out accountOwnerID);
            }

            ClaimsIdentityModel model = new ClaimsIdentityModel()
            {
                IpAddress = IpAddressExtensions.GetIpAddress(),
                AccountOwnerEmail = accountOwnerEmail,
                AccountOwnerID = accountOwnerID,
                AccountEmail = userEmail,
                AccountID = userID,
                UserRoles = userRolesToString,
                UserType = userType
            };

            return model;
        }
    }
}