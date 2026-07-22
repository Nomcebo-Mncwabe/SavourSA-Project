using System.Linq;
using System.Web.Mvc;
using SavourSA_Project.Models;

namespace SavourSA_Project.Controllers
{
    public class GuestController : Controller
    {
        private SavourSAContext db = new SavourSAContext();

        // GET: Guest/Menu
        public ActionResult Menu()
        {
            var recipes = db.Recipes.OrderByDescending(r => r.DateCreated).ToList();
            return View(recipes);
        }

        // GET: Guest/RecipeDetails/5
        public ActionResult RecipeDetails(int id)
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.RecipeId == id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
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