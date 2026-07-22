using System.Collections.Generic;
using SavourSA_Project.Models;

namespace SavourSA_Project.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalRecipes { get; set; }

        public int TotalCategories { get; set; }

        public int TotalFavourites { get; set; }

        public List<Recipe> RecentRecipes { get; set; }

        public List<CategorySummary> CategorySummary { get; set; }
        public List<ApplicationUser> LatestUsers { get; set; }

        public List<Recipe> LatestRecipes { get; set; }
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; }

        public int RecipeCount { get; set; }
    }
}