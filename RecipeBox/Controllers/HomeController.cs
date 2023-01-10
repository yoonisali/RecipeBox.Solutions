using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{
    public class HomeController : Controller
    {
      private readonly RecipeBoxContext _db;
      private readonly UserManager<ApplicationUser> _userManager;

      public HomeController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
      {
        _userManager = userManager;
        _db = db;
      }

      [HttpGet("/")]
      public async Task<ActionResult> Index()
      {
        // User Logic
        string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

        // Recipe Logic
        Dictionary<string, object[]> model = new Dictionary<string, object[]>();
        if (currentUser != null) {
          RecipeTag[] recipes = _db.RecipeTags
            .Where(rec => rec.RecipeId.ToString() == currentUser.Id)
            .ToArray();
          model.Add("Recipes", recipes);
        }

        // Tag Logic
        Tag[] tags = _db.Tags.ToArray();
        model.Add("Tags", tags);

        return View(model);
      }
    }
}