# Start npm watch script for local development
# This script runs npm install (if needed) and starts npm watch in a separate window
# Only runs for Debug builds, never in CI/QA/Prod environments

param(
    [string]$ProjectDir = (Get-Location).Path
)

$ErrorActionPreference = "SilentlyContinue"

# Define paths
$clientAppDir = Join-Path $ProjectDir "ClientApp"
$pidFile = Join-Path $ProjectDir "npm-watch.pid"
$nodeModulesDir = Join-Path $clientAppDir "node_modules"

# Check if we're in a CI environment
if ($env:CI -eq "true" -or $env:IsCI -eq "true" -or $env:TF_BUILD -eq "true" -or $env:GITHUB_ACTIONS -eq "true") {
    Write-Host "CI environment detected. Skipping npm watch."
    exit 0
}

# Change to ClientApp directory
if (-not (Test-Path $clientAppDir)) {
    Write-Host "ClientApp directory not found at: $clientAppDir"
    exit 0
}

Set-Location $clientAppDir

# Check if npm watch is already running
if (Test-Path $pidFile) {
    $pid = Get-Content $pidFile
    $process = Get-Process -Id $pid -ErrorAction SilentlyContinue
    if ($process -and $process.ProcessName -eq "node") {
        Write-Host "npm watch is already running (PID: $pid)"
        exit 0
    }
    # Clean up stale PID file
    Remove-Item $pidFile -Force
}

# Run npm install if node_modules doesn't exist
if (-not (Test-Path $nodeModulesDir)) {
    Write-Host "Running npm install..."
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "npm install failed"
        exit 1
    }
}

# Start npm watch in a new window
Write-Host "Starting npm watch in a new window..."
$watchProcess = Start-Process -FilePath "cmd.exe" -ArgumentList "/c", "title npm watch - Rushton Roots && npm run watch" -WorkingDirectory $clientAppDir -PassThru -WindowStyle Normal

if ($watchProcess) {
    # Save the PID
    $watchProcess.Id | Out-File -FilePath $pidFile -Encoding ASCII
    Write-Host "npm watch started with PID: $($watchProcess.Id)"
    Write-Host "PID saved to: $pidFile"
} else {
    Write-Host "Failed to start npm watch"
    exit 1
}

exit 0
