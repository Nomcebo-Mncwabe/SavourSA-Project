using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SavourSA_Project;
using SavourSA_Project.Models;
using SavourSA_Project.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _userManager = value; }
        }

        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            set { _signInManager = value; }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Provinces = SAProvinces.All;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Provinces = SAProvinces.All;

            if (!ModelState.IsValid)
                return View(model);

            // Extra server-side check: username/email uniqueness feedback (good UX — error prevention)
            if (await UserManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "That username is already taken.");
                return View(model);
            }
            if (await UserManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            string profilePicUrl = null;
            if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
            {
                if (model.ProfilePicture.ContentLength > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfilePicture", "Image must be under 2MB.");
                    return View(model);
                }
                var ext = System.IO.Path.GetExtension(model.ProfilePicture.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    ModelState.AddModelError("ProfilePicture", "Only JPG or PNG files are allowed.");
                    return View(model);
                }
                var fileName = Guid.NewGuid() + ext;
                var path = Server.MapPath("~/Content/ProfilePictures/" + fileName);
                model.ProfilePicture.SaveAs(path);
                profilePicUrl = "/Content/ProfilePictures/" + fileName;
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Province = model.Province,
                ProfilePictureUrl = profilePicUrl,
                SubscribeNewsletter = model.SubscribeNewsletter
            };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate email confirmation token and send verification email
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                var body = EmailService.BuildVerificationEmail(user.FirstName, confirmUrl);
                await EmailService.SendEmailAsync(user.Email, "Verify your SavourSA account", body);

                return RedirectToAction("VerifyEmailSent", new { email = user.Email });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        // GET: /Account/VerifyEmailSent
        [AllowAnonymous]
        public ActionResult VerifyEmailSent(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmailSuccess" : "Error");
        }

        // POST: resend verification email
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResendVerification(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null && !user.EmailConfirmed)
            {
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var body = EmailService.BuildVerificationEmail(user.FirstName, confirmUrl);
                await EmailService.SendEmailAsync(user.Email, "Verify your SavourSA account", body);
            }
            ViewBag.Email = email;
            ViewBag.Resent = true;
            return View("VerifyEmailSent");
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Allow login via email OR username
            ApplicationUser user;
            if (model.EmailOrUsername.Contains("@"))
                user = await UserManager.FindByEmailAsync(model.EmailOrUsername);
            else
                user = await UserManager.FindByNameAsync(model.EmailOrUsername);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                ModelState.AddModelError("", "Please verify your email before logging in. Check your inbox for the verification link.");
                ViewBag.ShowResend = true;
                ViewBag.UnverifiedEmail = user.Email;
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: true);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Account locked due to multiple failed attempts. Try again later.");
                    return View(model);
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        private ApplicationDbContext _db;
        public ApplicationDbContext Db => _db ?? (_db = ApplicationDbContext.Create());

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user != null && await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                await CreateAndSendOtpAsync(user);
            }

            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }

        // GET: /Account/VerifyOtp
        [AllowAnonymous]
        public ActionResult VerifyOtp(string email)
        {
            return View(new VerifyOtpViewModel { Email = email });
        }

        // POST: /Account/VerifyOtp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid request. Please start over.");
                return View(model);
            }

            var (success, error) = await SavourSA_Project.Services.OtpService.VerifyOtpAsync(Db, user.Id, model.Otp);
            if (!success)
            {
                ModelState.AddModelError("Otp", error);
                return View(model);
            }

            return RedirectToAction("ResetPassword", new { email = model.Email, otp = model.Otp });
        }

        // POST: /Account/ResendOtp
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ResendOtp(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                await CreateAndSendOtpAsync(user);
            }
            ViewBag.Resent = true;
            return View("VerifyOtp", new VerifyOtpViewModel { Email = email });
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string email, string otp)
        {
            return View(new ResetPasswordViewModel { Email = email, Otp = otp });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid request. Please start over.");
                return View(model);
            }

            var (success, error) = await SavourSA_Project.Services.OtpService.VerifyOtpAsync(Db, user.Id, model.Otp);
            if (!success)
            {
                ModelState.AddModelError("", error);
                return View(model);
            }

            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

            if (result.Succeeded)
            {
                await SavourSA_Project.Services.OtpService.MarkUsedAsync(Db, user.Id);
                return RedirectToAction("ResetPasswordSuccess");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err);
            return View(model);
        }

        // GET: /Account/ResetPasswordSuccess
        [AllowAnonymous]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        private async Task<string> CreateAndSendOtpAsync(ApplicationUser user)
        {
            var code = SavourSA_Project.Services.OtpService.GenerateOtp();

            var existing = Db.PasswordResetOtps.Where(o => o.UserId == user.Id && !o.Used);
            foreach (var o in existing) o.Used = true;

            Db.PasswordResetOtps.Add(new PasswordResetOtp
            {
                UserId = user.Id,
                Email = user.Email,
                OtpCode = SavourSA_Project.Services.OtpService.HashOtp(code),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                Used = false,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            });
            await Db.SaveChangesAsync();

            var body = SavourSA_Project.Services.EmailService.BuildOtpEmail(user.FirstName, code);
            await SavourSA_Project.Services.EmailService.SendEmailAsync(user.Email, "Your SavourSA password reset code", body);

            return code;
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        // Google external login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null) return RedirectToAction("Login");

            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            if (result == SignInStatus.Success)
                return RedirectToLocal(returnUrl);

            // First-time Google login — create account automatically, email already verified by Google
            var user = new ApplicationUser
            {
                UserName = loginInfo.Email,
                Email = loginInfo.Email,
                EmailConfirmed = true,
                FirstName = loginInfo.ExternalIdentity.FindFirstValue(System.Security.Claims.ClaimTypes.GivenName),
                LastName = loginInfo.ExternalIdentity.FindFirstValue(System.Security.Claims.ClaimTypes.Surname)
            };
            var createResult = await UserManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToLocal(returnUrl);
            }
            return RedirectToAction("Login");
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
    internal class ChallengeResult : HttpUnauthorizedResult
    {
        public const string XsrfKey = "XsrfId";
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new Microsoft.Owin.Security.AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}