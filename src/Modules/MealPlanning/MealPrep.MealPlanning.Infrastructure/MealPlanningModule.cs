using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.MealPlanning.Infrastructure;

public static class MealPlanningModule
{
    public static IServiceCollection AddMealPlanningModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        // Register this module's use cases and adapters when they are implemented.
        return services;
    }
}
