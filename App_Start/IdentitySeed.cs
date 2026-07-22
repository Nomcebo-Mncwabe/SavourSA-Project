using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SavourSA_Project.Models;

namespace SavourSA_Project.App_Start
{
    public static class IdentitySeed
    {
        public static void SeedRoles(ApplicationDbContext context)
        {
            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            if (!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }
        }
    }
}