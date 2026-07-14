using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }

        public int TotalRecipes { get; set; }

        public int TotalCategories { get; set; }

        public int TotalComments { get; set; }
    }
}