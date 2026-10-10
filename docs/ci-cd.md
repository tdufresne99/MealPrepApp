# Continuous integration and delivery

## Current implementation

The repository currently contains product and architecture documentation. The `CI` workflow runs on pushes to all branches, pull requests targeting `main`, and manual dispatch. It uses a GitHub-hosted Ubuntu runner; the owner's computer is not a CI runner.

The `Documentation` job uses the standard `markdownlint-cli2` action to check Markdown formatting in the root README and under `docs/`. Rules cover headings, lists, code fences, spacing and other Markdown conventions. The small repository configuration allows long prose and HTML line breaks in tables. Link-target existence and Mermaid rendering are not checked by this workflow.

There is no custom documentation checker or test suite to maintain. Workflow action dependencies are pinned to full commit hashes. Dependabot proposes weekly action updates; those updates still require review. Token permissions are read-only and checkout does not persist credentials. Superseded runs on the same branch or pull request are cancelled.

## Run the same checks locally

Use a Node.js version supported by the linter, then run the same linter version bundled with the pinned action:

```text
npx --yes markdownlint-cli2@0.23.1 "README.md" "docs/**/*.md"
```

These commands run from the repository root. A nonzero exit status means the check failed. GitHub execution is verified only after the workflow has been committed, pushed and run in the repository Actions tab.

## Main branch protection

After the first successful GitHub run, the owner can configure a branch rule for `main` requiring the `Documentation` status check before merging a pull request. The workflow itself does not configure repository settings or prevent direct pushes. Rules depend on repository settings and the available GitHub plan.

## Add application checks when scaffolding exists

1. Backend: restore dependencies, build .NET projects and run domain/application/architecture tests.
2. Database integration: start a disposable PostgreSQL service and validate mappings, migrations, ownership, concurrency and idempotency.
3. Client: install locked dependencies, check translations, build Angular and run relevant tests.
4. Packaging: build the portable API image after checks pass; publish immutable versions only from trusted release paths.
5. iOS: add a macOS build path once signing and native dependencies are validated.

Do not add placeholder build jobs that pass without finding application projects. Introduce each check with the implementation it actually validates.

## Delivery and deployment gates

Automatic deployment is not enabled because no executable prototype or deployment target configuration exists yet. Development, Staging and Production remain separate. Later delivery should promote the same tested image, apply controlled compatible migrations, perform readiness checks and provide a rollback procedure.

Before adding deployment to the owner's computer, decide its reachable access method, deployment account, credentials, availability, backup location and release approval policy. Pull-request checks must not receive deployment credentials or execute on the personal host. Select a deployment mechanism separately from CI so a future hosting provider can replace it without changing business code.

## References

- [GitHub Actions workflow syntax](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax)
- [GitHub Actions secure use](https://docs.github.com/en/actions/reference/security/secure-use)
- [Dependabot options](https://docs.github.com/en/code-security/reference/supply-chain-security/dependabot-options-reference)
- [markdownlint-cli2 action](https://github.com/DavidAnson/markdownlint-cli2-action)
