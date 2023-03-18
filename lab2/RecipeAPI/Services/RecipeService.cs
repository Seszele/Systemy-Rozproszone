using System.Collections.Generic;
using System.Linq;
using RecipeAPI.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class RecipeService : IRecipeService
{
    private readonly HttpClient client;
    private readonly string apiKey;
    public RecipeService(HttpClient httpClient, string apiKey)
    {
        client = httpClient;
        this.apiKey = apiKey;
    }
    public async Task<Recipe> GetRecipe(List<string> ingredients)
    {
        string ingredientsString = string.Join(",", ingredients);
        string apiUrl = $"https://api.spoonacular.com/recipes/findByIngredients?apiKey={apiKey}&ingredients={ingredientsString}&number=1";
        Console.WriteLine($"apiUrl: {apiUrl}");

        HttpResponseMessage response = await client.GetAsync(apiUrl);
        string content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            JArray recipes = JArray.Parse(content);
            JObject firstRecipe = recipes.FirstOrDefault() as JObject;

            if (firstRecipe != null)
            {
                return new Recipe
                {
                    Title = firstRecipe.Value<string>("title"),
                    SourceUrl = $"https://spoonacular.com/recipe/{firstRecipe.Value<string>("title").Replace(" ", "-")}-{firstRecipe.Value<int>("id")}",
                    Ingredients = ingredientsString,
                    ImgUrl = firstRecipe.Value<string>("image"),
                    // "original" field from "usedIngredients" turned into a list of strings + the same for missedIngredients
                    AllIngredients = firstRecipe.Value<JArray>("usedIngredients").Select(x => x.Value<string>("original")).Union(firstRecipe.Value<JArray>("missedIngredients").Select(x => x.Value<string>("original"))).ToList()
                };
            }
        }

        return null;
    }
}
