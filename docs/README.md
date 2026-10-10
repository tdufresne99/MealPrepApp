# Documentation

## Phase 1 reading order

| Document | Purpose |
| --- | --- |
| [Specifications](phase-1/specifications.md) | Product scope, requirements, domain vocabulary and use cases |
| [Architecture](phase-1/architecture.md) | Hexagonal module boundaries, entities and tables, communication contracts, runtime and deployment views, architectural decisions |
| [Prototype plan](phase-1/prototype-plan.md) | Bounded prototype scope, development milestones, decision gates and acceptance checklist |
| [Prototype user stories](phase-1/prototype-user-stories.md) | Ordered story backlog, branch names, prerequisites and pull request acceptance criteria |
| [GitHub issues](phase-1/github-issues.md) | Created issue links and GitHub Project setup |

## Documentation conventions

See [Continuous integration and delivery](ci-cd.md) for the current checks and future deployment gates.

Markdown files in this repository are the maintained documentation baseline. Keep documentation and code in English; French is used only for application translations or relevant user-content examples.

The specifications define product behavior. The architecture explains proposed mechanisms and distinguishes accepted requirements from proposed technology choices. The prototype plan narrows the first demonstrable scope without deleting long-term product requirements. Resolve contradictory decisions explicitly rather than assuming all proposals are accepted.

Diagrams use Mermaid code blocks so their source can be reviewed in Git and rendered by GitHub. Tables, requirement identifiers, use cases and existing ADR identifiers are retained from the Word documents. Proposed names, schemas and API routes do not represent implemented code.

Original Word files remain in the workspace outside this Git repository. They are historical authoring sources and are not a second maintained baseline. PDFs may be generated later for distribution; they are not needed for reviewing the current documentation.

When changing a requirement, update the affected architecture decision and prototype acceptance criteria in the same change. Record whether a decision is accepted, proposed, superseded or unresolved. Do not add developer setup commands until they have been implemented and checked.

## Decisions incorporated during repository integration

- Lunch-friendly quotas count portions allocated to lunch slots only.
- Initial backend hosting is the owner's computer; access and availability details remain open.
- Development, Staging and Production are logically separate environments.
- Hosting portability is an explicit requirement, with relocation/restoration acceptance criteria.
- The current Git repository is the application code root.

## Format references

[GitHub README documentation](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes) describes repository navigation and relative links. [GitHub diagram documentation](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/creating-diagrams) describes Mermaid support in Markdown files.
