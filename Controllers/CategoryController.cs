using SavourSA_Project.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        // GET: /Category
        public async Task<ActionResult> Index()
        {
            var categories = db.Categories
                .Include(c => c.Recipes)
                .OrderBy(c => c.Name);

            return View(await categories.ToListAsync());
        }

        // GET: /Category/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var category = await db.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // GET: /Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Name,Description")]
            Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            category.Name = category.Name.Trim();

            var categoryExists = await db.Categories
                .AnyAsync(c => c.Name == category.Name);

            if (categoryExists)
            {
                ModelState.AddModelError(
                    "Name",
                    "A category with this name already exists."
                );

                return View(category);
            }

            db.Categories.Add(category);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /Category/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var category = await db.Categories.FindAsync(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            int id,
            [Bind(Include = "Name,Description")]
            Category updatedCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedCategory);
            }

            var existingCategory =
                await db.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return HttpNotFound();
            }

            updatedCategory.Name =
                updatedCategory.Name.Trim();

            var duplicateName = await db.Categories
                .AnyAsync(c =>
                    c.Name == updatedCategory.Name &&
                    c.CategoryId != id);

            if (duplicateName)
            {
                ModelState.AddModelError(
                    "Name",
                    "A category with this name already exists."
                );

                return View(updatedCategory);
            }

            existingCategory.Name =
                updatedCategory.Name;

            existingCategory.Description =
                updatedCategory.Description;

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: /Category/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest);
            }

            var category = await db.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(
            int id)
        {
            var category = await db.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == id);

            if (category == null)
            {
                return HttpNotFound();
            }

            if (category.Recipes != null &&
                category.Recipes.Any())
            {
                ModelState.AddModelError(
                    "",
                    "This category cannot be deleted because recipes are assigned to it."
                );

                return View("Delete", category);
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
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