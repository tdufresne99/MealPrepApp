using MealPrep.Platform.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.MealPlanning.Infrastructure;

public static class MealPlanningModule
{
    public static IServiceCollection AddMealPlanningModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new ModuleSchema("planning"));
        return services;
    }
}
