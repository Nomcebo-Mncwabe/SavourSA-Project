using Microsoft.Owin;
using Owin;
using SavourSA_Project.App_Start;
using SavourSA_Project.Models;

[assembly: OwinStartup(typeof(SavourSA_Project.Startup))]

namespace SavourSA_Project
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(
                ApplicationUserManager.Create);

            app.UseCookieAuthentication(
                new Microsoft.Owin.Security.Cookies
                    .CookieAuthenticationOptions
                {
                    AuthenticationType =
                        Microsoft.AspNet.Identity
                            .DefaultAuthenticationTypes
                            .ApplicationCookie,

                    LoginPath = new PathString("/Account/Login")
                });
        }
    }
}