# Prototype user stories and pull request sequence

Version 0.1 | 10 October 2026

## Purpose and working agreement

This backlog turns the [prototype plan](prototype-plan.md) into individually reviewable branches and pull requests. Product behavior is defined in the [specifications](specifications.md); module boundaries follow the [architecture](architecture.md). Track implementation status in the [GitHub issues](github-issues.md). US-01 has an implementation in [pull request #28](https://github.com/tdufresne99/MealPrepApp/pull/28). The documentation CI already exists and is not a new implementation story.

Technical enablers use a developer or operator persona. User-facing stories use household members or editors. Each story must produce its stated result without requiring an unmerged branch to build or test. The application becomes progressively usable; an intermediate main branch is not automatically a production release.

### One story per branch and pull request

1. Confirm the story scope and resolve its listed decision gates.
2. Merge its prerequisite stories into `main`.
3. Create the suggested branch from the latest `main`.
4. Implement only the story scope, including relevant tests, configuration and documentation.
5. Open a pull request referencing the story ID and showing acceptance evidence.
6. Merge after review and passing applicable CI checks. Squash merge is proposed to keep one readable change per story.
7. Begin the next dependent story from updated `main`.

Do not create all branches at once. Avoid stacked pull requests for this first prototype. Independent stories may be developed separately when their prerequisites are merged. Branch names are suggestions; no branches or pull requests are created by this document.

### Common definition of done

- The story's acceptance criteria are satisfied and demonstrated in the PR.
- The current repository builds with documented compatible tool versions.
- Relevant implemented behavior is covered by meaningful tests; documentation-only changes do not require invented application tests.
- Applicable CI checks pass. Add build/test checks alongside real projects and tests, rather than placeholder jobs.
- Domain and Application code respect context boundaries; no cross-module SQL shortcuts.
- New application-owned visible text is available in French and English. Code and engineering documentation remain English.
- No credentials or personal production data are committed.
- Changed contracts, setup steps and architectural decisions are documented.
- Scope changes or unresolved acceptance rules are brought to the product owner before dependent implementation.

## Ordered backlog

Dependencies are prerequisites, not calendar estimates. Physical-device or host-dependent stories can wait while independent backend work continues.

| ID | Stage | Story | Prerequisites | Suggested branch |
| --- | --- | --- | --- | --- |
| US-01 | Foundations | Buildable backend and health endpoint | None | `chore/us-01-backend-foundation` |
| US-02 | Foundations | Enforced hexagonal module boundaries | US-01 | `chore/us-02-module-boundaries` |
| US-03 | Foundations | Portable PostgreSQL and environment startup | US-02 | `chore/us-03-local-infrastructure` |
| US-04 | Foundations | Bilingual Angular mobile foundation | None | `chore/us-04-mobile-foundation` |
| US-05 | Foundations | First physical iPhone API connection | US-01, US-03, US-04 | `chore/us-05-iphone-connectivity` |
| US-06 | Accounts and recipes | Authenticate seeded users | US-02, US-03, US-04 | `feature/us-06-authentication` |
| US-07 | Accounts and recipes | Household access and permissions | US-06 | `feature/us-07-household-access` |
| US-08 | Accounts and recipes | Ingredient and unit reference data | US-03 | `feature/us-08-ingredient-reference` |
| US-09 | Accounts and recipes | Create and view household recipes | US-07, US-08, US-04 | `feature/us-09-household-recipes` |
| US-10 | Accounts and recipes | Recipe labels for planning | US-09 | `feature/us-10-recipe-labels` |
| US-11 | Accounts and recipes | Safely edit shared recipes | US-09, US-10 | `feature/us-11-recipe-editing` |
| US-12 | Planning | Agreed planning rules and examples | None | `docs/us-12-planning-rules` |
| US-13 | Planning | Preparations and allocation invariants | US-02, US-12 | `feature/us-13-planning-domain` |
| US-14 | Planning | Eligible recipes and constraint input | US-10, US-13 | `feature/us-14-planning-input` |
| US-15 | Planning | Generate a valid constrained candidate | US-14 | `feature/us-15-plan-generator` |
| US-16 | Planning | Bounded generation and failure outcomes | US-15 | `feature/us-16-generation-outcomes` |
| US-17 | Planning | Review and save a plan with snapshots | US-07, US-11, US-16, US-04 | `feature/us-17-plan-review-save` |
| US-18 | Planning | Shared plan viewing and version conflicts | US-17 | `feature/us-18-shared-plans` |
| US-19 | Groceries | Calculate grocery totals from preparations | US-13 | `feature/us-19-grocery-calculation` |
| US-20 | Groceries | Save and view household grocery lists | US-18, US-19 | `feature/us-20-shared-groceries` |
| US-21 | Reliability | Retry saved-plan creation without duplicates | US-17 | `feature/us-21-command-retries` |
| US-22 | Reliability | Validate the full bilingual phone journey | US-05, US-11, US-18, US-20, US-21 | `test/us-22-mobile-journey` |
| US-23 | Operations | Back up and restore persistent data | US-03, US-20, US-21 | `chore/us-23-backup-restore` |
| US-24 | Operations | Rehearse relocation to another host | US-05, US-22, US-23 | `chore/us-24-host-relocation` |
| US-25 | Operations | Release the household pilot | US-22, US-23, US-24 | `chore/us-25-household-pilot` |

## Stage 1 Foundations

### US-01 Buildable backend and health endpoint

**Story:** As a developer, I want a reproducible backend skeleton so that subsequent stories start from a buildable main branch.

**Scope:** ASP.NET Core API host, solution, tool-version configuration, one health endpoint and backend build CI.

**Acceptance criteria:**

- A clean checkout restores and builds the API using the documented SDK.
- The running API returns a successful health response without requiring authentication or a database.
- CI builds the real backend project; it does not claim application tests exist yet.
- API configuration contains no credentials or fixed hosting-provider address.

**Gate:** Validate installed .NET tooling and select compatible pinned versions.

### US-02 Enforced hexagonal module boundaries

**Story:** As a developer, I want explicit context boundaries so that business rules stay independent of transport, persistence and other modules.

**Scope:** Domain/Application/Infrastructure structure for the five contexts, narrow published Contracts where needed, composition registration and architecture checks.

**Acceptance criteria:**

- The API host composes module registrations and remains buildable.
- Domain projects have no HTTP, EF Core or other-context domain dependencies.
- Cross-context adapters may reference published Contracts, not provider Infrastructure or DbContext types.
- Architecture checks reject forbidden references and run in CI.
- No speculative repository or application service is added solely to fill an empty layer.

### US-03 Portable PostgreSQL and environment startup

**Story:** As an operator, I want repeatable container startup so that the backend can run on my computer and later move to another host.

**Scope:** API image, PostgreSQL, durable storage, module persistence registration, controlled migration entry point and environment configuration templates.

**Acceptance criteria:**

- Documented commands start the API and PostgreSQL from a clean environment.
- PostgreSQL is reachable by the backend, not exposed as the phone data interface.
- Development, Staging and Production select distinct configuration and data.
- Recreating the API container does not delete persistent database data.
- Readiness identifies database unavailability; secrets are supplied externally.

**Gate:** Decide practical local ports, environment isolation and available Docker tooling. Add module schema migrations when their entities are actually implemented.

### US-04 Bilingual Angular mobile foundation

**Story:** As a household user, I want a mobile interface available in French and English so that I can use my preferred language.

**Scope:** Angular/Ionic/Capacitor shell, minimal navigation, language resources, API address configuration and real client build CI.

**Acceptance criteria:**

- A clean checkout installs locked dependencies and builds the client.
- The shell switches between French and English with no untranslated application-owned text.
- API addresses are environment configuration rather than hard-coded provider URLs.
- CI builds the client and checks the implemented translation resources.

**Gate:** Validate Angular/Ionic/Capacitor compatibility, regional locales and switching behavior.

### US-05 First physical iPhone API connection

**Story:** As a household user, I want the application on my iPhone to reach the backend so that the mobile delivery path is proven early.

**Scope:** Signed device build, minimal connection screen and selected HTTPS access path.

**Acceptance criteria:**

- The app is installed on the physical iPhone and calls the health endpoint.
- The selected network path and trusted certificate work on that device.
- A connection failure produces a clear French or English message.
- Repeatable build/install instructions and actual limitations are recorded.

**Gate:** Borrowed Mac availability, signing/distribution route and Wi-Fi-only versus remote access. US-06 onward can proceed while device access is pending.

## Stage 2 Accounts and recipes

### US-06 Authenticate seeded users

**Story:** As a household user, I want to sign in with my own account so that the server recognizes my identity.

**Scope:** Selected account/session implementation, seeded test accounts, login screen, session expiration and logout. Self-service registration is deferred.

**Acceptance criteria:**

- Two individually seeded users can authenticate through the client.
- Invalid credentials and expired sessions produce translated recoverable outcomes.
- The server derives user identity from authentication, not a request user ID.
- Session storage follows the agreed mobile design; passwords are not stored for automatic login.
- Tests cover successful, failed and expired authentication paths.

**Gate:** Accept the authentication/session and device credential-storage decision before coding it.

### US-07 Household access and permissions

**Story:** As a household member, I want to access my household's resources according to my role so that shared data remains private.

**Scope:** Seeded households and membership, current-household selection, access query contracts and confirmed Member/Editor/Administrator capabilities.

**Acceptance criteria:**

- Both initial users see their shared household.
- Another seeded household's users cannot read or modify its resources.
- The server enforces capability checks independently of visible buttons.
- Removing a membership invalidates subsequent protected operations.
- Household administration rights do not imply public-catalogue administration rights.

**Gate:** Confirm the permissions exercised by the prototype. Invitation and administration screens remain deferred.

### US-08 Ingredient and unit reference data

**Story:** As a recipe editor, I want consistent ingredient identities and units so that recipe quantities can later be combined correctly.

**Scope:** Recipes-owned Ingredient/Unit entities and tables, minimal bilingual fixtures, reference query and quantity rules.

**Acceptance criteria:**

- Stable ingredient IDs remain the same in French and English.
- The reference set supports agreed mass, volume and count units.
- Invalid quantities and incompatible conversion requests are rejected.
- PostgreSQL mappings and migrations are verified against a disposable database in CI.

**Gate:** Confirm quantity precision, conversion and rounding rules. No nutritional data or density assumptions.

### US-09 Create and view household recipes

**Story:** As a household editor, I want to create recipes that all household members can view so that the generator has a shared catalogue.

**Scope:** Household catalogue, Recipe and RecipeIngredient persistence, authorized read/create contracts and minimal screens.

**Acceptance criteria:**

- An editor saves a recipe with name, ingredient quantities, preparation time and reference servings.
- Description and instructions may be omitted.
- Household members retrieve the same saved recipe; unrelated users are denied.
- User-entered text remains unchanged when the interface language changes.
- Invalid yields or ingredient quantities do not persist.

Public and personal catalogue copy workflows are deferred.

### US-10 Recipe labels for planning

**Story:** As a household editor, I want to define and assign recipe labels so that planning can use my classifications.

**Scope:** Scoped label entities, recipe assignments, authorized endpoints and label controls.

**Acceptance criteria:**

- An editor defines a label and assigns multiple labels to a recipe.
- All household members see the assignments.
- Stable IDs, not translated names, are used by later planning contracts.
- Application-provided labels have French/English text; user labels are preserved.

**Gate:** Decide duplicate-name behavior and the minimal label-edit permissions.

### US-11 Safely edit shared recipes

**Story:** As a household editor, I want to change a recipe without silently overwriting another member's edits.

**Scope:** Recipe editing UI, protected aggregate version, ingredient/label updates and conflict recovery.

**Acceptance criteria:**

- A recipe edit is persisted and visible to other household members after refresh.
- Ingredient and label changes advance the recipe version.
- Of two competing edits from the same version, a stale write fails explicitly.
- The interface lets the user reload and review rather than force an overwrite.
- A lost connection is not shown as a confirmed save.

## Stage 3 Planning

### US-12 Agreed planning rules and examples

**Story:** As the product owner, I want precise planning examples so that the generator implements the intended behavior.

**Scope:** Resolve relevant decisions and update requirements/examples before dependent domain implementation.

**Acceptance criteria:**

- Exact-versus-maximum preparation count is explicitly accepted.
- Leftover reuse window, meal composition and cold quota semantics are defined.
- The confirmed lunch rule counts only portions assigned to lunch slots.
- At least one valid 28-portion/seven-preparation example and conflicting examples are written with expected outcomes.
- Unconfirmed preferences remain distinguishable from required constraints.

This is a documentation decision story; no arbitrary food-safety duration is selected without agreement.

### US-13 Preparations and allocation invariants

**Story:** As a planner, I want portions from one preparation to supply later meals without exceeding its yield.

**Scope:** Pure MealPlan/Preparation/MealSlot/MealAllocation model, RecipeSnapshot value object and invariant tests. No solver, API or persistence yet.

**Acceptance criteria:**

- One four-portion preparation can supply two two-portion meals.
- Allocations cannot exceed produced portions or belong to a different plan.
- Consumption obeys the agreed preparation chronology and reuse rule.
- Produced, allocated and unallocated portions have distinct totals.
- Snapshot ingredients and reference yield can be supplied as fixtures without sharing a mutable Recipe entity.
- Tests exercise valid and rejected allocations without database access.

### US-14 Eligible recipes and constraint input

**Story:** As a planner, I want validated criteria and eligible recipe inputs so that generation uses the right catalogue and rules.

**Scope:** PlanningConstraints, consumer-owned recipe query port, Recipes provider projection and eligibility validation.

**Acceptance criteria:**

- Recipe candidates come through an authorized immutable contract, not planning SQL against recipe tables.
- Input includes source versions, yields, quantities and label IDs.
- Required-label filters follow the agreed combination rules.
- Invalid portion targets or obviously impossible slot quotas produce specific validation outcomes.

### US-15 Generate a valid constrained candidate

**Story:** As a household editor, I want an automatically generated plan that meets my required portion and label constraints.

**Scope:** Pure generation algorithm and independent validation of a returned candidate.

**Acceptance criteria:**

- The agreed example produces 28 allocated portions and seven preparations.
- Vegetarian-only filtering, fourteen lunch-friendly lunch portions and the agreed cold quota are satisfied.
- Repeated recipes do not confuse recipe variety with preparation count.
- A portion may satisfy multiple quotas without inflating the total portions.
- Every returned success is independently validated against hard constraints.

**Gate:** Select the initial bounded search approach after a focused spike. This intermediate engine is not exposed as a production-ready endpoint until US-16.

### US-16 Bounded generation and failure outcomes

**Story:** As a planner, I want generation to stop predictably and explain failure so that an unsuitable request does not leave me waiting indefinitely.

**Scope:** Cancellation/search limits, typed outcomes and representative generator measurements.

**Acceptance criteria:**

- Generation returns success, invalid input, proven infeasibility or search limit as appropriate.
- A failed incomplete search is not labelled proven infeasible.
- Hard constraints are never silently relaxed.
- Limits and observed fixture sizes/durations are documented and testable.

**Gate:** Agree numerical limits from the spike evidence rather than an unmeasured target.

### US-17 Review and save a plan with snapshots

**Story:** As a household editor, I want to review a proposal before saving it so that only an accepted plan becomes shared data.

**Scope:** Generation/review/save endpoints and screens, snapshot persistence and atomic plan storage.

**Acceptance criteria:**

- The proposal shows preparations, meal allocations, remaining portions and criterion totals.
- Generating a proposal does not automatically save a plan.
- Save revalidates authorization, plan invariants, proposal integrity and the agreed source freshness rule.
- Saving persists the plan and children atomically with a version.
- A later source recipe edit does not change the saved preparation ingredients or yield.

**Gate:** Agree how a recipe changed between generation and save is handled. Manual replacements and partial regeneration remain deferred unless explicitly selected.

### US-18 Shared plan viewing and version conflicts

**Story:** As a household member, I want to retrieve the saved plan and understand stale changes so that both phones share consistent planning data.

**Scope:** Authorized plan retrieval, refresh, plan version handling and conflict responses for implemented plan mutations.

**Acceptance criteria:**

- A second user retrieves the same saved plan with its preparations and allocations.
- A refresh retrieves a newer saved revision.
- Supported mutations require the expected revision; stale changes do not overwrite newer data.
- Invariant tests and PostgreSQL integration tests cover concurrency at the plan aggregate boundary.

**Gate:** If no plan-edit action has been selected, agree the minimal replace/save operation used to demonstrate a version conflict. Do not silently expand this story into a full planning editor.

## Stage 4 Groceries and reliability

### US-19 Calculate grocery totals from preparations

**Story:** As a household member, I want ingredient totals based on what will be cooked so that leftovers do not cause duplicate purchases.

**Scope:** Pure grocery calculation over preparation snapshots and compatible units.

**Acceptance criteria:**

- Each preparation contributes once at its produced yield.
- One preparation allocated to several meals does not multiply ingredients.
- Identical ingredients in compatible units are combined correctly.
- Count, mass and volume remain separate without explicit conversion rules.
- Rounding follows the agreed quantity policy and is covered by domain examples.

This can progress independently of generator implementation after the allocation/snapshot contract is defined.

### US-20 Save and view household grocery lists

**Story:** As a household member, I want both phones to retrieve a grocery list derived from the saved plan.

**Scope:** MealPlanning ingredient projection, Groceries persistence/API and minimal list screen.

**Acceptance criteria:**

- Grocery generation reads through MealPlanning.Contracts and checks household permission.
- A saved list records its source plan and revision.
- Both users retrieve the same totals with localized unit labels.
- A newer plan revision does not silently present old groceries as current.
- Source-plan changes during generation are detected or explicitly represented.

**Gate:** Confirm who may generate the list and the initial explicit regeneration behavior. Purchase checkboxes and manual items remain deferred.

### US-21 Retry saved-plan creation without duplicates

**Story:** As an editor, I want to retry a save whose response was lost without creating a second plan.

**Scope:** Idempotency for implemented saved-plan creation, technical receipt adapter and client retry behavior.

**Acceptance criteria:**

- A retry with the same actor, operation, destination and key returns the committed result.
- The same key with different content is rejected.
- Receipt and plan commit atomically; a failed transaction leaves neither half committed.
- Tests simulate a lost response after commit.
- The UI distinguishes a confirmed failure from an unknown outcome without promising offline persistence.

**Gate:** Agree key retention and endpoint scope. Extend to other create commands only through explicitly scoped follow-up work.

### US-22 Validate the full bilingual phone journey

**Story:** As the initial household, we want the complete recipe-to-grocery flow to work on our phones in either language.

**Scope:** Integrated acceptance scenarios, physical-device evidence and focused fixes revealed by those checks.

**Acceptance criteria:**

- Users sign in, enter recipes/labels, generate, review and save a plan, and view groceries.
- Application-owned screens, errors, accessibility labels and formatting work in French and English.
- User text and underlying quantities do not change with language.
- Connection loss, session expiry, stale edits and generation failure have recoverable outcomes.
- Device/build details, remaining limitations and evidence are recorded.

Large newly discovered features become separate follow-up stories rather than expanding this validation PR indefinitely.

## Stage 5 Operations and pilot release

### US-23 Back up and restore persistent data

**Story:** As an operator, I want a proven restoration procedure before relying on real household data.

**Scope:** Database backup/restore commands, external backup destination and verification on an isolated target.

**Acceptance criteria:**

- Backup storage is outside the hosting computer.
- A restore retrieves users, household membership, recipes, plans and groceries.
- Restore validation checks data and access behavior, not merely command exit status.
- Environment configuration/secrets and supported database versions are documented separately from data backups.
- Media transfer is explicitly deferred when upload support is absent.

**Gate:** Choose the backup destination, retention and available restore target. Test with fixture data before real household use.

### US-24 Rehearse relocation to another host

**Story:** As an operator, I want to move the backend without changing business code so that hosting remains replaceable.

**Scope:** Restore on a second machine or independently configured host, same API image, external configuration, test endpoint cutover and rollback instructions.

**Acceptance criteria:**

- The original tested API image runs on the target with configuration changes only.
- Restored prototype data and permissions work from the existing mobile client.
- The stable API address can be switched without introducing a hard-coded provider dependency.
- The documented rehearsal covers source write freeze, validation, cutover and rollback.
- Actual interruption and limitations are recorded; zero downtime is not assumed.

**Gate:** Obtain the target host and agree the allowed interruption. Media portability must be checked separately when that feature is introduced.

### US-25 Release the household pilot

**Story:** As the initial household, we want a validated release with recoverable deployment so that we can use the prototype together.

**Scope:** Trusted release workflow or documented controlled deployment, Staging-to-Production promotion, selected iPhone distribution and household feedback record.

**Acceptance criteria:**

- The same tested backend image is promoted after Staging acceptance.
- Compatible migrations run once as a controlled step; readiness and restoration prerequisites are checked.
- Deployment credentials are scoped and never available to untrusted PR checks.
- Production can return to a compatible prior application version according to the rollback procedure.
- Both users receive the selected device build and complete the reference journey.
- Pilot results update the backlog and relevant accepted/proposed decisions.

**Gate:** Confirm the deployment access method, approval policy, image registry if used, distribution route and explicit authorization for publishing/deployment. No public App Store launch is implied.

## Mapping to prototype milestones

| Existing milestone | Stories |
| --- | --- |
| M0 Scope and decision baseline | US-12 and each story's decision gate |
| M1 Repository and module skeleton | US-01, US-02, US-04 |
| M2 Portable infrastructure | US-03 |
| M3 iPhone connectivity | US-05 |
| M4 Identity and household access | US-06, US-07 |
| M5 Recipe vertical slice | US-08 to US-11 |
| M6 Planning domain and generator | US-12 to US-16 |
| M7 Saved plans and groceries | US-17 to US-21 |
| M8/M9 Integrated mobile validation | US-22 |
| M9 Restoration and portability | US-23, US-24 |
| M10 Household pilot | US-25 |

## Suggested starting sequence

Start with US-01, then US-02 and US-03. US-04 is independent and can be merged early. Validate US-05 as soon as Mac/device access permits. Continue with US-06 through US-11 while resolving US-12 before planning-domain work. The dependency table is the authority for branching order.

Estimate story effort only after its technical spike or decision gate is understood. If a story proves too large for one focused review, split it before implementation and retain explicit prerequisite IDs. All implementation stories require their applicable tests and CI updates in the same PR.
