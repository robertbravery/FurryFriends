#!/usr/bin/env pwsh
# Create a new feature
[CmdletBinding()]
param(
    [switch]$Json,
    [string]$ShortName,
    [int]$Number = 0,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$FeatureDescription
)
$ErrorActionPreference = 'Stop'

# Load common functions (includes Resolve-Template)
. "$PSScriptRoot/common.ps1"

if (-not $FeatureDescription -or $FeatureDescription.Count -eq 0) {
    Write-Error "Usage: ./create-new-feature.ps1 [-Json] [-ShortName <name>] [-Number <num>] <feature description>"
    exit 1
}
$featureDesc = ($FeatureDescription -join ' ').Trim()

# Resolve repository root. Prefer git information when available, but fall back
# to searching for repository markers so the workflow still functions in repositories that
# were initialised with --no-git.
function Find-RepositoryRoot {
    param(
        [string]$StartDir,
        [string[]]$Markers = @('.git', '.specify')
    )
    $current = Resolve-Path $StartDir
    while ($true) {
        foreach ($marker in $Markers) {
            if (Test-Path (Join-Path $current $marker)) {
                return $current
            }
        }
        $parent = Split-Path $current -Parent
        if ($parent -eq $current) {
            # Reached filesystem root without finding markers
            return $null
        }
        $current = $parent
    }
}
$fallbackRoot = (Find-RepositoryRoot -StartDir $PSScriptRoot)
if (-not $fallbackRoot) {
    Write-Error "Error: Could not determine repository root. Please run this script from within the repository."
    exit 1
}

try {
    $repoRoot = git rev-parse --show-toplevel 2>$null
    if ($LASTEXITCODE -eq 0) {
        $hasGit = $true
    } else {
        throw "Git not available"
    }
} catch {
    $repoRoot = $fallbackRoot
    $hasGit = $false
}

Set-Location $repoRoot

$specsDir = Join-Path $repoRoot 'specs'
New-Item -ItemType Directory -Path $specsDir -Force | Out-Null

# Function to get highest branch number from all branches
function Get-HighestNumberFromBranches {
    $highest = 0
    try {
        $branches = git branch -a 2>$null | ForEach-Object { $_.Trim().TrimStart('* ') } | Where-Object { $_ }
        foreach ($branch in $branches) {
            if ($branch -match '^(\d{3})-') {
                $num = [int]$matches[1]
                if ($num -gt $highest) { $highest = $num }
            }
        }
    } catch {
        Write-Verbose "Could not check Git branches: $_"
    }
    return $highest
}

function Get-NextBranchNumber {
    param(
        [string]$SpecsDir
    )

    # Fetch all remotes to get latest branch info (suppress errors if no remotes)
    try {
        git fetch --all --prune 2>$null | Out-Null
    } catch {
        # Ignore fetch errors
    }

    # Get highest number from ALL branches (not just matching short name)
    $highestBranch = Get-HighestNumberFromBranches

    # Get highest number from ALL specs (not just matching short name)
    $highestSpec = Get-HighestNumberFromSpecs -SpecsDir $SpecsDir

    # Take the maximum of both
    $maxNum = [Math]::Max($highestBranch, $highestSpec)

    # Return next number
    return $maxNum + 1
}

function ConvertTo-CleanBranchName {
    param([string]$Name)
    
    return $Name.ToLower() -replace '[^a-z0-9]', '-' -replace '-{2,}', '-' -replace '^-', '' -replace '-$', ''
}

# Function to generate branch name with stop word filtering and length filtering
function Get-BranchName {
    param([string]$Description)
    
    # Common stop words to filter out
    $stopWords = @(
        'i', 'a', 'an', 'the', 'to', 'for', 'of', 'in', 'on', 'at', 'by', 'with', 'from',
        'is', 'are', 'was', 'were', 'be', 'been', 'being', 'have', 'has', 'had',
        'do', 'does', 'did', 'will', 'would', 'should', 'could', 'can', 'may', 'might', 'must', 'shall',
        'this', 'that', 'these', 'those', 'my', 'your', 'our', 'their',
        'want', 'need', 'add', 'get', 'set'
    )
    
    # Convert to lowercase and extract words (alphanumeric only)
    $cleanName = $Description.ToLower() -replace '[^a-z0-9\s]', ' '
    $words = $cleanName -split '\s+' | Where-Object { $_ }
    
    # Filter words: remove stop words and words shorter than 3 chars (unless they're uppercase acronyms in original)
    $meaningfulWords = @()
    foreach ($word in $words) {
        # Skip stop words
        if ($stopWords -contains $word) { continue }
        
        # Keep words that are length >= 3 OR appear as uppercase in original (likely acronyms)
        if ($word.Length -ge 3) {
            $meaningfulWords += $word
        } elseif ($Description -match "\b$($word.ToUpper())\b") {
            # Keep short words if they appear as uppercase in original (likely acronyms)
            $meaningfulWords += $word
        }
    }
    
    # If we have meaningful words, use first 3-4 of them
    if ($meaningfulWords.Count -gt 0) {
        $maxWords = if ($meaningfulWords.Count -eq 4) { 4 } else { 3 }
        $result = ($meaningfulWords | Select-Object -First $maxWords) -join '-'
        return $result
    } else {
        # Fallback to original logic if no meaningful words found
        $result = ConvertTo-CleanBranchName -Name $Description
        $fallbackWords = ($result -split '-') | Where-Object { $_ } | Select-Object -First 3
        return [string]::Join('-', $fallbackWords)
    }
}

# Generate branch name
if ($ShortName) {
    # Use provided short name, just clean it up
    $branchSuffix = ConvertTo-CleanBranchName -Name $ShortName
} else {
    # Generate from description with smart filtering
    $branchSuffix = Get-BranchName -Description $featureDesc
}

# Determine branch number
if ($Number -eq 0) {
    if ($hasGit) {
        # Check existing branches on remotes
        $Number = Get-NextBranchNumber -SpecsDir $specsDir
    } else {
        # Fall back to local directory check
        $Number = (Get-HighestNumberFromSpecs -SpecsDir $specsDir) + 1
    }
}

$featureNum = ('{0:000}' -f $Number)
$branchName = "$featureNum-$branchSuffix"

# GitHub enforces a 244-byte limit on branch names
# Validate and truncate if necessary (use UTF-8 byte count, not character count)
$maxBranchLength = 244
$utf8Bytes = [System.Text.Encoding]::UTF8.GetByteCount($branchName)
if ($utf8Bytes -gt $maxBranchLength) {
    # Calculate how much we need to trim from suffix
    # Account for: feature number (3) + hyphen (1) = 4 chars
    $maxSuffixLength = $maxBranchLength - 4
    
    # Calculate truncation based on current UTF-8 bytes
    $currentSuffixBytes = [System.Text.Encoding]::UTF8.GetByteCount($branchSuffix)
    $bytesToRemove = $currentSuffixBytes - $maxSuffixLength
    
    # Truncate suffix accounting for UTF-8 multi-byte chars
    if ($bytesToRemove -gt 0 -and $branchSuffix.Length -gt 0) {
        $truncatedSuffix = $branchSuffix
        # Remove characters from end until we're under limit
        while (([System.Text.Encoding]::UTF8.GetByteCount("$featureNum-$truncatedSuffix") -gt $maxBranchLength) -and ($truncatedSuffix.Length -gt 0)) {
            $truncatedSuffix = $truncatedSuffix.Substring(0, $truncatedSuffix.Length - 1)
        }
        # Remove trailing hyphen if truncation created one
        $truncatedSuffix = $truncatedSuffix -replace '-$', ''
        
        $originalBranchName = $branchName
        $branchName = "$featureNum-$truncatedSuffix"
        
        Write-Warning "[specify] Branch name exceeded GitHub's 244-byte limit"
        Write-Warning "[specify] Original: $originalBranchName ($([System.Text.Encoding]::UTF8.GetByteCount($originalBranchName)) bytes)"
        Write-Warning "[specify] Truncated to: $branchName ($([System.Text.Encoding]::UTF8.GetByteCount($branchName)) bytes)"
    }
}

# Check for potential branch name collision and add suffix if needed
if ($hasGit) {
    $existingBranches = git branch -a 2>$null | ForEach-Object { $_.Trim().TrimStart('* ') } | Where-Object { $_ }
    $collisionCount = 0
    $testBranchName = $branchName
    
    while ($existingBranches -contains $testBranchName) {
        $collisionCount++
        $testBranchName = "$branchName-$collisionCount"
    }
    
    if ($collisionCount -gt 0) {
        Write-Warning "[specify] Branch '$branchName' already exists, appending suffix: -$collisionCount"
        $branchName = $testBranchName
    }
}

if ($hasGit) {
    $branchCreated = $false
    try {
        git checkout -q -b $branchName 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $branchCreated = $true
        }
    } catch {
        # Exception during git command
    }

    if (-not $branchCreated) {
        # Check if branch already exists
        $existingBranch = git branch --list $branchName 2>$null
        if ($existingBranch) {
            Write-Error "Error: Branch '$branchName' already exists. Please use a different feature name or specify a different number with -Number."
            exit 1
        } else {
            Write-Error "Error: Failed to create git branch '$branchName'. Please check your git configuration and try again."
            exit 1
        }
    }
} else {
    Write-Warning "[specify] Warning: Git repository not detected; skipped branch creation for $branchName"
}

$featureDir = Join-Path $specsDir $branchName
New-Item -ItemType Directory -Path $featureDir -Force | Out-Null

$template = Resolve-Template -TemplateName 'spec-template' -RepoRoot $repoRoot
$specFile = Join-Path $featureDir 'spec.md'
if ($template -and (Test-Path $template)) { 
    Copy-Item $template $specFile -Force 
} else { 
    New-Item -ItemType File -Path $specFile | Out-Null 
}

# Set the SPECIFY_FEATURE environment variable for the current session
$env:SPECIFY_FEATURE = $branchName

if ($Json) {
    $obj = [PSCustomObject]@{ 
        BRANCH_NAME = $branchName
        SPEC_FILE = $specFile
        FEATURE_NUM = $featureNum
        HAS_GIT = $hasGit
    }
    $obj | ConvertTo-Json -Compress
} else {
    Write-Output "BRANCH_NAME: $branchName"
    Write-Output "SPEC_FILE: $specFile"
    Write-Output "FEATURE_NUM: $featureNum"
    Write-Output "HAS_GIT: $hasGit"
    Write-Output "SPECIFY_FEATURE environment variable set to: $branchName"
}

