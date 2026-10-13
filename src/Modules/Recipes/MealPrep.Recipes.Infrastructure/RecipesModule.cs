using MealPrep.Platform.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Recipes.Infrastructure;

public static class RecipesModule
{
    public static IServiceCollection AddRecipesModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new ModuleSchema("recipes"));
        return services;
    }
}
