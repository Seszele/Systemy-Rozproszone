namespace RecipeAPI.Models
{
    public class Recipe
    {
        public string Title { get; set; } = "No title";
        public string Ingredients { get; set; } = "No ingredients";
        public string ImgUrl { get; set; } = "No image";
        public string SourceUrl { get; set; } = "No source";
        public List<string> AllIngredients { get; set; } = new List<string>();
        public NutritionInfo NutritionInfo { get; internal set; } = new NutritionInfo();
    }
}