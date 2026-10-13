using MealPrep.Groceries.Infrastructure;
using MealPrep.Households.Infrastructure;
using MealPrep.Identity.Infrastructure;
using MealPrep.MealPlanning.Infrastructure;
using MealPrep.Recipes.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityModule()
    .AddHouseholdsModule()
    .AddRecipesModule()
    .AddMealPlanningModule()
    .AddGroceriesModule();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
