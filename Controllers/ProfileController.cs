using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SavourSA_Project.Models;
using SavourSA_Project.Models.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        // ==========================================
        // VIEW CURRENT USER PROFILE
        // GET: /Profile
        // ==========================================
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var recipes = await db.Recipes
                .Where(r => r.UserId == currentUserId)
                .Include(r => r.Category)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            var favouriteCount = await db.Favourites
                .CountAsync(f => f.UserId == currentUserId);

            var model = new ProfileViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Province = user.Province,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                RecipeCount = recipes.Count,
                FavouriteCount = favouriteCount,
                Recipes = recipes
            };

            return View(model);
        }

        // ==========================================
        // VIEW ONLY MY RECIPES
        // GET: /Profile/MyRecipes
        // ==========================================
        public async Task<ActionResult> MyRecipes()
        {
            var currentUserId = User.Identity.GetUserId();

            var recipes = await db.Recipes
                .Where(r => r.UserId == currentUserId)
                .Include(r => r.Category)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return View(recipes);
        }

        // ==========================================
        // DISPLAY EDIT PROFILE PAGE
        // GET: /Profile/Edit
        // ==========================================
        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            var currentUserId = User.Identity.GetUserId();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Province = user.Province,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        // ==========================================
        // UPDATE PROFILE
        // POST: /Profile/Edit
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = User.Identity.GetUserId();

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var usernameExists = await db.Users
                .AnyAsync(u =>
                    u.UserName == model.Username &&
                    u.Id != currentUserId);

            if (usernameExists)
            {
                ModelState.AddModelError(
                    "Username",
                    "That username is already in use."
                );

                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.Username;
            user.PhoneNumber = model.PhoneNumber;
            user.Province = model.Province;
            user.Bio = model.Bio;
            user.ProfilePictureUrl = model.ProfilePictureUrl;

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ==========================================
        // PROFILE STATISTICS
        // GET: /Profile/Statistics
        // ==========================================
        [HttpGet]
        public async Task<JsonResult> Statistics()
        {
            var currentUserId = User.Identity.GetUserId();

            var recipeCount = await db.Recipes
                .CountAsync(r => r.UserId == currentUserId);

            var favouriteCount = await db.Favourites
                .CountAsync(f => f.UserId == currentUserId);

            return Json(
                new
                {
                    recipes = recipeCount,
                    favourites = favouriteCount
                },
                JsonRequestBehavior.AllowGet
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}