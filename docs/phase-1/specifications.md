# MealPrepApp specifications

Functional scope and requirements

Version 0.2 | 10 October 2026 | Product owner Thomas Dufresne

This document defines the agreed product direction for MealPrepApp and the requirements to refine before implementation. The initial application requires an Internet connection, shares persistent data within households, and generates weekly meal plans from recipes and user-defined constraints. The application supports French and English. Documentation and source code use English exclusively.

Requirements describe target behavior, not completed functionality. Confirmed requirements reflect product decisions from discovery. Proposed requirements and open decisions remain subject to validation. Technology candidates are recorded in Section 7; their detailed selection belongs in the architecture document.

## 1 Context vision and problem to solve

### 1.1 Platform MealPrepApp

MealPrepApp helps household members decide what to cook, distribute prepared portions across the week, and determine which ingredients to buy. The first users are the product owner and his partner. The long-term objective is a publicly available application serving independent households.

The main user journey is to create an account, join or create a household, maintain a recipe catalogue, generate a weekly plan under selected constraints, and generate a grocery list from the planned preparations.

### 1.2 Sectoral and regulatory context

The product is a meal planning tool. Recipe labels such as high protein are user classifications unless a separate nutritional calculation feature is introduced. No nutritional or food safety certification is implied by those labels.

Account information, household membership, uploaded photos and private recipe content require privacy and access controls. Applicable jurisdictions, public-launch territories and regulatory obligations have not been selected. Appendix A records the required review rather than asserting legal compliance.

## 2 Stakeholders

| Stakeholder | Role and need |
| --- | --- |
| Product owner | Defines scope, priorities and acceptance criteria; initially develops the application. |
| Initial household members | Plan meals together and access consistent shared recipes, plans and grocery lists. |
| Future users | Use the same capabilities in separate households with private data. |
| Household administrators | Manage household details, membership and member permissions. |
| Household recipe editors | Manage shared recipes and generate household plans; role details are proposed. |
| Public catalogue maintainers | Maintain application-provided recipes and their French and English content; workflow remains open. |

## 3 Functional scope

### 3.1 Confirmed initial scope

- Account creation using email and password, authentication, and a profile photo selected from supplied avatars or uploaded by the user.

- Households with a name, photos, members and administrative permissions.

- Recipe creation with ingredients and quantities, reference servings, preparation time and user-defined labels; description and instructions are optional.

- Household recipe visibility and a permission level between member and administrator for recipe editing and planning.

- Automatic weekly planning using serving targets, preparation counts, label filters and minimum labelled-serving quotas.

- Allocation of portions from one preparation to multiple planned meals, including leftovers.

- Grocery list generation from the ingredients required by planned preparations.

- French and English support for application-owned elements and content.

- Connected operation with persistent shared data in a central database.

### 3.2 Confirmed product direction with release timing to decide

The product distinguishes a public read-only recipe catalogue, an editable personal catalogue, and a household catalogue visible to household members. Public recipes can be copied into the personal catalogue and customized there. Personal recipes should support portability between households. The release timing and size of the public catalogue, and the exact copy workflows between personal and household catalogues, remain open.

### 3.3 Future or unconfirmed scope

Offline operation is a nice-to-have for a future release. It does not drive the initial architecture and no local database, pending-change queue or offline merge mechanism is required initially. Ingredient inventory, stock deduction, barcode scanning, nutritional calculations, external recipe import and real-time push updates are not confirmed for the initial release.

Proposed convenience features include marking grocery items as purchased, adding manual grocery items, password recovery and editing profile details. They require scope confirmation.

## 4 Reference scenario

### 4.1 Weekly planning for two household members

Two users create individual accounts and share a household. An authorized editor adds household recipes, records their reference servings and assigns labels such as vegetarian, lunch friendly and suitable cold.

The editor requests a plan covering seven days with lunch and dinner for two people: fourteen meal slots and twenty-eight allocated portions. The editor requests seven preparations, filters all selected recipes to vegetarian, and requires at least fourteen lunch-friendly portions and four portions suitable cold.

The application generates a feasible proposal, shows the preparations and their portion allocations, and explains whether the criteria are satisfied. A preparation producing four portions may supply two portions for dinner and two for the next lunch. The grocery list counts the ingredients for that preparation once, scaled to four portions.

Household members retrieve the saved plan from their phones through the central service. If a request fails because the connection is lost, the application does not claim that the change was saved. If the constraints are infeasible, it explains the result without silently weakening them.

### 4.2 Core domain vocabulary

| Term | Meaning |
| --- | --- |
| Recipe | A reusable dish definition with ingredients, reference servings and optional instructions. |
| Preparation | One planned cooking occurrence of a recipe, with a date and a number of produced portions. |
| Planned meal | A meal slot consuming portions allocated from a preparation. |
| Portion | One serving unit. Produced portions and allocated portions are separate quantities. |
| Leftovers | Produced portions available for later meals; these may already be allocated or remain unallocated. |
| Label | A user-defined recipe classification used in filters or serving quotas. |
| Household | A shared workspace with members, permissions, recipes, plans and grocery lists. |

## 5 Functional requirements

### 5.1 Accounts and households

FR-01  Account creation. The application shall allow a user to create an individual account with an email address and password and then authenticate. Detailed verification and recovery workflows remain open.

FR-02  Profile image. The user shall be able to upload a profile photo or select an application-provided avatar. Upload limits and accepted formats remain open.

FR-03  Household creation. An authenticated user shall be able to create a household with a name and household photos. The creator receives administrative rights. Photo count and required fields beyond the name remain open.

FR-04  Membership administration. Authorized administrators shall be able to add members, remove members and modify their household permissions. Invitation with explicit acceptance is the proposed joining mechanism.

FR-05  Household roles. The application shall distinguish ordinary members, recipe editors and administrators. The intermediate role can add, modify and delete household recipes and generate plans. The complete permission matrix and minimum-administrator rule are proposed below.

| Capability | Member | Editor | Admin |
| --- | --- | --- | --- |
| View household recipes and plans | Yes | Yes | Yes |
| Generate grocery list | To decide | Proposed | Proposed |
| Manage household recipes | No | Yes | Yes |
| Generate meal plan | No | Yes | Yes |
| Modify saved meal plan | To decide | Proposed | Proposed |
| Manage members and roles | No | No | Yes |
| Manage household details | No | No | Yes |

Proposed safeguard: a household cannot lose its last administrator without an explicit ownership-transfer or closure workflow. Membership in multiple households and member departure behavior remain open.

### 5.2 Recipe catalogues and labels

FR-06  Recipe fields. A recipe shall contain a name, ingredients with quantities and units, a positive reference serving count, an average preparation time and labels. Description and instructions are optional. Allowed precision, time interpretation and label cardinality remain to be specified.

FR-07  Household visibility. All household members shall be able to view the household recipe catalogue. Editing permissions follow the household role. Personal catalogue access is restricted to its owner.

FR-08  Public recipe copies. Public catalogue recipes shall be read-only for ordinary users. A user shall be able to copy a public recipe into the personal catalogue and edit the resulting copy without changing the public original. Public catalogue release timing remains open.

FR-09  Personal portability. A user shall be able to retain personalized recipes independently of household membership and reuse them in another household. Explicit copies between personal and household catalogues are proposed; ownership and export rights after departure remain open.

FR-10  User-defined labels. Users shall be able to define recipe labels in their personal catalogue. Shared household labels shall be editable by authorized users. Multiple labels can apply to one recipe. User-created names are stored as entered and are not automatically translated.

FR-11  Serving scaling. The application shall scale scalable ingredient quantities using requested portions divided by reference servings. For example, 400 g of rice for four reference servings becomes 200 g for two portions. Non-scalable quantities and rounding rules remain open.

FR-12  Recipe lifecycle. Authorized users shall be able to modify and delete recipes in editable catalogues. The treatment of recipes already referenced by saved preparations must be decided before implementation; existing plans must not silently lose their meaning.

### 5.3 Automatic planning and leftovers

FR-13  Planning inputs. An authorized user shall be able to request automatic planning for a selected week with meal slots, portion targets, a desired preparation count and recipe criteria. Whether preparation count is exact or a maximum remains open.

FR-14  Recipe eligibility. The generator shall apply selected inclusion filters to eligible household recipes. For example, a vegetarian-only filter excludes recipes without the selected vegetarian label. The combination rules for multiple filters remain open.

FR-15  Labelled portion quotas. The generator shall support minimum portion quotas for selected labels, such as at least fourteen lunch-friendly portions and four portions suitable cold. Lunch-friendly quotas count only portions allocated to lunch meal slots; dinner allocations from the same labelled recipe do not count toward this quota. One allocated portion may satisfy several quotas but counts only once toward the overall portion target.

FR-16  Preparations and allocations. The generator shall model preparations separately from recipes and meal slots. A preparation may supply several meals, and one recipe may be prepared more than once in a week. An optional recipe-variety constraint is proposed rather than assumed.

FR-17  Portion accounting. The plan shall track produced, allocated and unallocated portions. Allocated portions shall not exceed produced portions. The requested weekly portion target refers to portions assigned to meals; any excess production must be visible.

FR-18  Leftover chronology. A later meal may consume portions from an earlier preparation. A meal shall not consume a preparation scheduled after it. Storage duration, freezing, reheating and cross-week carryover policies remain open and shall not be inferred from recipe labels.

FR-19  Generation outcomes. The generator shall return a plan satisfying its validated constraints or an explicit failure outcome. It shall not silently relax required filters or quotas. The user shall be able to revise criteria. Solver limits and the distinction between proven infeasibility and a search timeout remain open.

FR-20  Plan review. A generated proposal shall show selected recipes, preparation dates, produced portions, meal allocations and criterion totals before it is saved. Manual replacement, partial regeneration and protected meal slots are proposed workflows to confirm.

FR-21  Shared plans. A saved household plan shall be persisted centrally and be retrievable by authorized members on their phones. Immediate push updates are not required by the current scope.

### 5.4 Grocery lists

FR-22  Ingredient derivation. The application shall generate a grocery list from each planned preparation exactly once, scaling recipe ingredients to the preparation yield. Allocating leftovers to additional meals shall not duplicate those ingredients.

FR-23  Ingredient grouping. The application shall group ingredients only when their identities match and their units are compatible. For example, 500 g plus 1 kg may become 1.5 kg. Two tomatoes and 300 g of tomatoes shall not be merged without an explicit conversion rule.

FR-24  Regeneration behavior. When a plan changes, the application shall provide an explicit way to update its grocery list. Automatic updates, preservation of purchased states and preservation of manually added items remain open.

No inventory deduction is specified for the initial release. Grocery purchase checkboxes, manual items, duplicate-name handling and display-unit preferences require confirmation.

### 5.5 French and English application content

FR-25  Language coverage. Every application-owned user-facing element shall have French and English versions. This includes navigation, field labels, actions, validation messages, errors, permission names, planning explanations, empty states, accessibility labels, supplied avatars with text and application-provided notifications or emails when introduced.

FR-26  Shared application content. Application-provided recipe content, ingredient names, default labels, help content and other shared reference data shall support French and English when included in the product. Public recipes are application-owned content and therefore require both languages.

FR-27  User content preservation. User-specific names, descriptions, instructions, labels and household names shall be preserved as entered. No automatic translation of user content is required. A copied public recipe remains editable; how its bilingual source fields are retained in a personal copy remains open.

FR-28  Language preference. Proposed behavior: each user can select French or English independently of other household members. The application remembers the choice across sessions. Initial language selection, regional variants and fallback rules remain open.

FR-29  Locale-aware presentation. Dates, numbers and translated unit labels shall follow the selected supported locale without changing stored quantities. Language selection shall not automatically convert measurement systems. Supported regional locales and unit preferences remain open.

## 6 Nonfunctional requirements

### 6.1 Quality attributes

NFR-01  Connected operation. The initial release requires an Internet connection for data retrieval and persistence. It shall not promise offline editing or deferred synchronization. A failed write shall not be displayed as successfully saved; retry behavior must prevent unintended duplicate actions.

NFR-02  Data isolation. The server shall enforce access to personal and household resources for every operation. Authorization shall not rely only on hidden interface controls or household identifiers supplied by a client.

NFR-03  Concurrent changes. The application shall prevent silent loss when users edit the same shared resource concurrently. Version-based conflict detection is a proposed solution. The user shall receive a clear conflict outcome and a recovery path; offline merging is outside scope.

NFR-04  Maintainability. Business rules shall be independently testable and separated from user-interface, transport and persistence details. A modular backend with hexagonal boundaries is the proposed architectural direction; no offline-specific structures are required initially.

NFR-05  English engineering language. Project documentation, source identifiers, comments, developer-facing messages and test descriptions shall use English exclusively. French appears in code only where needed as application translation content or user test data. User-facing English and French resources remain distinct from engineering prose.

NFR-06  Translation completeness. Acceptance review shall check both languages for each application-owned text and relevant workflow. Shared users shall see the same underlying data regardless of language. Missing translation keys and mixed-language application messages shall be detected before release.

NFR-07  Mobile usability. The application shall be usable on an iPhone, including text entry, keyboard behavior, navigation and translated text expansion. Supported iOS versions, screen sizes and accessibility acceptance criteria remain to be selected.

NFR-08  Performance targets. Generation duration, API response times, household catalogue size and expected simultaneous users require measurable targets before implementation acceptance. No numerical performance commitments have been agreed.

NFR-09  Hosting portability. The backend shall be deployable using a standard container image and externally supplied configuration. Central persistence uses standard PostgreSQL. Provider-specific integrations remain behind replaceable adapters. Media references use object keys rather than provider-specific URLs. A relocation rehearsal shall restore implemented data on another host without changes to business code.

NFR-10  Environment separation. Development, Staging and Production shall have distinct configurations, credentials and data. They may initially share the owner's computer. Database connectivity remains private to backend services; the mobile client uses the API.

### 6.2 Observability

Proposed operational visibility includes failed requests, authentication and authorization failures, generation duration and outcome, persistence errors and correlated request identifiers. Logs should avoid passwords, tokens, private recipe bodies and unnecessary personal information. Logging retention, monitoring provider and alert thresholds remain open.

### 6.3 Acceptance and test approach

| Check | Expected outcome |
| --- | --- |
| Serving scaling | A four-serving recipe containing 400 g scales to 200 g for two portions. |
| Leftover allocation | One four-portion preparation covers two two-portion meals and is counted once in groceries. |
| Planning quotas | A valid plan covers 28 allocated portions, the selected preparation count and each required labelled quota. |
| Unsatisfied constraints | The application reports failure without silently changing the requested criteria. |
| Household isolation | A user cannot access another household by changing a resource identifier. |
| Concurrent editing | A stale change cannot silently overwrite an intervening saved change. |
| Connection failure | An unconfirmed change is not reported as saved. |
| Bilingual behavior | Both languages cover application-owned text; user recipe names remain unchanged. |

These are planned checks. No implementation or test results are asserted by this document.

### 6.4 Compliance and operations

Before public launch, define backup and restore procedures, deployment environments, secret management, account lifecycle, data retention, upload controls and privacy information in both languages. Operational objectives and legal applicability remain open. Detailed architecture and deployment choices belong in the architecture document.

## 7 Technical constraints

The initial household backend may run on the owner's personal computer. Hosting must remain replaceable; availability, remote access and backups require explicit configuration.

The developer primarily uses .NET 10 for backend development and Angular 21 for frontend development. The target includes iPhone. An iPhone is available for testing; access to a borrowed Mac is limited.

Proposed technology candidates are Angular with Ionic and Capacitor, an ASP.NET Core .NET 10 API, Entity Framework Core and a central PostgreSQL database. These candidates require compatibility checks and an early iPhone build prototype before acceptance. Hosting, authentication provider, media storage and build automation are undecided.

Clients shall access central data through the application service rather than direct database credentials. The initial architecture shall support household access control and ordinary online concurrency. A future offline mode will require its own requirements and design review.

## 8 Use cases

### UC-01 Create an account

Actor: Visitor. Precondition: The service is reachable.

Main flow: Enter email and password and submit valid registration details; choose or upload a profile image.

Outcome: An individual account is created; detailed verification behavior is open.

### UC-02 Create and administer a household

Actor: Authenticated user then household administrator. Precondition: The user is authenticated.

Main flow: Create the household, add its details, add members and assign permitted roles.

Outcome: The household and membership changes are saved and subject to access controls.

### UC-03 Maintain a household recipe

Actor: Household editor or administrator. Precondition: The actor has editing rights.

Main flow: Create or edit a recipe with ingredients, units, reference servings, preparation time and labels.

Outcome: The saved recipe is visible to household members; invalid data is rejected.

### UC-04 Customize and transfer a recipe

Actor: Authenticated user. Precondition: An accessible source recipe exists.

Main flow: Copy a public recipe to the personal catalogue and edit it; transfer a personal copy to a household through the confirmed workflow.

Outcome: The original remains unchanged and the personalized recipe can be retained independently of membership.

### UC-05 Generate a weekly plan

Actor: Household editor or administrator. Precondition: Eligible household recipes exist.

Main flow: Select the week, meal slots, portion target, preparation count and label constraints; generate and review the proposal.

Outcome: A valid plan can be saved, or the application reports an explicit failure. Detailed editing actions remain open.

### UC-06 Allocate leftovers

Actor: Authorized planner. Precondition: A preparation has available portions.

Main flow: Assign portions to later meal slots without exceeding the preparation yield.

Outcome: The plan records chronological allocations and remaining portions without duplicating preparation ingredients.

### UC-07 Generate a grocery list

Actor: Authorized household user. Precondition: A saved plan exists and the actor has the required permission.

Main flow: Generate ingredient totals for all planned preparations using compatible units.

Outcome: The household list is saved; permissions and later regeneration behavior remain open.

### UC-08 Use the application in French or English

Actor: User. Precondition: Both language resources are available.

Main flow: Select the preferred language using the proposed setting.

Outcome: Application-owned content changes language; user-created content and numerical quantities retain their meaning.

## Appendix A Regulatory review

No jurisdiction-specific legal requirements have been confirmed. Before public release, establish launch territories and assess privacy obligations, account and data deletion, retention, media permissions and platform distribution requirements. Record verified primary references and applicability here once those decisions are made. No compliance claim is made at this stage.

## Appendix B Open product decisions

| ID | Decision to resolve |
| --- | --- |
| OD-01 | Which catalogue capabilities and how many public recipes belong in the initial release? Who maintains the public catalogue? |
| OD-02 | Can a user belong to multiple households? How are invitations, departure, ownership transfer and recipe recovery handled? |
| OD-03 | Confirm the permission matrix, grocery permissions and saved-plan editing rights. |
| OD-04 | Define copy and provenance behavior between catalogues, including bilingual fields copied from public recipes. |
| OD-05 | Is the preparation count exact or a maximum? Are recipe variety, repeated dishes and several dishes per meal configurable? |
| OD-06 | Define label filter combinations, duplicate labels, quota eligibility and constraint priorities. |
| OD-07 | Define leftover storage windows, freezing, cross-week carryover and excess-portion handling. |
| OD-08 | Define ingredient identities, units, conversion rules, scaling exceptions and rounding. |
| OD-09 | Define plan edits, regeneration, recipe changes after planning and grocery update behavior. |
| OD-10 | Confirm purchase checkboxes, manual grocery items, profile changes and account recovery scope. |
| OD-11 | Select French and English regional locales, initial preference, fallback and translation maintenance. |
| OD-12 | Select uploads limits, public-launch territories, operational targets and supported iPhone versions. |
