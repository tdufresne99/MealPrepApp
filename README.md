# MealPrepApp

A connected household meal-planning application that generates weekly preparations, allocates portions and leftovers, and derives grocery lists from recipes and user-defined constraints.

## Project status

Phase 1 is in planning. The repository currently contains documentation; the application has not been scaffolded or deployed.

The [CI workflow](.github/workflows/ci.yml) checks documentation on pushes and pull requests. See [CI/CD](docs/ci-cd.md) for local checks, limitations and the planned application/deployment stages.

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

The proposed stack is Angular/Ionic/Capacitor, ASP.NET Core .NET 10 and PostgreSQL, organized as a modular hexagonal monolith. Dependency compatibility and an actual iPhone prototype remain to be validated. No runnable setup instructions are available yet.
