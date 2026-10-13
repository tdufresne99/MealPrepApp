using MealPrep.Groceries.Infrastructure;
using MealPrep.Households.Infrastructure;
using MealPrep.Identity.Infrastructure;
using MealPrep.MealPlanning.Infrastructure;
using MealPrep.Recipes.Infrastructure;
using MealPrep.Platform.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var initializeSchemas = args.Contains("--migrate", StringComparer.Ordinal);
var builder = WebApplication.CreateBuilder(args.Where(a => a != "--migrate").ToArray());

builder.Services
    .AddIdentityModule()
    .AddHouseholdsModule()
    .AddRecipesModule()
    .AddMealPlanningModule()
    .AddGroceriesModule();

var databaseConnection = builder.Configuration.GetConnectionString("MealPrep")
    ?? PostgresPersistence.CreateConnectionString(
        builder.Configuration["Database:Host"], builder.Configuration["Database:Name"],
        builder.Configuration["Database:Username"], builder.Configuration["Database:PasswordFile"],
        builder.Configuration["Database:Password"], builder.Configuration["Database:Port"]);
builder.Services.AddPostgresPersistence(databaseConnection);
builder.Services.AddHealthChecks().AddCheck<PostgresReadinessCheck>("postgres", tags: ["ready"]);

var app = builder.Build();

if (initializeSchemas)
{
    try
    {
        await app.Services.GetRequiredService<SchemaInitializer>().InitializeAsync();
        app.Logger.LogInformation("Module schemas initialized successfully. No business table migrations exist yet.");
    }
    catch
    {
        app.Logger.LogError("Schema initialization failed. Verify database connectivity and configuration.");
        Environment.ExitCode = 1;
    }
    await app.DisposeAsync();
    return;
}

app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") }).AllowAnonymous();

await app.RunAsync();
