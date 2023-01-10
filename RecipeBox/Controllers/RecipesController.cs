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
                          // .Include(recipe => recipe.Tag)
                          .ToList();
            return View(userRecipes);
        }

        // [Authorize]
        public ActionResult Create()
        {
            ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Name");
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

        [HttpGet]
        public ActionResult Details(int id)
        {
            Recipe thisRecipe = _db.Recipes.FirstOrDefault(rec => rec.RecipeId == id);
            List<RecipeTag> thisJoins = _db.RecipeTags
              .Where(rectag => rectag.RecipeId == id).ToList();
            List<Tag> thisTags = new List<Tag> { };
            foreach (RecipeTag rectag in thisJoins)
            {
                Tag joinTag = _db.Tags.FirstOrDefault(tag => tag.TagId == rectag.TagId);
                thisTags.Add(joinTag);
            }
            ViewBag.tags = thisTags;
            // .Include(recipe => recipe.)
            // .Include(item => item.JoinEntities)
            // .ThenInclude(join => join.Tag)
            // .FirstOrDefault(item => item.ItemId == id);
            return View(thisRecipe);
        }

        [HttpGet("/Recipes/AddTag/{id}")]
        public ActionResult AddTag(int id)
        {
            Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Name");
            return View(thisRecipe);
        }

        [HttpPost("/Recipes/AddTag/{tagId}")]
        public ActionResult AddTag(Recipe recipe, int tagId)
        {
#nullable enable
            RecipeTag? joinEntity = _db.RecipeTags.FirstOrDefault(join => (join.TagId == tagId && join.RecipeId == recipe.RecipeId));
#nullable disable
            if (joinEntity == null && tagId != 0)
            {
                RecipeTag newJoin = new RecipeTag();
                newJoin.RecipeId = recipe.RecipeId;
                newJoin.TagId = tagId;
                _db.RecipeTags.Add(newJoin);
                // _db.RecipeTags.Add(new RecipeTag() { RecipeId = recipeId = tag.TagId });
                _db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = recipe.RecipeId });
        }

        public ActionResult Delete(int id)
        {
            Recipe thisItem = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            return View(thisItem);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            _db.Recipes.Remove(thisRecipe);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteJoin(int joinId)
        {
            RecipeTag joinEntry = _db.RecipeTags.FirstOrDefault(entry => entry.RecipeTagId == joinId);
            _db.RecipeTags.Remove(joinEntry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Recipe thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "Name");
            return View(thisRecipe);
        }

        [HttpPost]
        public ActionResult Edit(Recipe recipe)
        {
            _db.Recipes.Update(recipe);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }




    }
}