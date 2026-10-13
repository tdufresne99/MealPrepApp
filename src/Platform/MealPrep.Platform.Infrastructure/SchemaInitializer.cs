using System.Text.RegularExpressions;
using Npgsql;

namespace MealPrep.Platform.Infrastructure;

public sealed class SchemaInitializer(IEnumerable<ModuleSchema> schemas, NpgsqlDataSource? database = null)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (database is null)
            throw new InvalidOperationException("Database is not configured.");
        await using var connection = await database.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        // Serialize bootstrap commands in this database. Normal API startup never executes DDL.
        await using (var gate = new NpgsqlCommand("SELECT pg_advisory_xact_lock(68723491003)", connection, transaction))
            await gate.ExecuteNonQueryAsync(cancellationToken);
        foreach (var schema in schemas.OrderBy(s => s.Name).Distinct())
        {
            if (!Regex.IsMatch(schema.Name, "^[a-z][a-z0-9_]*$"))
                throw new InvalidOperationException("Invalid module schema identifier.");
            await using var command = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS \"{schema.Name}\"", connection, transaction);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        await transaction.CommitAsync(cancellationToken);
    }
}
