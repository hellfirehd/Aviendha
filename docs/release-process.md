# Release Process

This document outlines the release process for the Aviendha framework.

## Overview

The Aviendha framework uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic semantic versioning and GitHub Actions for CI/CD.

## Version Types

### Preview Releases (Automatic)
- **Trigger**: Pushes to `main`, `develop`, or `release/*` branches
- **Version**: `X.Y.Z-preview.N` (from main), `X.Y.Z-alpha.N` (from develop)
- **Workflow**: `.github/workflows/ci.yml`
- **NuGet**: Published automatically to https://www.nuget.org

### Stable Releases
- **Trigger**: Git tags matching `v*` (e.g., `v2.1.2`)
- **Version**: `X.Y.Z`
- **Workflow**: `.github/workflows/release.yml`
- **NuGet**: Published to https://www.nuget.org
- **GitHub**: Creates a GitHub release

### Manual Preview Releases
- **Trigger**: Manual workflow dispatch
- **Version**: Based on selected branch
- **Workflow**: `.github/workflows/manual-preview.yml`
- **NuGet**: Published to https://www.nuget.org

## Setup Requirements

### GitHub Secrets
Configure these secrets in your GitHub repository:

1. `NUGET_API_KEY`: Your NuGet.org API key
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push new packages and package versions" permission
   - Add it to GitHub repository secrets

### GitHub Environments
Create these environments in your repository settings:

1. `nuget-preview`: For preview releases
2. `nuget-release`: For stable releases

You can add protection rules to these environments if needed.

## Release Workflow

### Creating Preview Releases

#### Automatic Preview Releases
1. Push changes to `main` or `develop` branch
2. CI pipeline runs automatically
3. If tests pass, preview packages are published to NuGet

#### Manual Preview Releases
1. Go to Actions → Manual Preview Release
2. Click "Run workflow"
3. Select branch and options
4. Review and run

### Creating Stable Releases

1. Ensure your changes are in the `main` branch
2. Create and push a version tag:
   ```bash
   git tag v2.1.2
   git push origin v2.1.2
   ```
3. The release pipeline will:
   - Build and test the code
   - Create NuGet packages
   - Publish to NuGet.org
   - Create a GitHub release

### Using the Release Script

A PowerShell script is provided to help manage releases:

```powershell
# Get current version
./scripts/release.ps1 -Action get-version

# Create a release tag (local only)
./scripts/release.ps1 -Action create-tag

# Create and push a release tag
./scripts/release.ps1 -Action create-tag -Push
```

## Package Information

### Generated Packages
The framework generates multiple NuGet packages:

- `Aviendha.*` - Core framework packages
- `Aviendha.BillingManagement.*` - Billing management module
- `Aviendha.MailingListManagement.*` - Mailing list management module

### Package Metadata
All packages include:
- Proper semantic versioning
- Source link for debugging
- Symbols packages (`.snupkg`)
- License and author information
- README and changelog

## Troubleshooting

### Common Issues

1. **Build Failures**: Check the Actions tab for detailed logs
2. **NuGet Push Failures**: Verify API key and permissions
3. **Version Conflicts**: Ensure unique version numbers

### Debugging
- All workflows upload build artifacts
- Test results and coverage reports are available
- Detailed logs in GitHub Actions

## Best Practices

1. **Branch Strategy**:
   - Use `main` for stable development
   - Use `develop` for experimental features
   - Use `release/vX.Y` branches for release preparation

2. **Testing**:
   - All tests must pass before publishing
   - Code coverage reports are generated
   - Both unit and integration tests are run

3. **Versioning**:
   - Follow semantic versioning (SemVer)
   - Use preview releases for testing
   - Only tag stable, tested code

4. **Documentation**:
   - Update CHANGELOG.md for each release
   - Ensure README.md is current
   - Document breaking changes clearly