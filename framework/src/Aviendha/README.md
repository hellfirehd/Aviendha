# Aviendha Meta Package

This is a meta NuGet package for the Aviendha framework. It references all core framework projects,
making it easy to consume the entire Aviendha platform with a single package reference.

## Included Packages
- Aviendha.Ddd.Domain
- Aviendha.Ddd.Domain.Shared
- Aviendha.Ddd.Application
- Aviendha.Ddd.Application.Contracts
- Aviendha.Logging
- Aviendha.Logging.Hosting
- Aviendha.Logging.WebAssembly
- Aviendha.OpenIddict
- Aviendha.EntityFrameworkCore
- Aviendha.Security
- Aviendha.Emailing
- Aviendha.Hosting
- Aviendha.Microservices
- Aviendha.IdGeneration
- Aviendha.OpenApi
- Aviendha.Core

## Usage
Add a reference to the `Aviendha` package in your project to automatically include all framework components.

## Important Notes
- **Do Not** use this package in library projects; it is intended for non-tiered application projects only.
- This package does **not** include any modules or application-specific code.
- Individual framework packages are still available for more granular control and should be used for library projects or tiered applications.

