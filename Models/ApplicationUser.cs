using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Favourites = new HashSet<Favourite>();
        }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Province { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public string ProfilePictureUrl { get; set; }

        public bool AcceptedPopia { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Recipe> Recipes { get; set; }

        public virtual ICollection<Favourite> Favourites { get; set; }
    }
}