using MealPrep.Platform.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPrep.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton(new ModuleSchema("identity"));
        return services;
    }
}
