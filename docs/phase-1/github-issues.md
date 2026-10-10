# Prototype GitHub issues

The 25 prototype stories were created as GitHub issues on 10 October 2026. Each issue contains its acceptance criteria, decision gates, suggested branch and links to prerequisite issues. GitHub is the source for current issue and pull request status; US-01 has an implementation in [pull request #28](https://github.com/tdufresne99/MealPrepApp/pull/28).

| Story | GitHub issue |
| --- | --- |
| US-01 | [#3 Buildable backend and health endpoint](https://github.com/tdufresne99/MealPrepApp/issues/3) |
| US-02 | [#4 Enforced hexagonal module boundaries](https://github.com/tdufresne99/MealPrepApp/issues/4) |
| US-03 | [#5 Portable PostgreSQL and environment startup](https://github.com/tdufresne99/MealPrepApp/issues/5) |
| US-04 | [#6 Bilingual Angular mobile foundation](https://github.com/tdufresne99/MealPrepApp/issues/6) |
| US-05 | [#7 First physical iPhone API connection](https://github.com/tdufresne99/MealPrepApp/issues/7) |
| US-06 | [#8 Authenticate seeded users](https://github.com/tdufresne99/MealPrepApp/issues/8) |
| US-07 | [#9 Household access and permissions](https://github.com/tdufresne99/MealPrepApp/issues/9) |
| US-08 | [#10 Ingredient and unit reference data](https://github.com/tdufresne99/MealPrepApp/issues/10) |
| US-09 | [#11 Create and view household recipes](https://github.com/tdufresne99/MealPrepApp/issues/11) |
| US-10 | [#12 Recipe labels for planning](https://github.com/tdufresne99/MealPrepApp/issues/12) |
| US-11 | [#13 Safely edit shared recipes](https://github.com/tdufresne99/MealPrepApp/issues/13) |
| US-12 | [#14 Agreed planning rules and examples](https://github.com/tdufresne99/MealPrepApp/issues/14) |
| US-13 | [#15 Preparations and allocation invariants](https://github.com/tdufresne99/MealPrepApp/issues/15) |
| US-14 | [#16 Eligible recipes and constraint input](https://github.com/tdufresne99/MealPrepApp/issues/16) |
| US-15 | [#17 Generate a valid constrained candidate](https://github.com/tdufresne99/MealPrepApp/issues/17) |
| US-16 | [#18 Bounded generation and failure outcomes](https://github.com/tdufresne99/MealPrepApp/issues/18) |
| US-17 | [#19 Review and save a plan with snapshots](https://github.com/tdufresne99/MealPrepApp/issues/19) |
| US-18 | [#20 Shared plan viewing and version conflicts](https://github.com/tdufresne99/MealPrepApp/issues/20) |
| US-19 | [#21 Calculate grocery totals from preparations](https://github.com/tdufresne99/MealPrepApp/issues/21) |
| US-20 | [#22 Save and view household grocery lists](https://github.com/tdufresne99/MealPrepApp/issues/22) |
| US-21 | [#23 Retry saved-plan creation without duplicates](https://github.com/tdufresne99/MealPrepApp/issues/23) |
| US-22 | [#24 Validate the full bilingual phone journey](https://github.com/tdufresne99/MealPrepApp/issues/24) |
| US-23 | [#25 Back up and restore persistent data](https://github.com/tdufresne99/MealPrepApp/issues/25) |
| US-24 | [#26 Rehearse relocation to another host](https://github.com/tdufresne99/MealPrepApp/issues/26) |
| US-25 | [#27 Release the household pilot](https://github.com/tdufresne99/MealPrepApp/issues/27) |

The story ID and the GitHub issue number are different identifiers. Use the issue number when closing an issue from a pull request, for example `Closes #3` for US-01.

## GitHub Project setup

GitHub Projects management is not exposed by the current connector. A [setup script](../../scripts/Setup-GitHubProject.ps1) creates or reuses a project named `MealPrepApp Prototype`, links it to the repository and adds these stories without adding duplicate items.

Install [GitHub CLI](https://cli.github.com/) and authenticate with an account able to manage the project. Grant the `project` scope, then run from the repository root:

```powershell
gh auth login
gh auth refresh --scopes project
./scripts/Setup-GitHubProject.ps1
```

The script requires PowerShell 7 and defaults to `tdufresne99/MealPrepApp`. It fails explicitly if prerequisites or expected issues are missing. Review the GitHub permissions requested during authentication. It does not create implementation branches, modify issue content or change project visibility.

Project creation and script execution have not been completed in this session. The script has been checked for syntax; its GitHub operations still need an authenticated execution.
