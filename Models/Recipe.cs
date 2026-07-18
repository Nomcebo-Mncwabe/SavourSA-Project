using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SavourSA_Project.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        public int PrepTime { get; set; }

        public int CookTime { get; set; }

        public int Servings { get; set; }

        public string ImageUrl { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Foreign Key to Category
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        // Foreign Key to User
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Favourite> Favourites { get; set; }
    }
}