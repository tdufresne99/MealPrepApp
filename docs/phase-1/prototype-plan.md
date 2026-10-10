# MealPrepApp prototype development plan

Version 0.1 | 10 October 2026 | Product owner Thomas Dufresne

## Objective

Build a connected iPhone prototype that lets two individually authenticated users in one household maintain recipes, generate a constrained weekly meal plan, allocate leftovers, and retrieve a grocery list. Validate the modular hexagonal boundaries and prove that the backend can move from the owner's computer to another host without changing business code.

The application supports French and English for all application-owned user-facing content. Documentation, code identifiers, comments and developer diagnostics use English. User-created recipe text and labels remain as entered.

This plan defines future development work. No prototype implementation, successful device build or executed acceptance test is implied. The repository contains the specifications, architecture and development plan; application scaffolding remains to be created.

## Source documents and precedence

- [Product specifications](specifications.md): functional requirements, scope and open product decisions.
- [Architecture](architecture.md): modular hexagonal proposal, including Section 5.4 on context ownership and contracts.
- This plan records later discovery decisions about lunch quotas, portable personal-computer hosting and the prototype sequence. These decisions are incorporated into the repository specifications and architecture. The external Word files are historical authoring sources and may lag behind repository changes.

## Confirmed direction

- Require an Internet connection; do not implement offline editing or synchronization.
- Use individual accounts and shared household data with Member, Editor and Administrator roles. Detailed unconfirmed permissions remain decision gates.
- Separate Recipe, Preparation, MealSlot and MealAllocation.
- Generate plans automatically using label filters, preparation counts and portion quotas.
- Count a lunch-friendly quota only on portions assigned to lunch meal slots. Dinner portions from the same labelled preparation do not contribute to that quota.
- Count an allocated portion once toward the overall target even if it satisfies several independent quotas.
- Generate grocery totals from preparations, not from repeated meal allocations.
- Keep Development, Staging and Production logically separate. They may initially run on the same physical computer.
- Keep hosting replaceable through standard containers, PostgreSQL, external configuration, a stable API address and explicit data-transfer procedures.

## Prototype boundary

### Included

- Seeded individual accounts, authentication and current household permission checks.
- One demonstration household and a second isolated test household for access-control verification.
- Creation, viewing and editing of household recipes with ingredients, quantities, reference servings, preparation time and labels.
- Minimal ingredient reference data and compatible mass, volume and count units.
- A small recipe dataset sufficient to demonstrate valid and invalid planning requests.
- Automatic generation, proposal review and explicit persistence of a weekly plan.
- Preparations whose portions can supply several later meal slots.
- A generated grocery list with plan revision provenance.
- Version-conflict handling for shared recipe and plan mutations.
- French and English interface resources, error messages and application-provided reference data.
- An installed iPhone client connected to the backend hosted on the owner's computer.
- Reproducible startup, database backup/restore and a backend relocation rehearsal.

### Deferred

Self-service registration, email verification, password recovery, invitation delivery, full household administration screens, photo upload, the large public catalogue, complete personal-catalogue transfer workflows, purchase checkboxes and manual grocery items are deferred from the first prototype. Their product requirements remain in the specifications.

Offline operation, inventory deduction, nutritional calculation, external recipe import and live push updates remain outside prototype scope. No production-complete account lifecycle or public-launch compliance is claimed by the prototype.

## Proposed planning defaults

These defaults make the first implementation bounded. Items still open are explicitly listed in the decision register.

| Parameter | Prototype proposal |
| --- | --- |
| Reference week | Seven days, lunch and dinner for two people |
| Consumption target | Fourteen meal slots and twenty-eight allocated portions |
| Preparation target | Exactly seven preparations; exact-versus-maximum semantics to confirm before solver implementation |
| Portion precision | Positive whole portions; decimal ingredient quantities |
| Meal composition | One preparation per slot initially; a preparation may supply multiple slots |
| Recipe repetition | Allowed; preparation count is independent of recipe variety |
| Required labels | All selected required labels must match each eligible recipe |
| Lunch quota | Count labelled portions assigned to lunch slots only; confirmed |
| Cold suitability quota | Count allocated portions from recipes labelled suitable cold; explicit consumption-mode semantics remain open |
| Leftovers | Consume after preparation, within an explicit agreed reuse window; do not infer a food-safety guarantee |
| Ingredient conversion | g/kg and ml/L conversions; no assumed mass-to-volume or count-to-mass conversion |
| Generation failure | Never relax required constraints silently; distinguish invalid input, proven infeasibility and search limit |

## Development milestones

| Stage | Depends on | Demonstrable output | Completion condition |
| --- | --- | --- | --- |
| M0 Scope and decision baseline | None | Prototype scenarios and decision register | Identify decisions needed for the next stage; do not require every future feature to be settled |
| M1 Repository and module skeleton | M0 | Buildable backend and Angular client, module boundaries and CI checks | A clean checkout builds; forbidden project references are checked |
| M2 Portable local infrastructure | M1 | API container, PostgreSQL, external settings and environment separation | Repeatable startup and readiness checks; database survives container recreation |
| M3 Early iPhone connectivity spike | M1, M2 | Installed minimal app calling the API on the owner's computer | Real-device HTTPS request succeeds; connection errors are visible; both languages are exercised |
| M4 Identity and household access | M2, M3 authentication decision | Seeded login, current-user identity and permissions | Two users access their household; an unrelated user cannot access it |
| M5 Recipe vertical slice | M4 | Recipe domain, persistence, API and minimal screens | Authorized recipe creation/editing persists; stale edits conflict; unit and label validation passes |
| M6 Planning domain and generator | M5, planning decision gates | Pure generator and invariant validation | Fixture satisfies 28 portions, 7 preparations and agreed quotas; impossible and limited searches return distinct outcomes |
| M7 Saved planning and groceries | M6 | Reviewed proposals, snapshots, saved allocations and grocery totals | Another user retrieves the same plan; leftovers count ingredients once; source revisions are recorded |
| M8 Complete mobile journey | M3 to M7 | Usable bilingual recipe-to-grocery workflow | Both users complete the journey on phones; no failed write is reported as saved |
| M9 Integration and portability rehearsal | M7, M8 | Test evidence, restoration and host relocation | Restore on another computer or independently configured host; same backend image works without business-code changes |
| M10 Household pilot and evaluation | M9 | Staging-validated release used by the household | Record feedback, limitations and measured generation behavior; decide what belongs in the next release |

M3 is deliberately early: native packaging and network reachability must be validated before investing in the full interface. Backend and generator work may continue independently when waiting for access to the borrowed Mac. Calendar estimates will follow the toolchain and generator spikes rather than assume an untested delivery date.

## Detailed work by milestone

### M0 Scope and decisions

- [ ] Record the confirmed lunch-slot counting rule in the planning examples.
- [ ] Confirm prototype exclusions and the proposed generator defaults before their dependent work.
- [ ] Prepare an example with two users, 28 portions, seven preparations, vegetarian-only recipes, fourteen lunch-friendly portions and four suitable-cold portions.
- [ ] Identify when the borrowed Mac is available and which remote-host access scenario the pilot needs.
- [ ] Record accepted choices separately from proposed defaults; unresolved choices stay visible.

### M1 Repository and hexagonal boundaries

- [ ] Use the current repository as the application code root and initialize the build/test conventions without overwriting the Word documents.
- [ ] Create one ASP.NET Core API host and module-specific Domain, Application and Infrastructure projects for Identity, Households, Recipes, MealPlanning and Groceries.
- [ ] Add narrow Contracts projects only for providers exposing cross-module capabilities.
- [ ] Keep Domain independent of EF Core, HTTP, identity-framework types and other module domains.
- [ ] Define consumer-owned outbound ports and provider-owned immutable read contracts.
- [ ] Scaffold the Angular client with feature folders and translation resources.
- [ ] Pin compatible dependency versions after checking .NET/EF/Npgsql and Angular/Ionic/Capacitor compatibility.
- [ ] Configure CI for builds, relevant tests and architecture reference checks.

Deliver one small working endpoint rather than fully implementing every module at this stage. Future features must not lead to empty repositories, speculative ports or unused integrations.

### M2 Hosting and environment foundation

- [ ] Build a standard API container image and local Compose configuration for API/PostgreSQL.
- [ ] Add persistent database volumes and a controlled migration command with one executor.
- [ ] Provide configuration templates without embedded credentials or provider-specific business settings.
- [ ] Separate Development, Staging and Production database names, credentials and ports/routes. Separate PostgreSQL instances may be selected if needed; three physical machines are not required.
- [ ] Add liveness/readiness endpoints and useful English diagnostics with request correlation.
- [ ] Verify startup after reboot and document the effect of sleep, lost connectivity and host shutdown.
- [ ] Keep database access internal; phones access the application API only.
- [ ] Define backup storage outside the hosting computer and a documented restore procedure before real household data is introduced.

Provider-specific routing or deployment files belong in infrastructure/deployment configuration. Hosting changes must not change module domain or application behavior.

### M3 iPhone and network spike

- [ ] Validate available macOS/Xcode versions, signing and the chosen native dependency versions.
- [ ] Install a minimal client on the physical iPhone and call the backend through its configured API address.
- [ ] Choose pilot access: household Wi-Fi only, private remote access, or a controlled public HTTPS entry point.
- [ ] Validate trusted HTTPS, certificate behavior and the actual mobile request origin/configuration.
- [ ] Demonstrate language selection and translated network errors.
- [ ] Identify a repeatable build/distribution path, using the borrowed Mac or a later hosted macOS build environment.

This spike validates the toolchain and connectivity; it is not the final authentication, media or production-release implementation.

### M4 Identity and Households

- [ ] Decide the account/session implementation and secure mobile credential storage before implementing login.
- [ ] Seed test users and households with no committed production credentials.
- [ ] Implement authentication, logout/expiration behavior and trusted current-user access.
- [ ] Implement household access queries and Member/Editor/Administrator capability checks.
- [ ] Enforce current membership on every protected request and provider contract.
- [ ] Test cross-household reads and writes and access after membership removal.

Identity owns its account/profile tables; Households owns membership and role assignments. Household role values must not grant public-catalogue administration permissions.

### M5 Recipes

- [ ] Implement Catalogue/Recipe ownership and minimal household catalogue persistence.
- [ ] Add a shared ingredient fixture, compatible units and stable label identifiers.
- [ ] Implement serving scaling and valid recipe child updates with aggregate version changes.
- [ ] Add authorized recipe read/create/edit endpoints and minimal bilingual screens.
- [ ] Expose the immutable planning query through Recipes.Contracts, adapted to MealPlanning's outbound need.
- [ ] Provide application-owned reference labels and ingredients in both languages while preserving user-entered text.
- [ ] Verify actual PostgreSQL constraints/mappings and recoverable stale-version errors.

Personal/public catalogue copies remain a later slice. Do not implement cross-module SQL access as a temporary shortcut.

### M6 Generator and allocation model

- [ ] Resolve preparation-count semantics, leftover reuse window, quota applicability and unit rounding before depending on those rules.
- [ ] Implement Preparation, MealSlot, MealAllocation, PlanningConstraints and immutable recipe input.
- [ ] Separate constraint validation, solving, final result validation and optional ranking preferences.
- [ ] Count lunch-friendly portions only in lunch slots; overlapping quotas must not inflate the consumption total.
- [ ] Verify yield conservation and preparation-before-consumption chronology.
- [ ] Prototype bounded search with explicit cancellation/limit outcomes; select a solver only if the spike justifies one.
- [ ] Test satisfiable examples, obvious impossible quotas, repeated recipes and search-limit outcomes.
- [ ] Record recipe-set size and generation duration; choose measurable performance targets from those observations.

The engine has no database or UI access. An unsuccessful incomplete search is not evidence of infeasibility.

### M7 Plan persistence and Groceries

- [ ] Implement generate/review/save as distinct application actions.
- [ ] Use coherent recipe inputs and verify the agreed source freshness rule when saving.
- [ ] Capture preparation recipe snapshots and save plan children atomically under the protected plan version.
- [ ] Calculate grocery ingredients once per preparation and combine only compatible units/identities.
- [ ] Store grocery source plan revision and expose stale provenance when a plan changes.
- [ ] Implement narrowly scoped retry protection for plan creation and other relevant commands; receipt and mutation commit together.
- [ ] Verify that changing a recipe later does not silently alter saved plan or grocery calculations.

Groceries reads preparation projections through MealPlanning.Contracts and never queries planning tables directly. Final plan-edit and grocery-regeneration behaviors require confirmation before extending the initial journey.

### M8 Bilingual mobile workflow

- [ ] Connect login, household context, recipe screens, generation controls, proposal review, saved plan and grocery list.
- [ ] Show portions and preparations as distinct quantities throughout the interface.
- [ ] Display meaningful translated validation, conflict, generation and connection outcomes.
- [ ] Confirm success only after a server response; do not imply offline persistence of edits.
- [ ] Complete French and English navigation, labels, plurals, date/number formatting and accessibility text.
- [ ] Verify text expansion, keyboard behavior and app return/session expiration on the iPhone.
- [ ] Use explicit Staging/Production configuration so test data is not mistaken for real household data.

### M9 Integration and relocation

- [ ] Run domain/application tests and real PostgreSQL integration tests appropriate to implemented behavior.
- [ ] Check assembly references, schema mappings and prohibition of cross-module EF/SQL access.
- [ ] Exercise concurrent recipe/plan edits and a request whose response is lost after commit.
- [ ] Restore a database backup into a separate target and verify users, recipes, plans and groceries.
- [ ] Start the same API image on another machine or independently configured host using only deployment configuration changes.
- [ ] Repoint the stable test API address and verify the existing mobile client.
- [ ] Transfer media objects and validate keys if media support has entered scope; otherwise record this portability check as deferred.
- [ ] Record source write-freeze, backup/restore, validation, cutover and rollback steps for the small pilot migration.

The relocation rehearsal proves portability for the implemented prototype. It does not promise zero downtime or a fixed migration duration at larger data volumes.

### M10 Pilot release

- [ ] Promote a tested backend image from Staging to Production without rebuilding it.
- [ ] Apply compatible database migrations as a controlled release step.
- [ ] Use a tested iPhone build through the agreed pilot distribution route.
- [ ] Verify production backups and keep an API rollback path compatible with the deployed schema.
- [ ] Collect household feedback on recipe entry, generation results and grocery usefulness.
- [ ] Update the specifications, ADR statuses and next-release priorities from actual observations.

## Decision register and user checkpoints

| ID | Decision | Current state | Needed before |
| --- | --- | --- | --- |
| D01 | Lunch-friendly quota counts lunch allocations only | Confirmed by user | M6; carry into specifications |
| D02 | Exact preparation count or maximum | Exact proposed; confirm | M6 |
| D03 | Leftover reuse window, freezing and cross-week handling | Open; no inferred safe duration | M6 |
| D04 | One preparation per meal, label combinations and cold quota semantics | Bounded defaults proposed | M6 |
| D05 | Authentication/session and device credential storage | Technical decision pending | M4 |
| D06 | Pilot network access and availability of the borrowed Mac | User information needed | M3 |
| D07 | Initial French/English regional locales and language-switch behavior | Open | M3/M8 |
| D08 | Ingredient precision, rounding and allowed units | Minimal units proposed | M5/M6 |
| D09 | Manual plan edits and grocery regeneration behavior | Deferred until initial flow works | Extension of M7/M8 |
| D10 | Generator limits and response-time targets | Measure before choosing numerical commitments | M6/M9 |
| D11 | Repository code root and practical environment configuration | Current repository selected as code root; environment configuration pending | M1/M2 |

Ask the product owner when a decision changes scope, visible behavior, data ownership, retention or access. Present a concrete recommendation, the alternative and their consequences. Record the answer and continue independent work while waiting. Do not treat an unanswered question as approval. Routine implementation details within confirmed scope can be resolved autonomously.

At each milestone, show the demonstrable result, relevant verification and the next decision gate. Do not ask for all future feature choices before scaffolding the prototype. Obtain authorization before external publishing, paid service creation or actions beyond the requested development scope.

## Prototype acceptance checklist

- [ ] The client runs on an actual iPhone and reaches the configured API.
- [ ] Two individual users see the same saved household recipes, plan and grocery list.
- [ ] Another household cannot access those resources.
- [ ] A valid request produces 28 allocated portions and the selected seven preparations.
- [ ] Fourteen lunch-friendly portions are allocated to lunch slots, not merely present in dinner recipes.
- [ ] Cold suitability and remaining-portion totals follow the explicitly selected rules.
- [ ] Leftover allocations do not multiply grocery ingredients.
- [ ] Invalid constraints and search-limit outcomes are distinguishable.
- [ ] Stale shared edits are not silently overwritten.
- [ ] Connection failures are not reported as successful persistence.
- [ ] Application-owned content is complete in French and English; personal text is unchanged.
- [ ] Backup restoration and host relocation succeed for the implemented data.
- [ ] Module ownership, architecture boundaries and relevant test evidence are recorded.

## Work status

The plan is created and D01 is confirmed. Implementation milestones have not started. The next authorized development task would be M1/M2 with the early M3 device spike; actual implementation is a separate task from preparing this plan.
