using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Models;

namespace RecipeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;
    private readonly INutritionService _nutritionService;
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(IRecipeService recipeService, INutritionService nutritionService, ILogger<RecipeController> logger)
    {
        _recipeService = recipeService;
        _nutritionService = nutritionService;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> GetRecipe([FromQuery] List<string> ingredients)
    {
        if (ingredients.Count <= 0 || ingredients is null)
        {
            return BadRequest("Ingredients list is empty.");
        }
        Recipe recipe = await _recipeService.GetRecipe(ingredients);
        if (recipe is null)
        {
            return NotFound("Recipe not found for given ingredients.");
        }
        var nutritionInfo = await _nutritionService.GetNutritionInfo(recipe.AllIngredients);
        recipe.NutritionInfo = nutritionInfo;

        return View("Views/Recipe/Recipe.cshtml", recipe);
    }
}
