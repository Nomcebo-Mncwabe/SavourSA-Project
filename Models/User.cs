using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Navigation
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Follow> Followrs { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public ICollection<Rating> Rating { get; set; } = new List<Rating>();
    }
}