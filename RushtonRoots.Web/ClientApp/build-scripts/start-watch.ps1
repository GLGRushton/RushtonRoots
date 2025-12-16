#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Starts or manages the npm watch build process for Angular development.

.DESCRIPTION
    This script manages an Angular watch build process (npm run watch) in a separate terminal window.
    - If a watch terminal is already running, does nothing
    - If no watch terminal is running, opens a new terminal running `npm run watch`
    - Logs the terminal process ID (PID) to a file for tracking
    - Allows stopping the terminal via -Stop, using the stored PID

.PARAMETER Stop
    If specified, stops the currently running watch terminal process

.EXAMPLE
    ./start-watch.ps1
    Starts the watch process in a new terminal if not already running

.EXAMPLE
    ./start-watch.ps1 -Stop
    Stops the currently running watch terminal
#>

param(
    [switch]$Stop
)

$ErrorActionPreference = "Stop"

# --------------------------------------------------------------------
# Paths
# --------------------------------------------------------------------

# Script directory: .../ClientApp/build-scripts
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# ClientApp directory: parent of build-scripts
$clientAppPath = Split-Path -Parent $scriptDir

# Project root (parent of ClientApp) – here if you ever need it
$projectRoot = Split-Path -Parent $clientAppPath

# PID file (tracks the terminal process)
$pidFile = Join-Path $scriptDir ".watch-pid"

# --------------------------------------------------------------------
# Helpers
# --------------------------------------------------------------------

function Test-ProcessRunning {
    param([int]$ProcessId)

    try {
        $process = Get-Process -Id $ProcessId -ErrorAction SilentlyContinue
        return $null -ne $process
    }
    catch {
        return $false
    }
}

function Stop-WatchProcess {
    if (Test-Path $pidFile) {
        $pidText = Get-Content $pidFile | Select-Object -First 1
        [int]$pid = 0

        if (-not [int]::TryParse($pidText, [ref]$pid)) {
            Write-Warning ("PID file contains invalid data: '{0}'. Deleting PID file." -f $pidText)
            Remove-Item $pidFile -Force
            return
        }

        Write-Host ("Stopping watch terminal (PID: {0})..." -f $pid)

        if (Test-ProcessRunning -ProcessId $pid) {
            try {
                Stop-Process -Id $pid -Force
                Write-Host "Watch terminal stopped successfully."
            }
            catch {
                Write-Warning ("Failed to stop process {0}: {1}" -f $pid, $_)
            }
        }
        else {
            Write-Host ("Process {0} is not running (stale PID file)." -f $pid)
        }

        Remove-Item $pidFile -Force
    }
    else {
        Write-Host "No watch process is currently running (no PID file found)."
    }
}

# --------------------------------------------------------------------
# Handle -Stop
# --------------------------------------------------------------------

if ($Stop) {
    Stop-WatchProcess
    exit 0
}

# --------------------------------------------------------------------
# Check if a watch terminal is already running
# --------------------------------------------------------------------

if (Test-Path $pidFile) {
    $existingPidText = Get-Content $pidFile | Select-Object -First 1
    [int]$existingPid = 0

    if ([int]::TryParse($existingPidText, [ref]$existingPid)) {
        if (Test-ProcessRunning -ProcessId $existingPid) {
            Write-Host ("Watch process is already running (terminal PID: {0})" -f $existingPid)
            Write-Host "To stop it, run: ./start-watch.ps1 -Stop"
            exit 0
        }
        else {
            Write-Host ("Stale PID file found (process {0} not running). Cleaning up..." -f $existingPid)
            Remove-Item $pidFile -Force
        }
    }
    else {
        Write-Warning ("PID file contains invalid data: '{0}'. Cleaning up..." -f $existingPidText)
        Remove-Item $pidFile -Force
    }
}

# --------------------------------------------------------------------
# Verify paths & dependencies
# --------------------------------------------------------------------

if (-not (Test-Path $clientAppPath)) {
    Write-Error "ClientApp directory not found at: $clientAppPath"
    exit 1
}

$packageJson = Join-Path $clientAppPath "package.json"
if (-not (Test-Path $packageJson)) {
    Write-Error "package.json not found at: $packageJson"
    exit 1
}

$nodeModules = Join-Path $clientAppPath "node_modules"
if (-not (Test-Path $nodeModules)) {
    Write-Host "node_modules not found. Installing npm dependencies..."
    Push-Location $clientAppPath
    try {
        if (Test-Path "package-lock.json") {
            npm ci
            if ($LASTEXITCODE -ne 0) {
                throw "npm ci failed with exit code $LASTEXITCODE"
            }
        }
        else {
            npm install
            if ($LASTEXITCODE -ne 0) {
                throw "npm install failed with exit code $LASTEXITCODE"
            }
        }
    }
    catch {
        Pop-Location
        Write-Error "Failed to install npm dependencies: $_"
        exit 1
    }
    finally {
        Pop-Location
    }
}

# Quick check that npm is available
try {
    $null = Get-Command npm -ErrorAction Stop
}
catch {
    Write-Error "npm is not available on PATH in this session. Make sure Node/npm are installed and restart your shell/VS."
    exit 1
}

# --------------------------------------------------------------------
# Resolve terminal executables (PS5-safe)
# --------------------------------------------------------------------

$windowsTerminal = Join-Path $env:LOCALAPPDATA 'Microsoft\Windows Terminal\wt.exe'

$pwshExe = $null
$powershellExe = $null

$cmd = Get-Command pwsh.exe -ErrorAction SilentlyContinue
if ($cmd) {
    $pwshExe = $cmd.Source
}

$cmd = Get-Command powershell.exe -ErrorAction SilentlyContinue
if ($cmd) {
    $powershellExe = $cmd.Source
}

# --------------------------------------------------------------------
# Start new terminal running `npm run watch`
# --------------------------------------------------------------------

Write-Host "Starting Angular watch build in a new terminal..."
Write-Host "  Working directory: $clientAppPath"
Write-Host "  Command: npm run watch"
Write-Host ""

$startedProcess = $null

if (Test-Path $windowsTerminal) {
    # Windows Terminal: wt -d <dir> -- <shell> -NoExit -Command "npm run watch"
    Write-Host "Using Windows Terminal..."
    
    $shell = if ($pwshExe) { $pwshExe } else { $powershellExe }
    if (-not $shell) {
        Write-Error "Neither pwsh.exe nor powershell.exe found. Cannot start terminal."
        exit 1
    }
    
    $wtArgs = @(
        '-d', $clientAppPath,
        '--', $shell,
        '-NoExit',
        '-Command', 'npm run watch'
    )
    
    $startedProcess = Start-Process -FilePath $windowsTerminal -ArgumentList $wtArgs -PassThru
}
elseif ($pwshExe -or $powershellExe) {
    # Fallback: plain PowerShell/pwsh in a new window
    Write-Host "Using PowerShell window (Windows Terminal not found)..."
    
    $shell = if ($pwshExe) { $pwshExe } else { $powershellExe }
    $shellArgs = @(
        '-NoExit',
        '-Command',
        "Set-Location '$clientAppPath'; npm run watch"
    )
    
    $startedProcess = Start-Process -FilePath $shell -ArgumentList $shellArgs -PassThru -WindowStyle Normal
}
else {
    # Last resort: cmd.exe
    Write-Host "Using cmd.exe (PowerShell not found)..."
    
    $cmdArgs = @(
        '/k',
        "cd /d `"$clientAppPath`" && npm run watch"
    )
    
    $startedProcess = Start-Process -FilePath 'cmd.exe' -ArgumentList $cmdArgs -PassThru -WindowStyle Normal
}

# --------------------------------------------------------------------
# Save PID
# --------------------------------------------------------------------

if ($startedProcess) {
    $startedProcess.Id | Out-File -FilePath $pidFile -Encoding ASCII
    Write-Host ("Watch terminal started successfully (PID: {0})" -f $startedProcess.Id)
    Write-Host ("PID saved to: {0}" -f $pidFile)
    Write-Host ""
    Write-Host "To stop the watch process, run:"
    Write-Host "  ./start-watch.ps1 -Stop"
}
else {
    Write-Error "Failed to start watch terminal. No process was created."
    exit 1
}

exit 0
