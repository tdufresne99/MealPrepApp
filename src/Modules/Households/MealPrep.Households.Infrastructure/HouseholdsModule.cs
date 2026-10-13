using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Households.Infrastructure;

public static class HouseholdsModule
{
    public static IServiceCollection AddHouseholdsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        // Register this module's use cases and adapters when they are implemented.
        return services;
    }
}
