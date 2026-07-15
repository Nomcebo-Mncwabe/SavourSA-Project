using System;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class Favourite
    {
        public int FavouriteId { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        public int RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }

        public DateTime DateSaved { get; set; } = DateTime.Now;
    }
}