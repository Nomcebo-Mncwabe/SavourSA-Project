using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null;
        public string Message { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}