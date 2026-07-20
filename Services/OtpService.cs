using Microsoft.AspNet.Identity;
using SavourSA_Project.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SavourSA_Project.Services
{
    public static class OtpService
    {
        private const int ExpiryMinutes = 10;
        private const int MaxAttempts = 5;

        public static string GenerateOtp()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                int value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
                return (value % 1000000).ToString("D6"); // always 6 digits, e.g. 042317
            }
        }

        public static string HashOtp(string otp)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(otp));
                return Convert.ToBase64String(bytes);
            }
        }

        public static async Task<PasswordResetOtp> CreateOtpAsync(ApplicationDbContext db, string userId, string email)
        {
            // Invalidate any previous unused OTPs for this user (only one active code at a time)
            var existing = db.PasswordResetOtps.Where(o => o.UserId == userId && !o.Used);
            foreach (var o in existing) o.Used = true;

            var code = GenerateOtp();
            var entry = new PasswordResetOtp
            {
                UserId = userId,
                Email = email,
                OtpCode = HashOtp(code),
                ExpiresAt = DateTime.UtcNow.AddMinutes(ExpiryMinutes),
                Used = false,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            };
            db.PasswordResetOtps.Add(entry);
            await db.SaveChangesAsync();

            return entry; // caller reads `code` separately to email it — never persisted in plain text
        }

        public static async Task<(bool Success, string Error)> VerifyOtpAsync(ApplicationDbContext db, string userId, string submittedOtp)
        {
            var entry = db.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.Used)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (entry == null)
                return (false, "No active verification code found. Please request a new one.");

            if (entry.ExpiresAt < DateTime.UtcNow)
                return (false, "This code has expired. Please request a new one.");

            if (entry.Attempts >= MaxAttempts)
                return (false, "Too many incorrect attempts. Please request a new code.");

            if (entry.OtpCode != HashOtp(submittedOtp))
            {
                entry.Attempts++;
                await db.SaveChangesAsync();
                return (false, $"Incorrect code. {MaxAttempts - entry.Attempts} attempt(s) remaining.");
            }

            return (true, null); // valid — do not mark Used yet, that happens after password reset succeeds
        }

        public static async Task MarkUsedAsync(ApplicationDbContext db, string userId)
        {
            var entry = db.PasswordResetOtps
                .Where(o => o.UserId == userId && !o.Used)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
            if (entry != null)
            {
                entry.Used = true;
                await db.SaveChangesAsync();
            }
        }
    }
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return SendEmailAsync(message.Destination, message.Subject, message.Body);
        }

        public static Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            return Task.Run(() =>
            {
                var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
                var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
                var smtpPass = ConfigurationManager.AppSettings["SmtpPass"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;
                    client.Timeout = 15000; // 15 seconds

                    var mail = new MailMessage
                    {
                        From = new MailAddress(smtpUser, "SavourSA"),
                        Subject = subject,
                        Body = htmlBody,
                        IsBodyHtml = true
                    };
                    mail.To.Add(toEmail);

                    client.Send(mail); // synchronous — SmtpClient's async methods are unreliable in ASP.NET
                }
            });
        }

        public static string BuildVerificationEmail(string firstName, string confirmUrl)
        {
            return $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto;'>
                <div style='background:#4B5D2A;padding:20px;text-align:center;'>
                    <h2 style='color:#fff;margin:0;'>🍲 SavourSA</h2>
                </div>
                <div style='padding:24px;background:#FDF9F3;'>
                    <h3>Hi {firstName},</h3>
                    <p>Thanks for joining SavourSA — the home of South African recipes. Please confirm your email address to activate your account.</p>
                    <p style='text-align:center;margin:30px 0;'>
                        <a href='{confirmUrl}' style='background:#D2601F;color:#fff;padding:12px 28px;border-radius:6px;text-decoration:none;font-weight:bold;'>Verify My Email</a>
                    </p>
                    <p style='font-size:13px;color:#777;'>If the button doesn't work, copy this link into your browser:<br>{confirmUrl}</p>
                    <p style='font-size:13px;color:#777;'>This link expires in 24 hours. If you didn't create this account, you can ignore this email.</p>
                </div>
            </div>";
        }
        public static string BuildOtpEmail(string firstName, string otpCode)
        {
            return $@"
    <div style='font-family:Segoe UI,Arial,sans-serif;max-width:480px;margin:auto;'>
        <div style='background:#4B5D2A;padding:20px;text-align:center;'>
            <h2 style='color:#fff;margin:0;'>🍲 SavourSA</h2>
        </div>
        <div style='padding:24px;background:#FDF9F3;'>
            <h3>Hi {firstName},</h3>
            <p>We received a request to reset your SavourSA password. Use the code below to continue:</p>
            <p style='text-align:center;margin:30px 0;'>
                <span style='font-size:32px;letter-spacing:8px;font-weight:bold;background:#fff;border:1px solid #E0D8C8;padding:14px 20px;border-radius:8px;display:inline-block;'>{otpCode}</span>
            </p>
            <p style='font-size:13px;color:#777;'>This code expires in 10 minutes and can only be used once. If you didn't request this, you can safely ignore this email — your password will not change.</p>
        </div>
    </div>";
        }
    }



}