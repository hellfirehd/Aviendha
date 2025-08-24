# Release Process

This document outlines the release process for the Aviendha framework using GitFlow-style development workflow.

## Overview

The Aviendha framework uses [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic semantic versioning and GitHub Actions for CI/CD with a GitFlow-style branching strategy.

## Branching Strategy

### Branch Types
- **`main`** - Production-ready code only (stable releases)
- **`develop`** - Integration and active development (preview packages)
- **`release/*`** - Release preparation branches (release candidate packages)
- **`hotfix/*`** - Emergency fixes for production (hotfix preview packages)
- **`feature/*`** - Individual features (build and test only)

### Version Types

#### Preview Releases (Automatic)
- **Trigger**: Pushes to `develop`, `release/*`, or `hotfix/*` branches
- **Version**: 
  - `X.Y.Z-alpha.N` (from develop)
  - `X.Y.Z-rc.N` (from release branches)
  - `X.Y.Z-beta.N` (from hotfix branches)
- **Workflow**: `.github/workflows/ci.yml`
- **NuGet**: Published automatically to https://www.nuget.org

#### Stable Releases (Automatic)
- **Trigger**: Pushes to `main` branch or Git tags matching `v*`
- **Version**: `X.Y.Z`
- **Workflow**: `.github/workflows/ci.yml`
- **NuGet**: Published to https://www.nuget.org
- **GitHub**: Creates a GitHub release automatically

#### Manual Preview Releases
- **Trigger**: Manual workflow dispatch
- **Version**: Based on selected branch
- **Workflow**: `.github/workflows/manual-preview.yml`
- **NuGet**: Published to https://www.nuget.org
- **Default Branch**: `develop`

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

## GitFlow Script Usage

The `scripts/GitFlow.ps1` script automates the GitFlow workflow and integrates seamlessly with Nerdbank.GitVersioning. This script handles all branch management, merging, tagging, and cleanup operations.

### Script Prerequisites

- PowerShell (Core or Windows PowerShell)
- Git installed and configured
- Access to the repository with appropriate permissions
- Nerdbank.GitVersioning (automatically installed by the script if missing)

### Available Actions

#### Version Information
```powershell
# Display current version and branch information
./scripts/GitFlow.ps1 -Action get-version
```

#### Feature Development
```powershell
# Start a new feature branch from develop
./scripts/GitFlow.ps1 -Action start-feature -Name "awesome-new-feature"

# Finish feature branch (merge to develop and cleanup)
./scripts/GitFlow.ps1 -Action finish-feature -Push
```

#### Release Management
```powershell
# Start a release branch from develop
./scripts/GitFlow.ps1 -Action start-release -Version "2.1.0" -Push

# Finish release (merge to main and develop, create tag)
./scripts/GitFlow.ps1 -Action finish-release -Push
```

#### Hotfix Management
```powershell
# Start a hotfix branch from main
./scripts/GitFlow.ps1 -Action start-hotfix -Name "critical-bug-fix"

# Finish hotfix (merge to main and develop, create tag)
./scripts/GitFlow.ps1 -Action finish-hotfix -Push
```

#### Manual Tagging
```powershell
# Create a version tag manually (typically from main)
./scripts/GitFlow.ps1 -Action create-tag -Push
```

### Script Parameters

- **`-Action`**: Required. The action to perform (see actions above)
- **`-Name`**: Branch name for features and hotfixes
- **`-Version`**: Version number for release branches (e.g., "2.1.0")
- **`-Push`**: Push changes to remote repository
- **`-DryRun`**: Show what would be done without executing commands
- **`-Force`**: Override safety checks (use with caution)

### Safety Features

The script includes several safety mechanisms:
- **Clean working directory check**: Ensures no uncommitted changes
- **Branch validation**: Confirms you're on the correct branch for operations
- **Dry-run mode**: Preview operations without making changes
- **Automatic cleanup**: Removes merged branches automatically
- **Release notes generation**: Creates commit-based release notes for tags

## Development Workflow

### Daily Development with GitFlow Script

#### Working on the Develop Branch
```powershell
# Check current version and branch
./scripts/GitFlow.ps1 -Action get-version

# Work directly on develop for small changes
# ... make changes, commit ...
git push origin develop  # 🚀 Triggers alpha preview packages
```

#### Working with Feature Branches
```powershell
# Start a new feature
./scripts/GitFlow.ps1 -Action start-feature -Name "user-authentication"

# Develop your feature
# ... code, commit, push ...

# Finish the feature (merges to develop and cleans up)
./scripts/GitFlow.ps1 -Action finish-feature -Push
```

### Release Preparation with GitFlow Script

#### Creating a Release
```powershell
# Start release preparation
./scripts/GitFlow.ps1 -Action start-release -Version "2.1.0" -Push

# Make final adjustments, bug fixes
# ... code, commit, push ...

# Complete the release
./scripts/GitFlow.ps1 -Action finish-release -Push
```

This automatically:
- Merges release branch to `main`
- Creates a version tag with release notes
- Merges back to `develop`
- Cleans up the release branch
- Triggers stable release packages 🚀

### Hotfix Workflow with GitFlow Script

```powershell
# Start emergency hotfix
./scripts/GitFlow.ps1 -Action start-hotfix -Name "security-patch"

# Fix the critical issue
# ... code, commit, push ...

# Complete the hotfix
./scripts/GitFlow.ps1 -Action finish-hotfix -Push
```

This automatically:
- Merges hotfix to `main`
- Creates a version tag
- Merges to `develop`
- Cleans up the hotfix branch
- Triggers stable release packages 🚀

### Legacy Git Commands (Manual Workflow)

> **Note**: The GitFlow script is the recommended approach. These manual commands are provided for reference or when the script cannot be used.

<details>
<summary>Click to expand manual Git workflow commands</summary>

#### Manual Feature Workflow
```bash
# Create a feature branch
git checkout develop
git pull origin develop
git checkout -b feature/awesome-new-feature

# Develop your feature
# ... code, commit ...
git push origin feature/awesome-new-feature

# Create pull request to develop
# After approval and merge, delete feature branch
git checkout develop
git pull origin develop
git branch -d feature/awesome-new-feature
```

#### Manual Release Workflow
```bash
# Create release branch from develop
git checkout develop
git pull origin develop
git checkout -b release/v2.1.0
git push origin release/v2.1.0  # 🚀 Triggers RC preview packages

# Make final adjustments, bug fixes
# ... code, commit ...
git push origin release/v2.1.0  # 🚀 Triggers updated RC packages

# Merge release branch to main
git checkout main
git pull origin main
git merge release/v2.1.0
git push origin main  # 🚀 Triggers stable release packages + GitHub release

# Merge release branch back to develop
git checkout develop
git merge release/v2.1.0
git push origin develop

# Clean up release branch
git branch -d release/v2.1.0
git push origin --delete release/v2.1.0
```

#### Manual Hotfix Workflow
```bash
# Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-bug-fix
git push origin hotfix/critical-bug-fix  # 🚀 Triggers beta preview packages

# Fix the issue
# ... code, commit ...
git push origin hotfix/critical-bug-fix

# Merge to main
git checkout main
git merge hotfix/critical-bug-fix
git push origin main  # 🚀 Triggers stable release

# Merge to develop
git checkout develop
git merge hotfix/critical-bug-fix
git push origin develop

# Clean up
git branch -d hotfix/critical-bug-fix
git push origin --delete hotfix/critical-bug-fix
```

</details>

## Release Workflow

### Automatic Releases

#### Preview Packages
Preview packages are automatically created and published when:
- Pushing to `develop` branch (alpha versions)
- Pushing to `release/*` branches (release candidate versions)
- Pushing to `hotfix/*` branches (beta versions)

#### Stable Releases
Stable packages are automatically created and published when:
- Pushing to `main` branch
- Pushing git tags matching `v*` pattern

### Manual Preview Releases
1. Go to **Actions → Manual Preview Release**
2. Click **"Run workflow"**
3. Select branch (defaults to `develop`)
4. Choose whether to include symbol packages
5. Review and run

### Creating Tagged Releases

#### Using GitFlow Script (Recommended)
```powershell
# Create a version tag (typically from main branch)
./scripts/GitFlow.ps1 -Action create-tag -Push
```

#### Manual Tag Creation
```bash
# Ensure you're on main with the code you want to release
git checkout main
git pull origin main

# Create and push a version tag
git tag v2.1.2
git push origin v2.1.2  # 🚀 Triggers release pipeline
```

## Package Information

### Generated Packages
The framework generates multiple NuGet packages organized by module:

#### Framework Packages
- `Aviendha` - Core framework
- `Aviendha.Ddd.*` - Domain-driven design components
- `Aviendha.EntityFrameworkCore` - Entity Framework Core integration
- `Aviendha.Hosting` - Hosting extensions
- `Aviendha.Logging.*` - Logging components
- `Aviendha.Security` - Security components
- `Aviendha.OpenApi` - OpenAPI integration
- `Aviendha.OpenIddict` - OpenIddict integration

#### Module Packages
- `Aviendha.BillingManagement.*` - Billing management module
- `Aviendha.MailingListManagement.*` - Mailing list management module

### Package Metadata
All packages include:
- Proper semantic versioning via Nerdbank.GitVersioning
- Source link for debugging
- Symbols packages (`.snupkg`) when requested
- License and author information
- README and changelog references

## Troubleshooting

### Common Issues

1. **GitFlow Script Errors**:
   - Ensure PowerShell execution policy allows script execution: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
   - Verify you're in the repository root directory
   - Check that git is properly configured with user name and email
   - Ensure you have permissions to push to the remote repository

2. **Build Failures**: 
   - Check the **Actions** tab for detailed logs
   - Ensure all tests pass locally before pushing
   - Verify project references and dependencies

3. **NuGet Push Failures**: 
   - Verify `NUGET_API_KEY` secret is correctly configured
   - Check API key permissions on NuGet.org
   - Ensure package version doesn't already exist

4. **Version Conflicts**: 
   - Nerdbank.GitVersioning automatically handles versioning
   - Ensure proper branch structure for expected version increments
   - Check `version.json` configuration if custom versioning is needed

5. **Merge Conflicts**:
   - Always pull latest changes before creating branches
   - Use `git merge develop` to update feature branches
   - Resolve conflicts locally before pushing

6. **Working Directory Not Clean**:
   - Commit or stash all changes before running GitFlow operations
   - Use `-Force` parameter to override this check (not recommended)

### Debugging
- All workflows upload build artifacts for investigation
- Test results and coverage reports are available in workflow runs
- Detailed logs available in GitHub Actions for each step
- Use `-DryRun` parameter with GitFlow script to preview operations

## Best Practices

### GitFlow Script Usage
1. **Always use `-Push` parameter**: Include `-Push` in finish operations to automatically push changes
2. **Use meaningful names**: Choose descriptive names for features and hotfixes
3. **Check version first**: Run `get-version` action to understand current state
4. **Use dry-run for safety**: Use `-DryRun` to preview complex operations
5. **Keep branches focused**: Create single-purpose feature branches

### Branch Management
1. **Keep `main` stable**: Only merge tested, release-ready code
2. **Use `develop` for integration**: Merge feature branches here first
3. **Short-lived feature branches**: Create focused, single-purpose branches
4. **Regular merging**: Keep feature branches up-to-date with develop

### Testing Strategy
1. **All tests must pass**: No exceptions for merging to develop or main
2. **Code coverage**: Maintain high coverage with meaningful tests
3. **Multiple test types**: Unit tests, integration tests, and end-to-end tests
4. **Automated testing**: All branches run full test suite

### Versioning Best Practices
1. **Follow semantic versioning (SemVer)**:
   - MAJOR: Breaking changes
   - MINOR: New features (backward compatible)
   - PATCH: Bug fixes (backward compatible)

2. **Use preview releases for testing**: Test alpha/beta packages before stable release

3. **Document breaking changes**: Clearly communicate breaking changes in release notes

### Documentation
1. **Update CHANGELOG.md**: Document all significant changes for each release
2. **Keep README.md current**: Ensure installation and usage instructions are accurate
3. **API documentation**: Maintain XML documentation comments for public APIs
4. **Release notes**: Create meaningful GitHub release descriptions

### Module Development
Given the modular structure of Aviendha:
- **Test module compatibility**: Ensure framework changes don't break modules
- **Version alignment**: Keep module versions aligned with framework versions
- **Integration testing**: Test modules together, not just in isolation
- **Preview packages**: Use preview packages to test cross-module dependencies

## Quick Reference

### Common GitFlow Commands
```powershell
# Daily development
./scripts/GitFlow.ps1 -Action get-version
./scripts/GitFlow.ps1 -Action start-feature -Name "feature-name"
./scripts/GitFlow.ps1 -Action finish-feature -Push

# Release cycle
./scripts/GitFlow.ps1 -Action start-release -Version "X.Y.Z" -Push
./scripts/GitFlow.ps1 -Action finish-release -Push

# Emergency fixes
./scripts/GitFlow.ps1 -Action start-hotfix -Name "hotfix-name"
./scripts/GitFlow.ps1 -Action finish-hotfix -Push
```

### Workflow Triggers
| Action | Branch | Result |
|--------|--------|--------|
| Push to `develop` | `develop` | Alpha preview packages |
| Push to `release/*` | `release/vX.Y.Z` | RC preview packages |
| Push to `hotfix/*` | `hotfix/name` | Beta preview packages |
| Push to `main` | `main` | Stable release packages |
| Git tag `v*` | Any | Stable release packages |