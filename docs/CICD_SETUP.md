# CI/CD Setup Guide

## Quick Start

Your CI/CD pipeline is now configured! Here's what you need to do to get it working:

## 1. Configure NuGet API Key

1. Go to [NuGet.org API Keys](https://www.nuget.org/account/apikeys)
2. Create a new API key with these settings:
   - **Key Name**: `Aviendha-CI`
   - **Package Owner**: Select your account
   - **Scopes**: `Push new packages and package versions`
   - **Packages**: `*` (or specify `Aviendha.*` for more security)
3. Copy the generated API key

## 2. Add GitHub Secret

1. Go to your repository on GitHub
2. Navigate to Settings → Secrets and variables → Actions
3. Click "New repository secret"
4. Name: `NUGET_API_KEY`
5. Value: Paste your NuGet API key
6. Click "Add secret"

## 3. Create GitHub Environments (Optional but Recommended)

1. Go to Settings → Environments
2. Create two environments:
   - `nuget-preview` (for preview releases)
   - `nuget-release` (for stable releases)
3. For `nuget-release`, you can add protection rules like:
   - Required reviewers
   - Deployment branches (only allow from main/release branches)

## 4. Test the Pipeline

### Test Preview Release
1. Make a small change to any file
2. Commit and push to `main`:
   ```bash
   git add .
   git commit -m "Test CI/CD pipeline"
   git push origin main
   ```
3. Go to Actions tab and watch the CI Pipeline run
4. If successful, check NuGet.org for your preview packages

### Test Manual Preview Release
1. Go to Actions → Manual Preview Release
2. Click "Run workflow"
3. Leave default settings and click "Run workflow"
4. Monitor the workflow execution

## 5. Create Your First Stable Release

When you're ready for a stable release:

1. Update version in `version.json` if needed
2. Create and push a tag:
   ```bash
   git tag v2.1.2
   git push origin v2.1.2
   ```
3. The release pipeline will automatically run

## What's Configured

### Automatic Preview Releases
- ✅ Triggered on pushes to `main`, `develop`, `release/*`
- ✅ Runs tests on both Ubuntu and Windows
- ✅ Publishes preview packages to NuGet
- ✅ Generates version using GitVersioning

### Stable Releases
- ✅ Triggered by version tags (e.g., `v2.1.2`)
- ✅ Runs full test suite
- ✅ Publishes stable packages to NuGet
- ✅ Creates GitHub releases

### Manual Workflows
- ✅ Manual preview release from any branch
- ✅ Configurable options (symbols, branch selection)

## Package Versioning

Your packages will use these version patterns:
- **Main branch**: `2.1.1-preview.X`
- **Develop branch**: `2.1.1-alpha.X`
- **Release tags**: `2.1.1`

## Next Steps

1. Complete the NuGet API key setup above
2. Test with a small commit to `main`
3. Monitor the Actions tab for execution
4. Check NuGet.org for published packages
5. Create your first stable release when ready

## Need Help?

- Check the [Release Process Documentation](docs/RELEASE_PROCESS.md)
- Review workflow logs in the Actions tab
- Verify your NuGet API key permissions

Happy releasing! 🚀