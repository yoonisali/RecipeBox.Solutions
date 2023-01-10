using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
    public class RecipesController : Controller
    {
      private readonly RecipeBoxContext _db;
      private readonly UserManager<ApplicationUser> _userManager;

      public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<ActionResult> Index()
        {
          string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
            List<Recipe> userRecipes = _db.Recipes
                          .Where(rec => rec.User.Id == currentUser.Id.ToString())
                          .Include(recipe => recipe.Tag)
                          .ToList();
            return View(userRecipes);
        }

        // [Authorize]
        public ActionResult Create ()
        {
          ViewBag.RecipeId = new SelectList(_db.Tags, "RecipeId", "Name");
          return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Recipe recipe, string RecipeId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RecipeId= new SelectList(_db.Recipes, "RecipeId", "Name");
                return View(recipe);
            }
            else
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
                recipe.User = currentUser;
                _db.Recipes.Add(recipe);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            



        }

    }
}