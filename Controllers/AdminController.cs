using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using SavourSA_Project.Models;
using SavourSA_Project.Models.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;

namespace SavourSA_Project.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ================= DASHBOARD =================
        public ActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                TotalUsers = db.Users.Count(),

                TotalRecipes = db.Recipes.Count(),

                TotalCategories = db.Categories.Count(),

                TotalFavourites = db.Favourites.Count(),

                RecentRecipes = db.Recipes
                                  .OrderByDescending(r => r.DateCreated)
                                  .Take(5)
                                  .ToList(),

                CategorySummary = db.Categories
                                    .Select(c => new CategorySummary
                                    {
                                        CategoryName = c.Name,
                                        RecipeCount = c.Recipes.Count()
                                    })
                                    .ToList()
            };

            return View(model);
        }

        // ================= RECIPES =================
        public ActionResult Recipes(string search)
        {
            var recipes = db.Recipes
                            .Include(r => r.Category)
                            .Include(r => r.User)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                recipes = recipes.Where(r => r.Title.Contains(search));
            }

            return View(recipes.OrderByDescending(r => r.DateCreated).ToList());
        }
        // Download Report as PDF
        public FileResult DownloadReport()
        {
            using (var memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 40, 40, 40, 40);

                PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                Font heading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                Font subHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font normal = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                document.Add(new Paragraph("SavourSA Administrator Report", heading));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Generated: " + DateTime.Now.ToString("dd MMMM yyyy HH:mm"), normal));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("-----------------------------------------------------"));

                document.Add(new Paragraph("Total Members: " + db.Users.Count(), normal));
                document.Add(new Paragraph("Total Recipes: " + db.Recipes.Count(), normal));
                document.Add(new Paragraph("Total Categories: " + db.Categories.Count(), normal));
                document.Add(new Paragraph("Total Favourites: " + db.Favourites.Count(), normal));

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Latest Members", subHeading));

                var latestUsers = db.Users
                                    .OrderByDescending(u => u.CreatedAt)
                                    .Take(5)
                                    .ToList();

                if (latestUsers.Any())
                {
                    foreach (var user in latestUsers)
                    {
                        document.Add(new Paragraph(
                            user.FirstName + " " + user.LastName +
                            " (" + user.Email + ")", normal));
                    }
                }
                else
                {
                    document.Add(new Paragraph("No registered members.", normal));
                }

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Latest Recipes", subHeading));

                var latestRecipes = db.Recipes
                                      .OrderByDescending(r => r.DateCreated)
                                      .Take(5)
                                      .ToList();

                if (latestRecipes.Any())
                {
                    foreach (var recipe in latestRecipes)
                    {
                        document.Add(new Paragraph(recipe.Title, normal));
                    }
                }
                else
                {
                    document.Add(new Paragraph("No recipes uploaded yet.", normal));
                }

                document.Close();

                return File(
                    memoryStream.ToArray(),
                    "application/pdf",
                    "SavourSA_Report.pdf");
            }
        }

        // ================= VIEW RECIPE =================

        public ActionResult ViewRecipe(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var recipe = db.Recipes
                           .Include(r => r.Category)
                           .Include(r => r.User)
                           .FirstOrDefault(r => r.RecipeId == id);

            if (recipe == null)
                return HttpNotFound();

            return View(recipe);
        }

        // ================= EDIT RECIPE =================

        public ActionResult EditRecipe(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var recipe = db.Recipes.Find(id);

            if (recipe == null)
                return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", recipe.CategoryId);

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRecipe(Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Recipe updated successfully.";

                return RedirectToAction("Recipes");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", recipe.CategoryId);

            return View(recipe);
        }

        // ================= DELETE RECIPE =================

        public ActionResult DeleteRecipe(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var recipe = db.Recipes
                           .Include(r => r.Category)
                           .FirstOrDefault(r => r.RecipeId == id);

            if (recipe == null)
                return HttpNotFound();

            return View(recipe);
        }

        [HttpPost, ActionName("DeleteRecipe")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRecipeConfirmed(int id)
        {
            var recipe = db.Recipes.Find(id);

            db.Recipes.Remove(recipe);

            db.SaveChanges();

            TempData["Success"] = "Recipe deleted successfully.";

            return RedirectToAction("Recipes");
        }

        // ================= CATEGORIES =================

        // Display all categories
        public ActionResult Categories()
        {
            var categories = db.Categories
                               .OrderBy(c => c.Name)
                               .ToList();

            return View(categories);
        }

        // ================= CREATE =================

        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();

                TempData["Success"] = "Category added successfully.";

                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // ================= EDIT =================

        public ActionResult EditCategory(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Category category = db.Categories.Find(id);

            if (category == null)
                return HttpNotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Category updated successfully.";

                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // ================= DELETE =================

        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
                return HttpNotFound();

            Category category = db.Categories.Find(id);

            if (category == null)
                return HttpNotFound();

            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            Category category = db.Categories.Find(id);

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["Success"] = "Category deleted successfully.";

            return RedirectToAction("Categories");
        }

        // ================= USERS =================

        public ActionResult Users(string search)
        {
            var users = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search));
            }

            return View(users.OrderBy(u => u.FirstName).ToList());
        }

        //================ VIEW USER ===================

        public ActionResult ViewUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        //================ DELETE USER ===================

        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(string id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction("Users");
        }

        // ================= REPORTS =================

        public ActionResult Reports()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                TotalUsers = db.Users.Count(),

                TotalRecipes = db.Recipes.Count(),

                TotalCategories = db.Categories.Count(),

                TotalFavourites = db.Favourites.Count(),

                LatestUsers = db.Users
                                .OrderByDescending(u => u.CreatedAt)
                                .Take(5)
                                .ToList(),

                LatestRecipes = db.Recipes
                                  .OrderByDescending(r => r.DateCreated)
                                  .Take(5)
                                  .ToList(),
                CategorySummary = db.Categories
                    .Select(c => new CategorySummary
                    {
                        CategoryName = c.Name,
                        RecipeCount = c.Recipes.Count()
                    })
                    .ToList()


            };

            return View(model);
        }

        // ================= COMMENTS =================

        public ActionResult Comments()
        {
            return View();
        }

        // ================= SETTINGS =================

        public ActionResult Settings()
        {
            return View();
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