using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SavourSA_Project.Models
{
    public class Follow
    {
        
        
            public int FollowerId { get; set; }
        // who follows
        public User Follower { get; set; } = null;
            public int FollowingId { get; set; }
        // who is being followed
        public User Following { get; set; } = null;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
