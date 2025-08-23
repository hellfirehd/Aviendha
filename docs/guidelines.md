# Development Guidelines and Best Practices for Aviendha Modules

We are passionate developers specializing in .NET development using C#, Entity Framework Core, WebApi, and Blazor. We follow best practices and leverage Domain Driven Design (DDD) and Command Query Responsibility Separation (CQRS). Event sourcing is being considered for core aggregate roots, such as Member.

We are developing a set of modules compatible with the *ABP Framework*. The primary modules are as follows:

- **Billing Management**: Provides invoicing, payment processing, and basic reporting capabilities.
- **Mailing List Management**: Manages email subscriptions and integrates with third-party email marketing services such as [Listmonk](https://listmonk.app/), [Mailchimp](https://mailchimp.com/), and [ConstantContact](https://www.constantcontact.com/).

Future modules include:

- Conference management, scheduling, and registration
- Online training
- Community building through online messaging and/or forums.

## Development Environment

- Current Target: .NET 9 / ABP Framework 9.
- Planned Upgrade: Adopt .NET 10 within one minor release cycle after GA (aim ~4 weeks).
- Platform: Windows, Linux
- IDE: Visual Studio 2022 or later (preferred), VS Code with C# extension, or Rider.

## Architecture

The modules are developed using the *[ABP Framework](https://github.com/abpframework/abp)* from [https://abp.io].
The user interfaces will be developed as a Blazor WebApps, and the backend will be developed using ASP.NET Core Web API.
There are no plans for a mobile applications at this time. The web application must be responsive and work well on mobile devices, but only a subset of features will be available on mobile.

Following *ABP Framework* recommendations, each module is organized into several layers:

- **Presentation Layer**: Contains the user interface components, such as Razor pages, and Blazor components. This layer is responsible for handling user interactions and presenting data to the user. It may be a standalone web application, or it may be a class library that can be used by other applications.
- **Application Contracts Layer**: Contains the service interfaces, commands, queries, and data transfer objects (DTOs) that define the contracts for the application services. This layer is used to communicate between the presentation layer and the application layer.
- **Application Layer**: Contains the implementations of the interfaces and services defined in the application contracts layer. This layer orchestrates the domain logic and is responsible for application-specific operations. It is primarily exposed as a REST API for external clients.
- **Domain Shared Layer**: Contains the domain constants, enumerations, value objects, and domain events. This layer defines the contracts for the domain services and events that can be used by the application layer. Ideally, this layer should not depend on any other layers, but it can reference the *ABP Framework* libraries.
- **Domain Layer**: Contains the core business logic as Manager classes and domain entities and aggregate roots. This layer must remain independent of any external frameworks or libraries, other than what is provided by the *ABP Framework*.
- **Infrastructure Layer**: Contains implementations of interfaces defined in the application layer, such as repositories, external service integrations, and data access logic. This layer interacts with external systems and databases.

### Multi-Tenancy

Each module must be multi-tenant aware. The *ABP Framework* provides the infrastructure to enable multi-tenancy, but we must still carefully consider the impact of multi-tenancy on each module.

### Setting Management

We rely primarily on the `ISettingProvider` interface from `Volo.Abp.Settings`. This allows us to define the settings at various levels. 

### Domain & Integration Events

- In-Process Domain Events: For intra-context side effects (transactional).
- Integration Events: Published for cross-context/service consumers (outbox pattern).
- Candidate Events (examples): MemberStandingChanged, MembershipLevelUpgraded, ContinuingEducationRequirementSatisfied, AnnualRenewalCompleted, PaymentSucceeded, PaymentFailed.
- Event Versioning: Suffix incremental version (e.g., MemberStandingChanged.v1) if contract must evolve incompatibly.
- Idempotency: Outbox table (EF Core) with background dispatcher.
- Transport Decision: RabbitMQ (ABP native support) using Rebus for abstraction & handler pipeline.
- Message Semantics: At-least-once delivery; handlers must be idempotent (natural keys + processed message log or outbox dispatch status).

#### Idempotency Pattern Options (Evaluation Needed)

Option A: ProcessedMessages table (MessageId PK)

- Pros: Simple, explicit audit, independent of domain model
- Cons: Extra write per message; table growth (needs pruning/archive)

Option B: Aggregate version / natural key checks

- Pros: No separate tracking table; leverages concurrency control
- Cons: Harder for cross-aggregate events; less explicit audit trail

(Decision: TBD)

### Outbox Pattern (Planned Implementation)

- Storage: Same relational DB as primary write model initially.
- Table Columns: Id (GUID), AggregateId, EventType, Payload (JSON), OccurredUtc, ProcessedUtc, ProcessingAttempts, Status.
- Dispatcher Interval: Short polling (e.g., 2–5s) or hosted service leveraging NOTIFY/LISTEN (PostgreSQL) if adopted.
- Failure Policy: Exponential backoff; poison events moved after N attempts.
- Rebus Integration: Outbox records are persisted within the same transaction as aggregate changes; a background worker publishes messages to RabbitMQ via Rebus after commit.
- Duplicate Prevention: Publisher marks row as Processed (with concurrency token) after successful ACK from Rebus.
- Exactly-Once (Effective): Achieved logically via idempotent consumers + message de-duplication key (MessageId GUID).

### Persistence & Data Model Strategy

- Primary: EF Core with SQL Server or PostgreSQL (decision drivers below).
- Criteria:
  - PostgreSQL: Potential LISTEN/NOTIFY for outbox efficiency, rich JSONB for flexible read models.
  - SQL Server: Existing organizational expertise, integrated tooling, temporal tables for auditing.
- MongoDB (Optional): Consider only for
  - High-volume append-only audit/event archive.
  - Denormalized read models requiring flexible schema evolution.
  - Avoid premature polyglot: introduce after real performance/reporting pain.
- Migrations: EF Core migrations as source-controlled first-class artifacts.

### Security & AuthN/AuthZ

- Provided by the *ABP Framework*.
- Rely on `ICurrentPrincipalAccessor` and `ICurrentUser` for user context.
- Permissions: Use *ABP Framework* permission system for role-based access control.
- Ensure compliance with security best practices throughout the application lifecycle.

### Performance & Scaling Assumptions

- Current Users: ~1000 members; low concurrency (~10 simultaneous).
- Growth Expectation: Plan for 10x concurrency over 5 years.
- Baseline Targets (Initial Proposal):
  - P95 API latency: < 300 ms for standard CRUD, < 800 ms for complex aggregate operations.
  - Background Job SLA: Standing recalculation & renewal processing within 1 minute of triggering event.
- Capacity Planning: Begin with vertical scaling; introduce horizontal scaling + read replicas after sustained >50% CPU or I/O saturation.

### Observability (Planned)

- Logging: Structured JSON (Serilog) with correlation ID middleware.
- Metrics: Prometheus-compatible exporters (request latency, event dispatch lag).
- Tracing: OpenTelemetry (HTTP, EF Core, background jobs).
- Dashboards: Grafana (or Azure Monitor) for latency, error rate, event backlog.
- Serilog: Primary structured logger; Seq sink active in all non-production environments; production exports to OpenTelemetry via Serilog OpenTelemetry sink.
- OpenTelemetry: Unified traces (HTTP, EF Core, Rebus messaging); resource attributes include tenant id (when present) and correlation id.
- Correlation: Incoming request X-Correlation-ID or generated GUID; propagated through Rebus headers.

## Coding Standards

We follow the official Microsoft C# coding conventions with some additional guidelines:

- Use `var` for local variable declarations when the type is obvious from the right-hand side of the assignment. Otherwise, use explicit types.
  - Append `()` on new object instantiations when there are no parameters.
- Use expression-bodied members for simple properties and methods to improve readability.
- Use `async` and `await` for asynchronous programming to improve responsiveness and scalability.
- Avoid using `this.` unless necessary for disambiguation.
- Use `nameof` operator instead of hard-coded strings for member names.
- Use `string interpolation` instead of `string.Format` for better readability.
- Use `null-coalescing operator` (`??`) and `null-conditional operator` (`?.`) to simplify null checks.
- Use `switch` expressions for complex conditional logic to improve readability.
- Use `record` types for immutable data structures and DTOs.
- Use `readonly` aggressively for fields that should not change after initialization. It can always be removed later.
- Use `const` for compile-time constants and `readonly` for runtime constants.
- Use `IEnumerable<T>` for method return types when the caller does not need to modify the collection. Use `List<T>` or other concrete types when modification is required.
- Use `is` pattern matching for type checks and casting to improve readability.
- Follow SOLID principles and design patterns to ensure maintainable and extensible code.
  - Ensure code is clean, well-organized, and adheres to the Single Responsibility Principle
- Write XML documentation comments for public members to improve code maintainability and usability.
- Ensure proper exception handling and logging using the built-in logging framework provided by ASP.NET Core and ABP Framework.
  - Prefer throwing specific exception types over general ones.
  - Throw domain-specific exceptions for business rule violations.
  - Avoid catching exceptions unless you can handle them or add meaningful context.
- Use dependency injection to manage dependencies and promote testability.
- Follow the principles of Domain-Driven Design (DDD) to model the domain effectively.
- Use CQRS principles to separate read and write operations when appropriate.
- Use meaningful names for variables, methods, and classes to improve code readability.
- Avoid deep nesting of code blocks by using early returns and guard clauses.
- Keep methods short and focused on a single task.
- Avoid using regions. Instead, prefer partial classes for large classes with multiple related members.
- Use LINQ for collection manipulation and querying, but avoid overly complex queries that are hard to read.
  - Prefer method syntax over query syntax for better readability, but use query syntax when it improves clarity.
- Use comments sparingly. The code should be self-explanatory. Use comments to explain the "why" behind complex logic, not the "what".
- Regularly refactor code to improve its structure and maintainability.
- Use code analysis tools like Roslyn analyzers and StyleCop to enforce coding standards and catch potential issues early.
- Ideally, write unit tests before writing the actual code (Test-Driven Development - TDD) to ensure code quality and correctness.
- Always write tests for bug fixes to prevent regressions.
- Obsolete APIs must be decorated with [Obsolete] after replacement exists for one full minor cycle.

## Naming Conventions

- Use descriptive names that clearly indicate the purpose of the class, method, or variable.
- Use `PascalCase` for class names, method names, properties, and static members or consts.
- Use `camelCase` for local variables and method parameters.
- Use `_camelCase` for private fields.
- Use `Async` suffix for asynchronous methods.
- Commands sent to the application layer should be named using the verb-noun pattern (e.g., `CreateMember`, `UpdateMembership`). Do not use the `Command` suffix or `Dto` suffix.
- Queries sent to the application layer should be named using the noun-verb pattern (e.g., `GetMemberById`, `ListActiveMembers`). Do not use the `Query` suffix or `Dto` suffix.
- Aggregate Roots and Domain entities should be named using the noun pattern (e.g., `Member`, `Membership`, `Payment`). Do not use the `Entity` suffix.
- Value objects should be named using the noun pattern (e.g., `EmailAddress`, `PhoneNumber`, `Address`). Do not use the `ValueObject` suffix.
- Responses from the application layer should be named using the noun pattern suffixed with `Dto` (e.g., `MemberDto`, `InvoiceDto`). Do not use the `Response` suffix.
- Avoid using abbreviations in names to enhance clarity.

## Testing Guidelines

**Important: All tests must use the Shouldly and Moq libraries.**

When writing tests, please follow these guidelines:

- Use the `Xunit` testing framework for unit tests.
- Each test method should be decorated with the `[Fact]` attribute for simple tests or `[Theory]` with `[InlineData]` for parameterized tests.
- Group related tests into test classes that correspond to the class or functionality being tested. Each test class should be named after the class it tests, with a `_Tests` suffix (e.g., `UserService_Tests` for testing `UserService`).
- Use descriptive names for test methods that clearly indicate what is being tested and the expected outcome. Underscore `_` can be used to separate different parts of the name for better readability.
- Tests can be placed in abstract base classes if they share common setup or utility methods, or if there are multiple implementations of an interface or service to be tested.
  - For example, many services depend on a repository interface. You can create an abstract base test class that defines the tests the repository should pass, and then create concrete test classes for each implementation of the repository interface.
- Tests should be structured in one of the two following ways:
  - Arrange, Act, Assert (AAA) pattern should be followed for unit testsin each test method to structure the code clearly.
  - Given, When, Then pattern can be used for better readability in complex scenarios, particularly in behavior-driven development (BDD) style tests, or integration tests.
- Use Shouldly assertions to verify the expected outcomes. 
  - For example, use `result.ShouldBe(expectedValue)` instead of `Assert.Equal(expectedValue, result)`.
- Multiple assertions in a single test are okay as long as they are testing the same behavior. Each test should ideally verify one specific behavior or outcome.
- Ensure that tests are isolated and do not depend on external systems or state. Use mocking to simulate interactions with dependencies.
- Avoid using hard-coded values in tests. Instead, use constants or variables to improve maintainability. Values should not be shared between test classes unless they are in a common base class.
- When mocking dependencies with Moq, set up the mock behavior clearly and verify interactions where necessary.
- When testing for exceptions, use the `Should.Throw<TException>` method to assert that the expected exception is thrown.
- Aim for high code coverage, but prioritize meaningful tests that validate the behavior of the code over achieving a specific coverage percentage.
- Regularly review and refactor tests to maintain clarity and effectiveness as the codebase evolves.
- Ensure that tests are fast and efficient. Avoid long-running operations or unnecessary delays in test execution
- Use comments sparingly in tests. The test code itself should be clear enough to convey its purpose without extensive comments.

## Compliance & Data Governance (Canada)

The modules are primarily designed for Canadian users, so we must ensure compliance with Canadian data protection and privacy regulations, such as 
PIPEDA (Personal Information Protection and Electronic Documents Act) and provincial privacy laws. This particularly applicable to the Billing Management module, which will handle personal information and financial data.

## Data Classification & Encryption Strategy (Draft)

- Classification Levels:
  - Public: Directory profile (limited fields explicitly approved).
  - Internal: Operational membership data (CE credits, renewal workflow states).
  - Confidential: Disciplinary actions, payment transaction references (no PAN), audit logs.
- Encryption At Rest: RDBMS native encryption (provider-managed keys) acceptable initially.
- Key Management: Decision pending on moving to customer-managed keys (BYOK) for higher assurance or tenant-specific keys.
- In-Transit: Enforce TLS 1.3; HSTS enabled on public endpoints.
- Field-Level Encryption (Optional Future): Consider for particularly sensitive narrative fields (e.g., supervision notes) if regulatory pressure increases.

## Deployment

### Release Tagging & Promotion

1. Ensure main is green (CI) and local main is up to date.
2. Run TagRelease.ps1 (default creates next semantic version tag).
3. Push tag (script does this). CI can auto-deploy tag to staging; manual approval promotes same tag to production.
4. Hotfix: branch from production, tag, apply fix, merge to main, re-run TagRelease.ps1.
5. Rollback: redeploy previous production tag (schema changes must be backward compatible).

Release notes: Generated from commits since prior tag (first line only). Commit message convention: <type>(context): summary (e.g., feat(membership): add renewal eligibility check).

### Branching & Release Strategy (Simplified)

- Branches: main (always deployable), short-lived feat/*, fix/*, hotfix/*.
- Merge cadence: < 1 day open; use feature flags for incomplete work.
- Hotfix: hotfix/* from current production tag → PR → merge → new tag → deploy.
- Tags: CalVer rel-YYYY.MM.DD.N (N increments per date). Tags drive staging & production promotions.
- No long-lived release or develop branches.

### Containerization & Deployment (Draft)

- Packaging: Container images (multi-stage builds) for API, Background Worker, Identity/Authority.
- Orchestration (TBD): Evaluate Azure Container Apps (cost efficiency) vs AKS (control) vs simple App Service for early stage.
- Environment Promotion: Dev → Staging → Prod with immutable image tags (git commit hash).
- Config: 12-factor; secrets via managed secret store (e.g., Azure Key Vault).

### CI/CD Roadmap (Initial)

- Platform (Proposed): GitHub Actions.
- Stages:
  1. Build & Restore (dotnet restore, deterministic builds).
  2. Static Analysis (Roslyn analyzers, StyleCop).
  3. Tests (unit + integration) with coverage (coverlet) threshold (initial 70%).
  4. Security Scan (dependency review, container image scan).
  5. Package & Push Image.
  6. Migration Dry Run (validate EF migrations).
  7. Deploy (staged).
- Branch Strategy: (Proposed) Trunk-based with short-lived feature branches; semantic version tags for public API/clients.

## Feature Flag Guidelines

Purpose: Keep main shippable; hide incomplete or risky changes.

Flag store: ABP Feature Management (database-backed). Access via IFeatureChecker / ABP feature system (or wrapper).

### Naming

- Pattern: `Context.Area.Name`
- Context examples: Membership, Compliance, Payments, Renewals, Resources
- Example: Membership.RenewalStreamlining, Payments.NewProvider
- Use PascalCase segments; avoid abbreviations.

### Flag Categories

- Release: Gradually enable new feature.
- Ops (Kill Switch): Disable problematic integration quickly.
- Permission Extension: Temporary gating until formal permission added.
- Experiment: A/B or progressive rollout (future).
- Migration: Controls dual-path logic; removed after cutover.

### Lifecycle

1. Add flag (default Off in all envs except local).
2. Implement code paths (guarded).
3. Enable in dev → staging → production.
4. Monitor metrics/logs.
5. Remove dead code & flag within 2 release tags after fully On (except persistent ops flags).

### Implementation Rules

- Evaluation at boundary (application service / UI) if it shifts whole feature visibility; deeper conditional only when behavior diverges internally.
- Never branch across transactional integrity (avoid partially applying domain changes based on flag mid-transaction).
- Tests: Provide both flag states for core logic (use fixture to inject IFeatureChecker mock).
- Avoid nesting >1 level of flags in same method; refactor to strategy classes if complexity rises.
- Configuration caching: Respect dynamic nature; do not hard-cache flag values for process lifetime unless category=Migration and documented.

### Anti-Patterns

- Using flags instead of proper versioned contracts.
- Leaving flags enabled for > 60 days without removal plan.
- Sharing flag names between bounded contexts.

### Open To-Do

- Add automated report listing stale flags (> 30 days fully On).
- Progressive rollout tooling (percentage / targeting) (deferred).

## Risk & Technical Debt Register (Seed)    

| Risk | Impact | Mitigation |
|------|--------|------------|
| Early polyglot persistence | Complexity | Defer Mongo until justified |
| Eventual consistency surprises | Data anomalies | Clear consumer retry & idempotency strategy |
| Missing multi-tenancy boundaries | Costly retrofit | Enforce tenant-agnostic abstractions now |
| Compliance retention gaps | Legal exposure | Implement retention/archival scheduler early |

## Open Questions (To Be Finalized)

1. Tenant DB provisioning automation tool choice (custom script, ABP extension, or orchestration service).
2. Event consumer idempotency pattern (store processed MessageId vs natural aggregate version checks) – finalize.
3. BYOK adoption timeline & whether tenant-scoped keys are required.
4. Exact archival approach: purge vs anonymize after retention window.
5. Field-level encryption necessity for confidential narrative fields.
