using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }
        public string Instructions { get; set; }
        public int CookingTimeMinutes { get; set; } 
        public DifficultyLevel Difficulty { get; set; }
        public string Cuisine { get; set; } 
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public double AverageRating { get; set; } 
        public int TotalRatings { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; } // who posted it

        // Navigation
        public User User { get; set; } = null;
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<Favorite> FavoritedBy { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }

    public enum DifficultyLevel { Easy, Medium, Hard }

}
