using SavourSA_Project.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Province { get; set; }

        public string Bio { get; set; }

        public string ProfilePictureUrl { get; set; }

        public int RecipeCount { get; set; }

        public int FavouriteCount { get; set; }

        public IEnumerable<Recipe> Recipes { get; set; }
    }

    public class EditProfileViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string Province { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        [Display(Name = "Profile Picture URL")]
        public string ProfilePictureUrl { get; set; }
    }
}