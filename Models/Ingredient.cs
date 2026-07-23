using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null;
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null;
        public string Quantity { get; set; } = string.Empty; 
    }
}