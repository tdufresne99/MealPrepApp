using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        // Register this module's use cases and adapters when they are implemented.
        return services;
    }
}
