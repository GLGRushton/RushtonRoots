# Copy hashed Angular build files to non-hashed versions for Razor view references
# This allows views to use consistent filenames (main.js, polyfills.js, etc.)
# while still benefiting from cache-busting hashed filenames in production

param(
    [string]$WwwrootPath = "."
)

$ErrorActionPreference = "Stop"

Write-Host "Creating non-hashed copies of Angular files in $WwwrootPath..."

# Find and copy main.js
$mainFile = Get-ChildItem -Path $WwwrootPath -Filter "main.*.js" -Exclude "*.map" | Select-Object -First 1
if ($mainFile) {
    Copy-Item -Path $mainFile.FullName -Destination (Join-Path $WwwrootPath "main.js") -Force
    Write-Host "Copied $($mainFile.Name) to main.js"
}

# Find and copy polyfills.js
$polyfillsFile = Get-ChildItem -Path $WwwrootPath -Filter "polyfills.*.js" -Exclude "*.map" | Select-Object -First 1
if ($polyfillsFile) {
    Copy-Item -Path $polyfillsFile.FullName -Destination (Join-Path $WwwrootPath "polyfills.js") -Force
    Write-Host "Copied $($polyfillsFile.Name) to polyfills.js"
}

# Find and copy runtime.js
$runtimeFile = Get-ChildItem -Path $WwwrootPath -Filter "runtime.*.js" -Exclude "*.map" | Select-Object -First 1
if ($runtimeFile) {
    Copy-Item -Path $runtimeFile.FullName -Destination (Join-Path $WwwrootPath "runtime.js") -Force
    Write-Host "Copied $($runtimeFile.Name) to runtime.js"
}

# Find and copy styles.css
$stylesFile = Get-ChildItem -Path $WwwrootPath -Filter "styles.*.css" -Exclude "*.map" | Select-Object -First 1
if ($stylesFile) {
    Copy-Item -Path $stylesFile.FullName -Destination (Join-Path $WwwrootPath "styles.css") -Force
    Write-Host "Copied $($stylesFile.Name) to styles.css"
}

Write-Host "Non-hashed file copies created successfully!"
