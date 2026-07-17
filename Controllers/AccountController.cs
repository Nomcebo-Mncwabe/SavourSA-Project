using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SavourSA_Project.App_Start;
using SavourSA_Project.Models;
using SavourSA_Project.Models.ViewModels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    public class AccountController : Controller
    {
        // Access the UserManager
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
        }

        // Access the Authentication Manager
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // =========================
        // REGISTER (GET)
        // =========================
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // =========================
        // REGISTER (POST)
        // =========================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Province = model.Province,
                AcceptedPopia = model.AcceptedPopia
            };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var identity = await UserManager.CreateIdentityAsync(
                    user,
                    DefaultAuthenticationTypes.ApplicationCookie
                );

                AuthenticationManager.SignIn(
                    new AuthenticationProperties
                    {
                        IsPersistent = false
                    },
                    identity
                );

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        // =========================
        // LOGOUT
        // =========================
        [Authorize]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}