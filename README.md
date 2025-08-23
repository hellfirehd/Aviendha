# Aviendha Modules

Aviendha Modules is a collection of modules and libraries that complement and extend the [ABP Framework](https://abp.io/). The modules follow
the best practices and recommendations from the ABP Framework and are designed to provide additional functionality and enhance the development
experience for .NET developers.

## Dependencies

* [ASP.NET Core Runtime 9.0+ Runtime](https://dotnet.microsoft.com/download/dotnet)
* [ABP Framework](https://abp.io/)

## Current State of the Project

### Modules

1. **Billing Management**
   * Provides invoicing, payment processing, and basic reporting capabilities.
   * Multi-tenant aware and integrates with the ABP Framework's feature management and settings.

2. **Mailing List Management**
   * Manages email subscriptions and integrates with third-party email marketing services such as Listmonk, Mailchimp, and ConstantContact.

### Architecture

The project follows a modular architecture based on the ABP Framework, with layers for Presentation, Application, Domain, and Infrastructure. It leverages Domain-Driven Design (DDD) principles and Command Query Responsibility Separation (CQRS).

### Development Environment

* Targeting .NET 9 with plans to adopt .NET 10 shortly after its release.
* IDEs: Visual Studio 2022 (preferred), VS Code, or Rider.
* Multi-platform support: Windows and Linux.

### Key Features

* All modules are Multi-Tenant aware.
* Event sourcing for core aggregate roots.
* Outbox pattern for integration events.
* Responsive Blazor WebApps for user interfaces.
* Secure and scalable architecture with observability features planned.

## Roadmap

1. **Complete Feature Development**
   * Finalize the implementation of the Billing Management and Mailing List Management modules.
   * Add unit and integration tests to ensure high code quality and coverage.

2. **Adopt Planned Upgrades**
   * Prepare for the migration to .NET 10 by ensuring compatibility and testing.

3. **Implement Observability**
   * Add logging, metrics, and tracing using Serilog and OpenTelemetry.
   * Set up dashboards for monitoring performance and error rates.

4. **Enhance Deployment Pipeline**
   * Finalize the CI/CD pipeline using GitHub Actions.
   * Automate release tagging and environment promotion.

5. **Address Open Questions**
   * Decide on tenant database provisioning automation.
   * Finalize the idempotency pattern for event consumers.
   * Evaluate the need for field-level encryption for sensitive data.

6. **Future Modules**
   * Begin planning and development for future modules, such as conference management, online training, and community building.

7. **Documentation**
   * Ensure all modules and features are well-documented for developers and end-users.

---

This README provides an overview of the current state of the project and outlines the next steps to ensure its successful development and deployment.
