using MealPrep.Platform.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Households.Infrastructure;

public static class HouseholdsModule
{
    public static IServiceCollection AddHouseholdsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new ModuleSchema("households"));
        return services;
    }
}
