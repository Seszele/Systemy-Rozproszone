using DotNetEnv;
Env.Load();

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (Environment.GetEnvironmentVariable("SPOONACULAR_API_BASE") == null || Environment.GetEnvironmentVariable("SPOONACULAR_API_KEY") == null || Environment.GetEnvironmentVariable("NUTRITIONIX_API_BASE") == null || Environment.GetEnvironmentVariable("NUTRITIONIX_APP_ID") == null || Environment.GetEnvironmentVariable("NUTRITIONIX_APP_KEY") == null)
{
    // environment variables not set, log and exit
    Console.WriteLine("Environment variables not set. Please create .env file and set SPOONACULAR_API_BASE, SPOONACULAR_API_KEY, NUTRITIONIX_API_BASE, NUTRITIONIX_APP_ID, and NUTRITIONIX_APP_KEY.");
    Environment.Exit(1);
}

builder.Services.AddSingleton<IRecipeService>(x => new RecipeService(new HttpClient { BaseAddress = new Uri(Environment.GetEnvironmentVariable("SPOONACULAR_API_BASE")!) }, Environment.GetEnvironmentVariable("SPOONACULAR_API_KEY")!));
builder.Services.AddSingleton<INutritionService>(x => new NutritionService(new HttpClient { BaseAddress = new Uri(Environment.GetEnvironmentVariable("NUTRITIONIX_API_BASE")!) }, Environment.GetEnvironmentVariable("NUTRITIONIX_APP_ID")!, Environment.GetEnvironmentVariable("NUTRITIONIX_APP_KEY")!));


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
