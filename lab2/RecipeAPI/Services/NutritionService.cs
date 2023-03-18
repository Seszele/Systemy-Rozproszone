using RecipeAPI.Models;
using Newtonsoft.Json.Linq;
public class NutritionService : INutritionService
{
    private readonly HttpClient client;
    private readonly string appId;
    private readonly string apiKey;
    public NutritionService(HttpClient httpClient, string appId, string appKey)
    {
        client = httpClient;
        this.appId = appId;
        this.apiKey = appKey;
    }
    public async Task<NutritionInfo> GetNutritionInfo(List<string> allIngredients)
    {
        string ingredientsString = string.Join(", ", allIngredients);
        string apiUrl = "https://trackapi.nutritionix.com/v2/natural/nutrients";
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-app-id", appId);
        client.DefaultRequestHeaders.Add("x-app-key", apiKey);

        var requestBody = new
        {
            query = ingredientsString
        };
        var response = await client.PostAsJsonAsync(apiUrl, requestBody);
        string content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            JObject result = JObject.Parse(content);
            JArray hits = result.Value<JArray>("foods");

            if (hits.Count > 0)
            {
                JObject firstHit = hits.FirstOrDefault() as JObject;
                return new NutritionInfo
                {
                    Calories = firstHit.Value<float>("nf_calories"),
                    Fat = firstHit.Value<float>("nf_total_fat"),
                    Carbohydrates = firstHit.Value<float>("nf_total_carbohydrate"),
                    Protein = firstHit.Value<float>("nf_protein")
                };

            }
        }
        return null;
    }
}