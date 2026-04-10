# Musicx

Musicx is a modular music platform built with .NET and Blazor, designed to combine a rich exploratory UI with a structured API architecture.

## Project Summary

Musicx provides a central place to explore and manage music-oriented data such as artists, albums, songs, genres, user ratings, and editorial relationships.

The project is structured for long-term evolution:

- a layered architecture with clear separation between contracts, application abstractions, infrastructure, and presentation;
- a hybrid web model (`Musicx.Presentation.Web` + `Musicx.Presentation.Web.Client`) using interactive WebAssembly rendering;
- extensibility for additional targets such as Desktop (already present) and future Mobile.

## What You Can Do on the Site

Depending on permissions and context, users can:

- browse and search music entities (artists, albums, songs, genres, tags, etc.);
- visualize structured data with dedicated views and charts;
- consult detail pages and data views for richer contextual information;
- create or edit content through forms and admin/moderation interfaces;
- rate and annotate music entities (including user-specific attributes);
- access patch notes and authentication-related flows.

## Main Technical Foundations

- **.NET 9**
- **ASP.NET Core** (host + API controllers)
- **Blazor** (interactive WebAssembly rendering)
- **MudBlazor** ecosystem for UI components
- **Custom SQL builder/repository infrastructure** for persistence
- **Localization via `ITranslationService` + `.resx` resources**

## Documentation

- Development standards: [`docs/DevelopmentGuidelines.md`](docs/DevelopmentGuidelines.md)
- Design/UI tokens: [`docs/DesignSystem.md`](docs/DesignSystem.md)

## Solution Structure (High-Level)

- `Musicx.Contracts` — shared request/response DTOs and enums.
- `Musicx.Application.Shared` — shared interfaces, helpers, and common models.
- `Musicx.Application.Api` — API-side abstractions and query contracts.
- `Musicx.Application.Web` — web-facing view model layer.
- `Musicx.Infrastructure` — implementations (API, shared services, web helpers, desktop persistence).
- `Musicx.Presentation.Web` — ASP.NET host and API endpoints.
- `Musicx.Presentation.Web.Client` — Blazor client UI.
- `Musicx.Application.Desktop` — desktop use cases/interfaces.
- `Musicx.Tests` — automated tests.

---

For contribution expectations and coding conventions, start with the development guideline document.