using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace MealPrep.Platform.Infrastructure;

public static class PostgresPersistence
{
    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services, string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
            services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
        services.AddSingleton<PostgresReadinessCheck>();
        services.AddSingleton<SchemaInitializer>();
        return services;
    }

    public static string? CreateConnectionString(string? host, string? database, string? username,
        string? passwordFile, string? password, string? port)
    {
        if (string.IsNullOrWhiteSpace(host))
            return null;
        if (!string.IsNullOrWhiteSpace(passwordFile))
            password = File.ReadAllText(passwordFile).TrimEnd('\r', '\n');
        if (string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("Database name, username and password are required when Database:Host is configured.");
        return new NpgsqlConnectionStringBuilder
        {
            Host = host, Database = database, Username = username, Password = password,
            Port = string.IsNullOrWhiteSpace(port) ? 5432 : int.Parse(port),
            Timeout = 3, CommandTimeout = 5
        }.ConnectionString;
    }
}

public sealed class PostgresReadinessCheck(NpgsqlDataSource? database = null) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (database is null)
            return HealthCheckResult.Unhealthy("Database is not configured.");
        try
        {
            await using var command = database.CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception) when (exception is NpgsqlException or TimeoutException or OperationCanceledException)
        {
            // Do not expose database addresses, credentials or driver exception details.
            return HealthCheckResult.Unhealthy("Database is unavailable.");
        }
    }
}
