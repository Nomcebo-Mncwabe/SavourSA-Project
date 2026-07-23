using System;

namespace SavourSA_Project.Models
{
    public class Favorite
    {
        public int UserId { get; set; }
        public User User { get; set; } = null;
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}