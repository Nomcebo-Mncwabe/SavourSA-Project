using Microsoft.AspNet.Identity;
using SavourSA_Project.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    [Authorize]
    public class FavouriteController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        // ==========================================
        // VIEW MY FAVOURITE RECIPES
        // GET: /Favourite
        // ==========================================
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();

            var favourites = db.Favourites
                .Where(f => f.UserId == currentUserId)
                .Include(f => f.Recipe)
                .Include(f => f.Recipe.Category)
                .Include(f => f.Recipe.User)
                .OrderByDescending(f => f.DateSaved);

            return View(await favourites.ToListAsync());
        }

        // ==========================================
        // SAVE RECIPE TO FAVOURITES
        // POST: /Favourite/Add/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(int recipeId)
        {
            var currentUserId = User.Identity.GetUserId();

            var recipeExists = await db.Recipes
                .AnyAsync(r => r.RecipeId == recipeId);

            if (!recipeExists)
            {
                return HttpNotFound();
            }

            var alreadySaved = await db.Favourites
                .AnyAsync(f =>
                    f.UserId == currentUserId &&
                    f.RecipeId == recipeId);

            if (!alreadySaved)
            {
                var favourite = new Favourite
                {
                    UserId = currentUserId,
                    RecipeId = recipeId
                };

                db.Favourites.Add(favourite);
                await db.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "Recipe",
                new { id = recipeId }
            );
        }

        // ==========================================
        // REMOVE RECIPE FROM FAVOURITES
        // POST: /Favourite/Remove/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Remove(int recipeId)
        {
            var currentUserId = User.Identity.GetUserId();

            var favourite = await db.Favourites
                .FirstOrDefaultAsync(f =>
                    f.UserId == currentUserId &&
                    f.RecipeId == recipeId);

            if (favourite == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.NotFound
                );
            }

            db.Favourites.Remove(favourite);
            await db.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Recipe",
                new { id = recipeId }
            );
        }

        // ==========================================
        // CHECK IF RECIPE IS ALREADY FAVOURITED
        // GET: /Favourite/Status/5
        // ==========================================
        [HttpGet]
        public async Task<JsonResult> Status(int recipeId)
        {
            var currentUserId = User.Identity.GetUserId();

            var isFavourite = await db.Favourites
                .AnyAsync(f =>
                    f.UserId == currentUserId &&
                    f.RecipeId == recipeId);

            return Json(
                new
                {
                    recipeId,
                    isFavourite
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // ==========================================
        // COUNT CURRENT USER'S FAVOURITES
        // GET: /Favourite/Count
        // ==========================================
        [HttpGet]
        public async Task<JsonResult> Count()
        {
            var currentUserId = User.Identity.GetUserId();

            var total = await db.Favourites
                .CountAsync(f => f.UserId == currentUserId);

            return Json(
                new
                {
                    count = total
                },
                JsonRequestBehavior.AllowGet
            );
        }

        // ==========================================
        // RELEASE DATABASE CONNECTION
        // ==========================================
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