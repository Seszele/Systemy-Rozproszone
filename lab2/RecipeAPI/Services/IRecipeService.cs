using System.Collections.Generic;
using RecipeAPI.Models;

public interface IRecipeService
{
    Task<Recipe> GetRecipe(List<string> ingredients);
}