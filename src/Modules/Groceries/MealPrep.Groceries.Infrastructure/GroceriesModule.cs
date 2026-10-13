using MealPrep.Platform.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Groceries.Infrastructure;

public static class GroceriesModule
{
    public static IServiceCollection AddGroceriesModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new ModuleSchema("groceries"));
        return services;
    }
}
