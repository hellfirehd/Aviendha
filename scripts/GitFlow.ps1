#!/usr/bin/env pwsh

<#
.SYNOPSIS
    GitFlow workflow management script for Aviendha framework
.DESCRIPTION
    This script helps manage the GitFlow development workflow, including branch management,
    version bumping, tagging, and integration with Nerdbank.GitVersioning.
.PARAMETER Action
    The action to perform:
    - get-version: Display current version information
    - start-feature: Create and switch to a new feature branch
    - finish-feature: Merge feature branch to develop and clean up
    - start-release: Create a release branch from develop
    - finish-release: Merge release branch to main and develop, create tag
    - start-hotfix: Create a hotfix branch from main
    - finish-hotfix: Merge hotfix branch to main and develop, create tag
    - create-tag: Create a version tag (manual tagging)
.PARAMETER Name
    Branch name (for start-feature, start-release, start-hotfix)
.PARAMETER Version
    Version for release/hotfix branches (e.g., "2.1.0")
.PARAMETER Push
    Whether to push changes to remote repository
.PARAMETER DryRun
    Show what would be done without actually executing commands
.PARAMETER Force
    Force operations (e.g., recreate existing tags, force push)
.EXAMPLE
    ./scripts/GitFlow.ps1 -Action get-version
.EXAMPLE
    ./scripts/GitFlow.ps1 -Action start-feature -Name "awesome-new-feature"
.EXAMPLE
    ./scripts/GitFlow.ps1 -Action start-release -Version "2.1.0" -Push
.EXAMPLE
    ./scripts/GitFlow.ps1 -Action finish-release -Push
.EXAMPLE
    ./scripts/GitFlow.ps1 -Action create-tag -Push
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("get-version", "start-feature", "finish-feature", "start-release", "finish-release", 
                 "start-hotfix", "finish-hotfix", "create-tag")]
    [string]$Action,
    
    [string]$Name,
    [string]$Version,
    [switch]$Push,
    [switch]$DryRun,
    [switch]$Force
)

# Color functions for better output
function Write-Success { param($Message) Write-Host $Message -ForegroundColor Green }
function Write-Info { param($Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host $Message -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host $Message -ForegroundColor Red }

# Git execution wrapper
function Invoke-Git {
    param([string[]]$Arguments, [switch]$ThrowOnError = $true)
    
    if ($DryRun) {
        Write-Info "[DryRun] git $($Arguments -join ' ')"
        return ""
    }
    
    $result = & git @Arguments 2>&1
    if ($LASTEXITCODE -ne 0 -and $ThrowOnError) {
        throw "Git command failed (exit $LASTEXITCODE): git $($Arguments -join ' ')`n$result"
    }
    return $result
}

# Ensure we're in a git repository
try {
    $repoRoot = git rev-parse --show-toplevel 2>$null
    if ($LASTEXITCODE -ne 0) { throw }
    Set-Location $repoRoot
} catch {
    Write-Error "Not in a git repository"
    exit 1
}

# Install/check for nbgv
function Ensure-Nbgv {
    if (!(Get-Command nbgv -ErrorAction SilentlyContinue)) {
        Write-Info "Installing Nerdbank.GitVersioning..."
        if (-not $DryRun) {
            dotnet tool install -g nbgv
        }
    }
}

# Get version information from nbgv
function Get-VersionInfo {
    Ensure-Nbgv
    if ($DryRun) {
        return @{ Version = "1.0.0-alpha.42"; SimpleVersion = "1.0.0" }
    }
    
    $json = nbgv get-version --format json 2>$null
    if (-not $json) { throw "Failed to get version from nbgv" }
    return $json | ConvertFrom-Json
}

# Get current branch
function Get-CurrentBranch {
    return git rev-parse --abbrev-ref HEAD 2>$null
}

# Check if branch exists locally
function Test-LocalBranch {
    param($BranchName)
    $result = git rev-parse --verify "$BranchName" 2>$null
    return $LASTEXITCODE -eq 0
}

# Check if branch exists on remote
function Test-RemoteBranch {
    param($BranchName)
    $result = git ls-remote --heads origin "$BranchName" 2>$null
    return $result -and $result.Length -gt 0
}

# Ensure working directory is clean
function Assert-CleanWorkingDirectory {
    $status = git status --porcelain 2>$null
    if ($status) {
        Write-Warning "Working directory is not clean:"
        Write-Host $status
        if (-not $Force) {
            throw "Working directory must be clean. Use -Force to ignore this check."
        }
    }
}

# Create release notes
function New-ReleaseNotes {
    param($TagName, $PreviousTag)
    
    $commits = if ($PreviousTag) {
        git log --pretty=format:"%h %s" "$PreviousTag..HEAD" 2>$null
    } else {
        git log --pretty=format:"%h %s" 2>$null
    }
    
    $now = Get-Date -AsUTC
    $notes = @"
Release $TagName
Date: $($now.ToString('u'))
Source: GitFlow workflow with Nerdbank.GitVersioning

Commits:
$($commits -join "`n")
"@
    return $notes
}

# Main action handlers
switch ($Action) {
    "get-version" {
        $versionInfo = Get-VersionInfo
        Write-Success "Current Version Information:"
        Write-Host "  Version: $($versionInfo.Version)" -ForegroundColor White
        Write-Host "  SimpleVersion: $($versionInfo.SimpleVersion)" -ForegroundColor White
        Write-Host "  Branch: $(Get-CurrentBranch)" -ForegroundColor White
        
        if ($versionInfo.PrereleaseVersion) {
            Write-Host "  Prerelease: $($versionInfo.PrereleaseVersion)" -ForegroundColor Yellow
        }
    }
    
    "start-feature" {
        if (-not $Name) { throw "Feature name is required. Use -Name parameter." }
        
        Assert-CleanWorkingDirectory
        
        $featureBranch = "feature/$Name"
        Write-Info "Starting feature branch: $featureBranch"
        
        # Ensure we're on develop and it's up to date
        Invoke-Git @("checkout", "develop")
        Invoke-Git @("pull", "origin", "develop")
        
        # Create and switch to feature branch
        Invoke-Git @("checkout", "-b", $featureBranch)
        
        if ($Push) {
            Invoke-Git @("push", "-u", "origin", $featureBranch)
        }
        
        Write-Success "Feature branch '$featureBranch' created successfully!"
        Write-Info "You can now work on your feature. When done, run:"
        Write-Info "  ./scripts/GitFlow.ps1 -Action finish-feature"
    }
    
    "finish-feature" {
        $currentBranch = Get-CurrentBranch
        if (-not $currentBranch.StartsWith("feature/")) {
            throw "Not on a feature branch. Current branch: $currentBranch"
        }
        
        Assert-CleanWorkingDirectory
        
        Write-Info "Finishing feature branch: $currentBranch"
        
        # Switch to develop and ensure it's up to date
        Invoke-Git @("checkout", "develop")
        Invoke-Git @("pull", "origin", "develop")
        
        # Merge feature branch
        Invoke-Git @("merge", "--no-ff", $currentBranch)
        
        if ($Push) {
            Invoke-Git @("push", "origin", "develop")
        }
        
        # Clean up feature branch
        Invoke-Git @("branch", "-d", $currentBranch)
        
        if ($Push -and (Test-RemoteBranch $currentBranch)) {
            Invoke-Git @("push", "origin", "--delete", $currentBranch)
        }
        
        Write-Success "Feature branch '$currentBranch' merged to develop and cleaned up!"
    }
    
    "start-release" {
        if (-not $Version) { throw "Version is required for release branch. Use -Version parameter (e.g., '2.1.0')." }
        
        Assert-CleanWorkingDirectory
        
        $releaseBranch = "release/v$Version"
        Write-Info "Starting release branch: $releaseBranch"
        
        # Ensure we're on develop and it's up to date
        Invoke-Git @("checkout", "develop")
        Invoke-Git @("pull", "origin", "develop")
        
        # Create and switch to release branch
        Invoke-Git @("checkout", "-b", $releaseBranch)
        
        if ($Push) {
            Invoke-Git @("push", "-u", "origin", $releaseBranch)
        }
        
        Write-Success "Release branch '$releaseBranch' created successfully!"
        Write-Info "You can now make final adjustments. When ready, run:"
        Write-Info "  ./scripts/GitFlow.ps1 -Action finish-release -Push"
    }
    
    "finish-release" {
        $currentBranch = Get-CurrentBranch
        if (-not $currentBranch.StartsWith("release/")) {
            throw "Not on a release branch. Current branch: $currentBranch"
        }
        
        Assert-CleanWorkingDirectory
        
        Write-Info "Finishing release branch: $currentBranch"
        
        # Extract version from branch name
        if ($currentBranch -match "release/v(.+)") {
            $releaseVersion = $matches[1]
        } else {
            throw "Cannot extract version from branch name: $currentBranch"
        }
        
        # Merge to main
        Invoke-Git @("checkout", "main")
        Invoke-Git @("pull", "origin", "main")
        Invoke-Git @("merge", "--no-ff", $currentBranch)
        
        # Create tag
        $versionInfo = Get-VersionInfo
        $tagName = "v$($versionInfo.SimpleVersion)"
        $previousTag = git tag --sort=-creatordate --list "v*" 2>$null | Select-Object -First 1
        $releaseNotes = New-ReleaseNotes -TagName $tagName -PreviousTag $previousTag
        
        $tempFile = New-TemporaryFile
        try {
            Set-Content -Path $tempFile -Value "$tagName`n`n$releaseNotes" -Encoding UTF8
            Invoke-Git @("tag", "-a", $tagName, "-F", $tempFile.FullName)
        } finally {
            Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
        }
        
        # Merge back to develop
        Invoke-Git @("checkout", "develop")
        Invoke-Git @("pull", "origin", "develop")
        Invoke-Git @("merge", "--no-ff", $currentBranch)
        
        if ($Push) {
            Invoke-Git @("push", "origin", "main")
            Invoke-Git @("push", "origin", "develop")
            Invoke-Git @("push", "origin", $tagName)
        }
        
        # Clean up release branch
        Invoke-Git @("branch", "-d", $currentBranch)
        if ($Push -and (Test-RemoteBranch $currentBranch)) {
            Invoke-Git @("push", "origin", "--delete", $currentBranch)
        }
        
        Write-Success "Release '$tagName' completed successfully!"
        Write-Info "Release packages will be automatically built and published to NuGet."
    }
    
    "start-hotfix" {
        if (-not $Name) { throw "Hotfix name is required. Use -Name parameter." }
        
        Assert-CleanWorkingDirectory
        
        $hotfixBranch = "hotfix/$Name"
        Write-Info "Starting hotfix branch: $hotfixBranch"
        
        # Create hotfix branch from main
        Invoke-Git @("checkout", "main")
        Invoke-Git @("pull", "origin", "main")
        Invoke-Git @("checkout", "-b", $hotfixBranch)
        
        if ($Push) {
            Invoke-Git @("push", "-u", "origin", $hotfixBranch)
        }
        
        Write-Success "Hotfix branch '$hotfixBranch' created successfully!"
        Write-Info "You can now fix the issue. When done, run:"
        Write-Info "  ./scripts/GitFlow.ps1 -Action finish-hotfix -Push"
    }
    
    "finish-hotfix" {
        $currentBranch = Get-CurrentBranch
        if (-not $currentBranch.StartsWith("hotfix/")) {
            throw "Not on a hotfix branch. Current branch: $currentBranch"
        }
        
        Assert-CleanWorkingDirectory
        
        Write-Info "Finishing hotfix branch: $currentBranch"
        
        # Merge to main
        Invoke-Git @("checkout", "main")
        Invoke-Git @("pull", "origin", "main")
        Invoke-Git @("merge", "--no-ff", $currentBranch)
        
        # Create tag
        $versionInfo = Get-VersionInfo
        $tagName = "v$($versionInfo.SimpleVersion)"
        $previousTag = git tag --sort=-creatordate --list "v*" 2>$null | Select-Object -First 1
        $releaseNotes = New-ReleaseNotes -TagName $tagName -PreviousTag $previousTag
        
        $tempFile = New-TemporaryFile
        try {
            Set-Content -Path $tempFile -Value "$tagName`n`n$releaseNotes" -Encoding UTF8
            Invoke-Git @("tag", "-a", $tagName, "-F", $tempFile.FullName)
        } finally {
            Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
        }
        
        # Merge to develop
        Invoke-Git @("checkout", "develop")
        Invoke-Git @("pull", "origin", "develop")
        Invoke-Git @("merge", "--no-ff", $currentBranch)
        
        if ($Push) {
            Invoke-Git @("push", "origin", "main")
            Invoke-Git @("push", "origin", "develop")
            Invoke-Git @("push", "origin", $tagName)
        }
        
        # Clean up hotfix branch
        Invoke-Git @("branch", "-d", $currentBranch)
        if ($Push -and (Test-RemoteBranch $currentBranch)) {
            Invoke-Git @("push", "origin", "--delete", $currentBranch)
        }
        
        Write-Success "Hotfix '$tagName' completed successfully!"
        Write-Info "Hotfix packages will be automatically built and published to NuGet."
    }
    
    "create-tag" {
        Assert-CleanWorkingDirectory
        
        $currentBranch = Get-CurrentBranch
        if ($currentBranch -ne "main") {
            Write-Warning "Not on main branch. Current branch: $currentBranch"
            if (-not $Force) {
                throw "Tags should typically be created from main branch. Use -Force to override."
            }
        }
        
        $versionInfo = Get-VersionInfo
        $tagName = "v$($versionInfo.SimpleVersion)"
        
        # Check if tag already exists
        $tagExists = git tag --list $tagName 2>$null
        if ($tagExists -and -not $Force) {
            throw "Tag '$tagName' already exists. Use -Force to recreate it."
        }
        
        Write-Info "Creating tag: $tagName"
        
        # Delete existing tag if forcing
        if ($tagExists -and $Force) {
            Invoke-Git @("tag", "-d", $tagName)
            if ($Push) {
                Invoke-Git @("push", "origin", ":refs/tags/$tagName") -ThrowOnError:$false
            }
        }
        
        # Create new tag
        $previousTag = git tag --sort=-creatordate --list "v*" 2>$null | Where-Object { $_ -ne $tagName } | Select-Object -First 1
        $releaseNotes = New-ReleaseNotes -TagName $tagName -PreviousTag $previousTag
        
        $tempFile = New-TemporaryFile
        try {
            Set-Content -Path $tempFile -Value "$tagName`n`n$releaseNotes" -Encoding UTF8
            Invoke-Git @("tag", "-a", $tagName, "-F", $tempFile.FullName)
        } finally {
            Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
        }
        
        if ($Push) {
            Invoke-Git @("push", "origin", $tagName)
        }
        
        Write-Success "Tag '$tagName' created successfully!"
        if ($Push) {
            Write-Info "Release packages will be automatically built and published to NuGet."
        }
    }
}

Write-Success "Operation completed successfully!"