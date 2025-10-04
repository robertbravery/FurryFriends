# Define file extensions to include
$extensions = @("*.cs", "*.razor", "*.cshtml", "*.css", "*.js", "*.html")

# Define directories to exclude
$excludeDirs = @(
    "bin",
    "obj",
    "wwwroot\lib",
    "node_modules",
    "wwwroot\css\bootstrap",
    "wwwroot\js\bootstrap",
    "Migrations",
    "Properties",
    "wwwroot\images"
)

# Define files to exclude (patterns)
$excludeFiles = @(
    "*.g.cs",
    "*.Designer.cs",
    "*.AssemblyInfo.cs",
    "*.AssemblyAttributes.cs",
    "*.min.css",
    "*.min.js",
    "bootstrap*.css",
    "bootstrap*.js",
    "jquery*.css",
    "jquery*.js",
    "site.css",
    "*Migrations*.cs",
    "*.generated.cs",
    "**/Debug/**",
    "Program.cs",
    "launchSettings.json",
    "appsettings.json",
    "appsettings.*.json",
    "*.csproj",
    "*.sln",
    "*.config",
    "*.ico",
    "*.png",
    "*.jpg",
    "*.svg"
)

# Initialize counters
$totalFiles = 0
$totalLines = 0
$projectStats = @{}

# Function to check if a path should be excluded
function ShouldExclude($path) {
    foreach ($dir in $excludeDirs) {
        if ($path -like "*\$dir\*") {
            return $true
        }
    }

    $fileName = Split-Path -Leaf $path
    foreach ($pattern in $excludeFiles) {
        if ($fileName -like $pattern) {
            return $true
        }
    }

    return $false
}

# Process each extension
foreach ($ext in $extensions) {
    $files = Get-ChildItem -Path . -Filter $ext -Recurse -File | Where-Object { -not (ShouldExclude $_.FullName) }

    foreach ($file in $files) {
        $content = Get-Content -Path $file.FullName
        $lineCount = ($content | Measure-Object -Line).Lines

        # Get project name from path
        $pathParts = $file.FullName -split '\\'
        $projectIndex = $pathParts.IndexOf("src")
        if ($projectIndex -ge 0 -and $projectIndex + 1 -lt $pathParts.Length) {
            $projectName = $pathParts[$projectIndex + 1]

            if (-not $projectStats.ContainsKey($projectName)) {
                $projectStats[$projectName] = @{
                    Files = 0
                    Lines = 0
                    ExtensionStats = @{}
                }
            }

            $projectStats[$projectName].Files++
            $projectStats[$projectName].Lines += $lineCount

            $extension = $file.Extension.TrimStart('.')
            if (-not $projectStats[$projectName].ExtensionStats.ContainsKey($extension)) {
                $projectStats[$projectName].ExtensionStats[$extension] = @{
                    Files = 0
                    Lines = 0
                }
            }

            $projectStats[$projectName].ExtensionStats[$extension].Files++
            $projectStats[$projectName].ExtensionStats[$extension].Lines += $lineCount
        }

        $totalFiles++
        $totalLines += $lineCount
    }
}

# Calculate file type totals
$fileTypeStats = @{}
foreach ($project in $projectStats.Keys) {
    foreach ($ext in $projectStats[$project].ExtensionStats.Keys) {
        if (-not $fileTypeStats.ContainsKey($ext)) {
            $fileTypeStats[$ext] = @{
                Files = 0
                Lines = 0
            }
        }

        $fileTypeStats[$ext].Files += $projectStats[$project].ExtensionStats[$ext].Files
        $fileTypeStats[$ext].Lines += $projectStats[$project].ExtensionStats[$ext].Lines
    }
}

# Output results
Write-Host "=== FurryFriends Code Statistics ==="
Write-Host "Total Files: $totalFiles"
Write-Host "Total Lines: $totalLines"
Write-Host ""

# Output file type summary
Write-Host "=== File Type Summary ==="
foreach ($ext in $fileTypeStats.Keys | Sort-Object) {
    $stats = $fileTypeStats[$ext]
    Write-Host ".$ext - $($stats.Files) files, $($stats.Lines) lines"
}
Write-Host ""

Write-Host "=== Project Breakdown ==="
foreach ($project in $projectStats.Keys | Sort-Object) {
    $stats = $projectStats[$project]
    Write-Host "$project - $($stats.Files) files, $($stats.Lines) lines"

    Write-Host "  File types:"
    foreach ($ext in $stats.ExtensionStats.Keys | Sort-Object) {
        $extStats = $stats.ExtensionStats[$ext]
        Write-Host "    .$ext - $($extStats.Files) files, $($extStats.Lines) lines"
    }
    Write-Host ""
}
