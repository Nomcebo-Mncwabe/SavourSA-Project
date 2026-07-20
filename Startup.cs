using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SavourSA_Project.Startup))]
namespace SavourSA_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}