# Musicx Development Guidelines

## Preamble

Welcome to the Musicx development guide.  
This document is the shared reference for building features in a consistent way across API, Web, and future clients (Mobile/Desktop).

The goals are:

- keep the codebase **clean, modular, and evolvable**;
- enforce a **predictable development flow** for endpoints and UI features;
- improve **readability, maintainability, and collaboration quality**.

Use this guide as the default standard. Deviations should be explicit, justified, and limited.

---

## Table of Contents

1. [Technology Stack](#1-technology-stack)
2. [Project Architecture](#2-project-architecture)
3. [Interfaces, Implementations, and Dependency Injection](#3-interfaces-implementations-and-dependency-injection)
4. [Endpoint Lifecycle (End-to-End)](#4-endpoint-lifecycle-end-to-end)
5. [Generic Endpoint Matrix](#5-generic-endpoint-matrix)
6. [Web vs Web.Client Separation (Blazor Hybrid Server + Interactive WASM)](#6-web-vs-webclient-separation-blazor-hybrid-server--interactive-wasm)
7. [Main Folder Responsibilities and Dependencies](#7-main-folder-responsibilities-and-dependencies)
8. [Coding Rules and Conventions](#8-coding-rules-and-conventions)
9. [Definition of Done Checklist](#9-definition-of-done-checklist)

---

## 1. Technology Stack

### .NET and Core Runtime

- **.NET target**: `net9.0` across main projects.
- Backend host: **ASP.NET Core Web App + API controllers**.
- Frontend rendering: **Razor Components + Interactive WebAssembly render mode**.

### Front-End Frameworks

- **Blazor** for component-based UI.
- **MudBlazor** as the main UI component system.
- **MudExtensions** and related ecosystem libraries when useful.

### Other Important Libraries and Building Blocks

- JWT tooling for auth/security flows.
- OpenAPI for API endpoint discoverability.
- Custom infrastructure for SQL building, repository abstractions, and mapping.
- `ITranslationService` + `.resx` resources for localization.

> Keep package updates coherent across projects and avoid accidental version drift.

---

## 2. Project Architecture

### Architectural Direction

Musicx follows a **Clean Architecture-inspired layering** with strong module boundaries:

- **Contracts**: DTOs and enums shared across layers.
- **Application.Shared**: cross-platform abstractions and reusable domain-adjacent logic.
- **Application.Api**: API-specific interfaces and query models.
- **Application.Web**: web-facing view models and web abstractions.
- **Infrastructure**: technical implementations (repositories, sql builders, mappers, clients, providers, localization).
- **Presentation.Web**: server host + API controllers + web composition root.
- **Presentation.Web.Client**: Blazor components/pages for UI.
- **Application.Desktop**: desktop use-cases and interfaces (already present and extensible).

### Separation by Runtime Target

- **Shared abstractions** stay in `Application.Shared` and `Contracts`.
- **API-specific behavior** lives in `Application.Api` + `Infrastructure/API` + `Presentation.Web` controllers.
- **UI behavior** is split into web host (`Presentation.Web`) and wasm client (`Presentation.Web.Client`).
- Future **Mobile/Desktop** clients must reuse `Contracts`, `Application.Shared`, and relevant infrastructure services without duplicating backend contracts.

---

## 3. Interfaces, Implementations, and Dependency Injection

### Interface / Implementation Strategy

- Define abstractions in Application projects (`Application.*.Interfaces`).
- Implement technical behavior in `Infrastructure`.
- Keep controller/component code focused on orchestration, not low-level concerns.

### Dependency Injection Rules

- Register services through composition root extension methods (`AddMusicxInfrastructure`, `AddMusicxApi`, `AddMusicxWeb`, `AddFrontFramework`).
- Keep service lifetime intentional (Scoped/Singleton) and aligned with usage.
- New endpoint/service dependencies must be wired in DI at the appropriate layer.

### Why this matters

This pattern keeps:

- unit boundaries clear;
- testing easier (mock interface contracts);
- migration to new presentation targets (Mobile/Desktop) significantly simpler.

---

## 4. Endpoint Lifecycle (End-to-End)

This is the reference flow when adding or modifying an endpoint.

1. **Define contracts** (`In*` and `Out*` DTOs in `Musicx.Contracts`).
2. **Define query models / repository interfaces** in `Application.Api` and shared abstractions:
    - repository interface,
    - `IJoinSpecification<TIn>`,
    - `OrderSpecification<TIn>`,
    - `IFindQuery<TIn>`.
3. **Define persistence structure** in Infrastructure/API:
    - SQL schema (`Create*Table.sql`) and columns,
    - mapper (`*Mapper`),
    - sql builder (`*SqlBuilder`),
    - repository implementation.
4. **Expose endpoint** in `Presentation.Web/Controllers`.
5. **Consume endpoint** from `Infrastructure/Shared/Clients/ApiClient.cs`.
6. **Use endpoint in UI/use cases** via services/view models.

### Important detail by stage

- **DTOs In/Out** are the external contract: stable, explicit, and version-safe.
- **Join/Order/FindQuery** make read behavior composable and reusable.
- **Columns + Mapper + SqlBuilder + Repository** ensure persistence concerns remain in Infrastructure.
- **Controller + ApiClient** close the loop between backend and clients.

---

## 5. Generic Endpoint Matrix

The project uses a repeatable generic pattern for many resources.

| Operation | Typical Verb + Route | Input | Output | Purpose |
|---|---|---|---|---|
| Count | `GET /api/{resource}/count` | optional `FindQuery` | `long` | Count matching entities |
| Find | `GET /api/{resource}` | `FindQuery` + `JoinSpecification` + `OrderSpecification` + paging | `OutGenericList<TOut>` | Paginated/filterable listing |
| FindById | `GET /api/{resource}/{id}` | `id` + optional joins | `TOut` | Read a single entity |
| Save | `POST /api/{resource}` | `In*` DTO | created/updated result or status | Create/update one entity |
| SaveAll | `POST /api/{resource}/save-all` | collection of `In*` DTOs | bulk status | Batch write |
| Delete | `DELETE /api/{resource}/{id}` | `id` | status | Remove one entity |
| DeleteAll | `DELETE /api/{resource}/by-ids?ids=...` | list of ids | status | Remove many entities |
| DataView | `GET /api/{resource}/{id}/data-view` | id + optional contextual params | `Out*DataView` | Read optimized UI aggregate |

> Rule: for each new backend endpoint, add or update the corresponding call in `ApiClient`.

---

## 6. Web vs Web.Client Separation (Blazor Hybrid Server + Interactive WASM)

### `Musicx.Presentation.Web` (Server Host)

Responsibilities:

- ASP.NET host bootstrap (`Program.cs`);
- middleware pipeline (auth/localization/compression/static assets);
- controller endpoints (`/api/...`);
- Razor host and interactive render mode activation.

### `Musicx.Presentation.Web.Client` (Interactive Client)

Responsibilities:

- pages/components/modals;
- code-behind and optionally ViewModel logic;
- MudBlazor-based rendering;
- service consumption through injected abstractions.

### Why this split

- Better boundary between server composition and UI feature code.
- Cleaner scalability for future client targets (Desktop/Mobile).
- Allows Interactive WASM experiences while keeping server capabilities in one host.

---

## 7. Main Folder Responsibilities and Dependencies

> This section intentionally describes generic folder roles (not feature-specific folders like `User`, `Album`, `Artist`, etc.).

- `Musicx.Contracts/`
    - DTO contracts (`Dto/Requests`, `Dto/Responses`) + enums.
    - Dependency direction: consumed by almost all layers.

- `Musicx.Application.Shared/`
    - Shared interfaces, helpers, enums, common models.
    - Dependency direction: used by API/Web/Desktop apps and infrastructure.

- `Musicx.Application.Api/`
    - API-oriented interfaces, data-view contracts, repository abstractions, query contracts.
    - Dependency direction: implemented by `Musicx.Infrastructure/API`.

- `Musicx.Application.Web/`
    - Web-specific view models/interfaces.
    - Dependency direction: consumed by UI and implemented through infrastructure/client services.

- `Musicx.Infrastructure/`
    - Concrete implementations for API persistence, shared clients/services, desktop persistence.
    - Subareas:
        - `API/`: columns/sql/schema/builders/mappers/repositories/auth/storage;
        - `Shared/`: ApiClient, localization, logging, reusable use-cases;
        - `Web/`: web-specific helpers;
        - `Desktop/`: desktop persistence/services.

- `Musicx.Presentation.Web/`
    - Hosting app, middleware, API controllers, server-side composition.

- `Musicx.Presentation.Web.Client/`
    - Blazor UI (pages/components/layout/modals/themes).

- `Musicx.Application.Desktop/`
    - Desktop use-cases and interfaces, aligned with cross-platform strategy.

- `Musicx.Tests/`
    - Automated tests.

- `docs/`
    - Human-readable architecture/design/guideline documentation.

---

## 8. Coding Rules and Conventions

### 8.1 API Documentation and Visibility

- All API-side classes and functions must be XML documented.
- Include purpose, and when relevant: parameters, return, throws, `<see>` links.
- Always include `<since>` with the version introducing the class/function.
- In API projects, everything that does not need public exposure must be `internal`.
- Classes must be `sealed` whenever inheritance is not intended.

### 8.2 Control Flow and Safety

- Never use single-line statements without braces.
- Prefer guard clauses.
- Use `is` / `is not` for null checks (not `== null` / `!= null`).
- In conditions, keep constant/value first when applicable (`if (5 == rating)`).
- For methods with many parameters: always use named arguments and put one argument per line.

### 8.3 Models and File Organization

- One model class per file.
- View-only models must be separated from code-behind.
- Reusable generic functions must be placed in services/helpers, not code-behind.
- Static/reusable content must not be hardcoded in code-behind or random classes; use helper/service abstractions.

### 8.4 Razor and UI Structure

- Use code-behind by default.
- If a view is very short (< 50 lines), inline logic may be accepted; otherwise split to `.razor.cs`.
- For heavy views, implement DataViewBuilder-oriented flow (and optional MVVM if code is large).
- In Razor markup:
    - separate child components with one empty line for readability;
    - comment every major section.

### 8.5 Front-End Assets

- JavaScript must go in `wwwroot/Js`.
- Reusable/generic CSS must go in `wwwroot/Css`.
- Do not create `*.razor.css` for MudBlazor usage.
    - Use `<style>` blocks in razor when local styling is enough.
    - For advanced css directives (`@media`, `@keyframes`, etc.), create dedicated files in `wwwroot/Css/Features`.

### 8.6 Localization and Text

- Always use `ITranslationService` for displayed text.
- Always update `Translations.resx` (and localized resource files when needed).
- Translation key format: `AppType.Page.Function` (example: `Web.UserEditor.UserSaved`).

### 8.7 API Client and Endpoint Governance

- Every new endpoint must be implemented in `ApiClient`.
- Endpoint controllers (except very generic read-only cases) must validate user permissions.
- Log all endpoint/service flows:
    - **INFO** with leading emoji for readability;
    - **DEBUG** with explicit action details.
- Ensure exceptions can be caught and handled in services/endpoints.
- Make SQL procedures if the statement takes more than 10 lines to avoid repository flood.

### 8.8 UX Feedback and Errors

- User must be informed through Snackbar:
    - failure -> error,
    - success -> success,
    - partial failure -> warning,
    - plus any useful contextual information.

### 8.9 Code-Behind Required Ordering

When using code-behind, organize members in this order:

1. `[Parameter]` first.
2. Attributes grouped by utility:
    - status booleans,
    - component references,
    - lists/data models, etc.
3. Lifecycle methods:
    - `OnInitialized`, `OnParametersSet`, `OnAfterRender`, ...
4. `Load` methods.
5. Business functions (move to ViewModel if too many).
6. Setters (`ToggleXxx`, `SetXxx`, ...).
7. Event handlers (`OnXxxClicked`, `OnChange`, ...).
8. `Save` methods.
9. `Clean` methods.

### 8.10 Data Types and Naming

- Prefer `IReadOnlyList<T>` over `List<T>` on API side.
- Non-nullable string default value must be `string.Empty`.
- Use PascalCase naming.

---

## 9. Definition of Done Checklist

Before merging, confirm:

- [ ] Contracts updated (`In*` / `Out*`) when needed.
- [ ] Repository interfaces/specifications/find-query updated.
- [ ] Infrastructure chain implemented (columns, mapper, sql builder, repository).
- [ ] Controller endpoint implemented with logging + permission checks.
- [ ] `ApiClient` updated for endpoint consumption.
- [ ] UI flow wired with localized texts (`ITranslationService` + `.resx`).
- [ ] Snackbar feedback added for success/warning/error states.
- [ ] Code-behind and view models split correctly.
- [ ] XML documentation includes `<since>`.
- [ ] Conventions validated (braces, guard clauses, null checks, sealed/internal, file organization).

---

If an exceptional case requires a rule break, document the reason clearly in the PR description and keep the deviation scoped.