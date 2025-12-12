# Stop npm watch script
# This script stops the npm watch process if it's running

param(
    [string]$ProjectDir = (Get-Location).Path
)

$ErrorActionPreference = "SilentlyContinue"

# Define paths
$pidFile = Join-Path $ProjectDir "npm-watch.pid"

# Check if PID file exists
if (-not (Test-Path $pidFile)) {
    Write-Host "No npm watch process found (PID file not found)"
    exit 0
}

# Read the PID
$pid = Get-Content $pidFile

# Try to stop the process
$process = Get-Process -Id $pid -ErrorAction SilentlyContinue
if ($process) {
    Write-Host "Stopping npm watch process (PID: $pid)..."
    Stop-Process -Id $pid -Force
    Write-Host "npm watch stopped"
} else {
    Write-Host "npm watch process not running (PID: $pid)"
}

# Clean up PID file
Remove-Item $pidFile -Force
Write-Host "PID file removed"

exit 0
