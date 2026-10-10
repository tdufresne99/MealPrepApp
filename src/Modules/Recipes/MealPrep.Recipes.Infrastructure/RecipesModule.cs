using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Recipes.Infrastructure;

public static class RecipesModule
{
    public static IServiceCollection AddRecipesModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        // Register this module's use cases and adapters when they are implemented.
        return services;
    }
}
