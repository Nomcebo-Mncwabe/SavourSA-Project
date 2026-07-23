using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int Score { get; set; } // 1-5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
