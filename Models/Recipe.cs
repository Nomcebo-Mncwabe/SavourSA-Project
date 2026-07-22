using System;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(250)]
        public string ShortDescription { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; } // e.g. "Main", "Dessert", "Snack"

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Who posted it (nullable if you want guest-created recipes too — otherwise required)
        public string CreatedByUserId { get; set; }
    }
}