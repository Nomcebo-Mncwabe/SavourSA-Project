using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SavourSA_Project.Models;
using SavourSA_Project;
using System;

namespace SavourSA_Project
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromDays(14),
                SlidingExpiration = true
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Google login — only works once real ClientId/ClientSecret are added to Web.config
            var googleClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            var googleClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"];
            if (!string.IsNullOrEmpty(googleClientId))
            {
                app.UseGoogleAuthentication(new Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions
                {
                    ClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"],
                    ClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"]
                });
            }
        }
    }
}