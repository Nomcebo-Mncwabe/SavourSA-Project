using Microsoft.AspNet.Identity;
using SavourSA_Project.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        // ==========================================
        // LIST ALL RECIPES
        // GET: /Recipe
        // ==========================================
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var recipes = db.Recipes
                .Include(r => r.Category)
                .Include(r => r.User)
                .OrderByDescending(r => r.DateCreated);

            return View(await recipes.ToListAsync());
        }

        // ==========================================
        // VIEW RECIPE DETAILS
        // GET: /Recipe/Details/5
        // ==========================================
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var recipe = await db.Recipes
                .Include(r => r.Category)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            return View(recipe);
        }

        // ==========================================
        // DISPLAY CREATE RECIPE PAGE
        // GET: /Recipe/Create
        // ==========================================
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(
                db.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name"
            );

            return View();
        }

        // ==========================================
        // CREATE RECIPE
        // POST: /Recipe/Create
        // ==========================================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include =
                "Title,Description,Ingredients,Instructions," +
                "PrepTime,CookTime,Servings,ImageUrl,CategoryId")]
            Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.UserId = User.Identity.GetUserId();
                recipe.DateCreated = DateTime.Now;

                db.Recipes.Add(recipe);
                await db.SaveChangesAsync();

                return RedirectToAction("Details",
                    new { id = recipe.RecipeId });
            }

            ViewBag.CategoryId = new SelectList(
                db.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name",
                recipe.CategoryId
            );

            return View(recipe);
        }

        // ==========================================
        // DISPLAY EDIT RECIPE PAGE
        // GET: /Recipe/Edit/5
        // ==========================================
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var recipe = await db.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (recipe.UserId != currentUserId &&
                !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden);
            }

            ViewBag.CategoryId = new SelectList(
                db.Categories.OrderBy(c => c.Name),
                "CategoryId",
                "Name",
                recipe.CategoryId
            );

            return View(recipe);
        }

        // ==========================================
        // UPDATE RECIPE
        // POST: /Recipe/Edit/5
        // ==========================================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            int id,
            [Bind(Include =
                "Title,Description,Ingredients,Instructions," +
                "PrepTime,CookTime,Servings,ImageUrl,CategoryId")]
            Recipe updatedRecipe)
        {
            var existingRecipe =
                await db.Recipes.FindAsync(id);

            if (existingRecipe == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (existingRecipe.UserId != currentUserId &&
                !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(
                    db.Categories.OrderBy(c => c.Name),
                    "CategoryId",
                    "Name",
                    updatedRecipe.CategoryId
                );

                return View(updatedRecipe);
            }

            existingRecipe.Title = updatedRecipe.Title;
            existingRecipe.Description = updatedRecipe.Description;
            existingRecipe.Ingredients = updatedRecipe.Ingredients;
            existingRecipe.Instructions = updatedRecipe.Instructions;
            existingRecipe.PrepTime = updatedRecipe.PrepTime;
            existingRecipe.CookTime = updatedRecipe.CookTime;
            existingRecipe.Servings = updatedRecipe.Servings;
            existingRecipe.ImageUrl = updatedRecipe.ImageUrl;
            existingRecipe.CategoryId = updatedRecipe.CategoryId;

            await db.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                new { id = existingRecipe.RecipeId });
        }

        // ==========================================
        // DISPLAY DELETE CONFIRMATION
        // GET: /Recipe/Delete/5
        // ==========================================
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var recipe = await db.Recipes
                .Include(r => r.Category)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (recipe.UserId != currentUserId &&
                !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden);
            }

            return View(recipe);
        }

        // ==========================================
        // DELETE RECIPE
        // POST: /Recipe/Delete/5
        // ==========================================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var recipe = await db.Recipes
                .Include(r => r.Favourites)
                .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (recipe.UserId != currentUserId &&
                !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.Forbidden);
            }

            if (recipe.Favourites != null &&
                recipe.Favourites.Any())
            {
                db.Favourites.RemoveRange(recipe.Favourites);
            }

            db.Recipes.Remove(recipe);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
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