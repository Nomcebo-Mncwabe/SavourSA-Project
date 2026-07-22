using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace SavourSA_Project.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("SavourSAConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Favourite> Favourites { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}