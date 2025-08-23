#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Release management script for Aviendha framework
.DESCRIPTION
    This script helps manage releases for the Aviendha framework, including version bumping and tagging.
.PARAMETER Action
    The action to perform: bump-major, bump-minor, bump-patch, create-tag, or get-version
.PARAMETER Push
    Whether to push the changes to the remote repository
.EXAMPLE
    ./scripts/release.ps1 -Action bump-minor
    ./scripts/release.ps1 -Action create-tag -Push
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("bump-major", "bump-minor", "bump-patch", "create-tag", "get-version")]
    [string]$Action,
    
    [switch]$Push
)

# Ensure we're in the repository root
$repoRoot = git rev-parse --show-toplevel
if ($LASTEXITCODE -ne 0) {
    Write-Error "Not in a git repository"
    exit 1
}

Set-Location $repoRoot

# Install nbgv if not available
if (!(Get-Command nbgv -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Nerdbank.GitVersioning..." -ForegroundColor Yellow
    dotnet tool install -g nbgv
}

switch ($Action) {
    "get-version" {
        $version = nbgv get-version --variable Version
        Write-Host "Current version: $version" -ForegroundColor Green
    }
    
    "bump-major" {
        Write-Host "Bumping major version..." -ForegroundColor Yellow
        nbgv prepare-release
        if ($Push) {
            git push origin main
            git push origin --tags
        }
    }
    
    "bump-minor" {
        Write-Host "Bumping minor version..." -ForegroundColor Yellow
        nbgv prepare-release
        if ($Push) {
            git push origin main
            git push origin --tags
        }
    }
    
    "bump-patch" {
        Write-Host "Bumping patch version..." -ForegroundColor Yellow
        nbgv prepare-release
        if ($Push) {
            git push origin main
            git push origin --tags
        }
    }
    
    "create-tag" {
        $version = nbgv get-version --variable Version
        $tag = "v$version"
        
        Write-Host "Creating tag: $tag" -ForegroundColor Yellow
        git tag $tag
        
        if ($Push) {
            Write-Host "Pushing tag to remote..." -ForegroundColor Yellow
            git push origin $tag
        }
    }
}

Write-Host "Done!" -ForegroundColor Green