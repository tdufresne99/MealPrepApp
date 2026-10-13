using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Groceries.Infrastructure;

public static class GroceriesModule
{
    public static IServiceCollection AddGroceriesModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        // Register this module's use cases and adapters when they are implemented.
        return services;
    }
}
