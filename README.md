# MealPrepApp

A connected household meal-planning application that generates weekly preparations, allocates portions and leftovers, and derives grocery lists from recipes and user-defined constraints.

## Project status

US-01 introduces the minimal ASP.NET Core API host and its health endpoint. Business modules, authentication, persistence and the mobile client remain to be implemented.

The [CI workflow](.github/workflows/ci.yml) checks documentation and builds the backend on pushes and pull requests. See [CI/CD](docs/ci-cd.md) for local checks and planned deployment stages.

## Run the backend

Install the .NET SDK **10.0.400**, pinned in `global.json`. From the repository root:

```sh
dotnet restore MealPrepApp.slnx
dotnet build MealPrepApp.slnx --configuration Release --no-restore
dotnet run --project src/MealPrep.Api --configuration Release --no-build -- --urls http://127.0.0.1:5080
```

Open <http://127.0.0.1:5080/health>. Expect HTTP `200` and the plain-text body `Healthy`, without authentication or a database. Stop the host with Ctrl+C.

This endpoint checks that the host can respond; it does not yet check database readiness. The example address is local development configuration. Hosting addresses can be set with `--urls` or `ASPNETCORE_URLS`; no hosting provider or credentials are embedded in the application.

## Documentation

Start with the [documentation index](docs/README.md).

- [Specifications](docs/phase-1/specifications.md): product requirements and open decisions.
- [Architecture](docs/phase-1/architecture.md): module boundaries, entities, owned tables, ports, deployment and ADRs.
- [Prototype plan](docs/phase-1/prototype-plan.md): milestones, dependencies, acceptance criteria and user decision gates.
- [Prototype user stories](docs/phase-1/prototype-user-stories.md): one-story branches and pull request sequence.

## Confirmed direction

- Connected operation with individually authenticated household members.
- French and English application content; English documentation and code.
- Separate recipes, preparations and planned meals.
- Lunch quotas count lunch-slot portions only.
- Portable backend hosting, initially on the owner's computer.
- Separate Development, Staging and Production data/configuration.

The proposed stack is Angular/Ionic/Capacitor, ASP.NET Core .NET 10 and PostgreSQL, organized as a modular hexagonal monolith. Client dependency compatibility and an actual iPhone prototype remain to be validated. The API host is the composition root; module boundaries will be introduced in subsequent stories.
