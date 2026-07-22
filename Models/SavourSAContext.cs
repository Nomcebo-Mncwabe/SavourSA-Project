using System.Data.Entity;

namespace SavourSA_Project.Models
{
    public class SavourSAContext : DbContext
    {
        public SavourSAContext() : base("SavourSAContext")
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
    }
}