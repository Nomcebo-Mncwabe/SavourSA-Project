using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

        public Category()
        {
            Recipes = new HashSet<Recipe>();
        }
    }
}