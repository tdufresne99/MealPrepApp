# Portable local infrastructure

US-03 runs the API and PostgreSQL through Docker Compose. Install Docker with Linux containers and Compose, and PowerShell 7 for the helper scripts. Run all commands from the repository root. The same Dockerfile and application code are used across environments; hosting configuration and credentials are supplied at runtime.

## Prepare environment configuration

```powershell
./scripts/Initialize-LocalEnvironment.ps1 -Environment Development
./scripts/Initialize-LocalEnvironment.ps1 -Environment Staging
./scripts/Initialize-LocalEnvironment.ps1 -Environment Production
```

Each invocation creates an ignored environment file and a cryptographically generated password file under `.secrets/<environment>/postgres-password`. Existing files are preserved. Templates under `deploy/` contain configuration only, never passwords. Compose mounts the password as a secret file into the API and PostgreSQL; it is not baked into the image or expanded into Compose environment values.

| Environment | Environment file | Compose project | API port | Database | Optional local database port |
| --- | --- | --- | --- | --- | --- |
| Development | `.env.development` | mealprep-development | 5080 | mealprep_development | 5432 |
| Staging | `.env.staging` | mealprep-staging | 5081 | mealprep_staging | 5433 |
| Production | `.env.production` | mealprep-production | 5082 | mealprep_production | 5434 |

The three projects own separate networks, password files and named volumes. Keep project names distinct. API ports bind to loopback by default; iPhone HTTPS access is addressed in US-05. These are local environment definitions, not a completed public deployment or backup procedure.

## Start Development

```powershell
docker compose --env-file .env.development up -d --wait db
docker compose --env-file .env.development run --rm --build migrate
docker compose --env-file .env.development up -d api
```

Open <http://localhost:5080/health> for process liveness, and <http://localhost:5080/health/ready> for database connectivity. Both return HTTP 200 and `Healthy` when ready. Readiness returns HTTP 503 and `Unhealthy` when PostgreSQL is unavailable or unconfigured. Liveness remains independent of the database. The root `/` has no page and returns 404.

For Staging and Production, substitute `.env.staging` or `.env.production` in the same commands and use the matching port. Normal API startup performs no schema modifications.

## Explicit schema initialization

The `migrate` service executes the same API image with `--migrate` and exits. Each module's Infrastructure registration declares its own schema: `identity`, `households`, `recipes`, `planning` and `groceries`. A narrow Platform.Infrastructure adapter initializes these schemas transactionally with an advisory lock, serializing concurrent bootstrap commands in the database. The command is idempotent and returns a nonzero exit code on failure.

US-03 creates schema namespaces only, with no business tables or EF Core DbContexts. Add each module's EF Core migrations and migration history when its entities are implemented; the current initializer does not pretend to be a versioned EF migration engine. Database privileges currently use the bootstrap account created by the PostgreSQL image. Schema names and project boundaries do not enforce database privileges; distinct migration/runtime roles remain a deployment refinement.

## Persistence and shutdown

```powershell
docker compose --env-file .env.development up -d --no-deps --force-recreate api
docker compose --env-file .env.development down
```

These operations retain the project's `postgres_data` named volume. PostgreSQL 18 stores version-specific data beneath `/var/lib/postgresql`, which is the mounted path. Avoid `down --volumes` for environments whose data must be retained. A named volume persists on its Docker host; moving hosts also requires backup and restore, covered by later stories.

Changing the password file on an existing volume does not change the database account's password: PostgreSQL initialization settings apply to an empty data directory. Coordinate account password changes with the API secret. Do not delete a volume to rotate credentials.

## Visual Studio and Postman

The default Compose file does not publish PostgreSQL. For local debugging of the API from Visual Studio, explicitly enable loopback database access:

```powershell
docker compose --env-file .env.development -f compose.yaml -f compose.local.yaml up -d --wait db
$env:Database__Host = 'localhost'
$env:Database__Port = '5432'
$env:Database__Name = 'mealprep_development'
$env:Database__Username = 'mealprep'
$env:Database__PasswordFile = (Resolve-Path .secrets/development/postgres-password).Path
dotnet run --project src/MealPrep.Api -- --urls http://127.0.0.1:5090
```

The environment variables apply to this terminal and processes started from it. Visual Studio requires the same configuration in its launch environment or .NET user secrets. Keep passwords out of committed launch settings. Postman uses GET `/health/ready` at the running API address, without authentication; phones never connect directly to PostgreSQL.

External hosts can supply `ConnectionStrings__MealPrep` instead of the individual `Database__*` settings. This takes precedence and supports provider-specific PostgreSQL TLS options without changing business code. Use secure runtime configuration for that value. Docker Compose secret files are local file mounts, not an encrypted secret-management service.

## Automated acceptance checks

```powershell
./scripts/Test-Us03Compose.ps1
```

The script builds the API image and starts three temporary projects named `mealprep-us03-<run-id>-<environment>` on ports 18080–18082. It checks schema initialization twice, healthy responses, separate environment data, persistence after API recreation, a database outage (503 readiness with 200 liveness), recovery and persistence after database recreation. Its probe table belongs only to disposable test databases. Use `-BasePort` to select other test ports.

The script cleans up only projects and volumes created by that invocation. It does not remove normal Development, Staging or Production data. The CI `Portable infrastructure` job runs the same script with fresh environment files. Image tags pin the SDK version and PostgreSQL/runtime major versions; patch tags may advance. PostgreSQL major upgrades and production image promotion require explicit review and later release procedures.

## References

- [Docker Compose secrets](https://docs.docker.com/compose/how-tos/use-secrets/)
- [PostgreSQL official image and data paths](https://hub.docker.com/_/postgres)
- [Npgsql data sources](https://www.npgsql.org/doc/basic-usage.html)
- [.NET Docker images](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)
