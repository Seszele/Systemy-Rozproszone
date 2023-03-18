using System.Collections.Generic;
using RecipeAPI.Models;

public interface INutritionService
{
    Task<NutritionInfo> GetNutritionInfo(List<string> allIngredients);
}