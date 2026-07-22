using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // Keep Description (our current project uses this)
        [Required]
        public string Description { get; set; }

        // Keep ShortDescription as well
        [StringLength(250)]
        public string ShortDescription { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        public int PrepTime { get; set; }

        public int CookTime { get; set; }

        public int Servings { get; set; }

        public string ImageUrl { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Category relationship
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        // User relationship
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        // Keep compatibility with teammate's property
        public string CreatedByUserId { get; set; }

        public virtual ICollection<Favourite> Favourites { get; set; }
    }
}