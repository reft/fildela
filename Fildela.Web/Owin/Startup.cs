using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Fildela.Web.SignalR.Startup))]

namespace Fildela.Web.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["AzureServiceBus"].ConnectionString;

            //GlobalHost.DependencyResolver.UseServiceBus(connectionString, "fildela");

            app.MapSignalR();

            ConfigureAuth(app);

            ConfigureExternalProvider(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/SignInRedirect"),
                ExpireTimeSpan = new TimeSpan(0, 2, 0, 0, 0)
            });
        }

        private void ConfigureExternalProvider(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed    
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Logon")
            });

            // Use a cookie to temporarily store information about a user logging in with a              
            // third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // You have to register this app at https://developers.facebook.com/ and get the     
            //appId and appSecret.   
            // Facebook requires SSL, so that need to be enanbled.  Project url can be found  
            // under project properties and can be localhost.

            var facebookAuthenticationOptions = new FacebookAuthenticationOptions()
            {
                AppId = "1525858801060556",
                AppSecret = "add3a567f4e0ff69cfe06f7e98a1bbc4",
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                        return Task.FromResult(true);
                    }
                }
            };
            facebookAuthenticationOptions.Scope.Add("email");


            app.UseFacebookAuthentication(facebookAuthenticationOptions);

            var googleOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "53032843116-tmtqsanl40lf2n7v138uhlrm8k5aa864.apps.googleusercontent.com",
                ClientSecret = "otYl9j4Zc4s-fTOccL48UOIW",
                CallbackPath = new PathString("/signin-google")
            };
            googleOptions.Scope.Add("email");
            googleOptions.Scope.Add("profile");
            app.UseGoogleAuthentication(googleOptions);
        }
    }
}
