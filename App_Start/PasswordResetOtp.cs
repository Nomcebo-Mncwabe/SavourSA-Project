using System;
using System.ComponentModel.DataAnnotations;

namespace SavourSA_Project.Models
{
    public class PasswordResetOtp
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }       // hashed, never stored plain — see note below
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }
        public int Attempts { get; set; }         // for lockout after repeated wrong guesses
        public DateTime CreatedAt { get; set; }
    }
}