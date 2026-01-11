# ErpCore Web Frontend Build Script
# Purpose: Build Vue frontend project

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore Web Frontend Build Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Find Node.js
$nodePaths = @(
    "C:\Program Files\nodejs\node.exe",
    "$env:ProgramFiles\nodejs\node.exe",
    "$env:LOCALAPPDATA\Programs\nodejs\node.exe",
    "C:\Program Files (x86)\nodejs\node.exe"
)

$nodePath = $null
foreach ($path in $nodePaths) {
    if (Test-Path $path) {
        $nodePath = $path
        Write-Host "Found Node.js: $path" -ForegroundColor Green
        break
    }
}

if (-not $nodePath) {
    Write-Host "Node.js not found in common locations" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please try one of the following:" -ForegroundColor Yellow
    Write-Host "1. Restart PowerShell or Command Prompt" -ForegroundColor White
    Write-Host "2. Manually navigate to ErpCore\src\ErpCore.Web and run:" -ForegroundColor White
    Write-Host "   npm install" -ForegroundColor Cyan
    Write-Host "   npm run build" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

# Set Node.js path to environment
$nodeDir = Split-Path $nodePath -Parent
$env:Path = "$nodeDir;$env:Path"

# Check Node.js and npm
Write-Host "Checking Node.js and npm..." -ForegroundColor Yellow
try {
    $nodeVersion = & $nodePath --version
    Write-Host "Node.js version: $nodeVersion" -ForegroundColor Green
    
    $npmPath = Join-Path $nodeDir "npm.cmd"
    if (Test-Path $npmPath) {
        $npmVersion = & $npmPath --version
        Write-Host "npm version: $npmVersion" -ForegroundColor Green
    } else {
        Write-Host "npm not found" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Cannot execute Node.js" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Navigate to Web project directory
$webProjectPath = "ErpCore\src\ErpCore.Web"
if (-not (Test-Path $webProjectPath)) {
    Write-Host "Web project directory not found: $webProjectPath" -ForegroundColor Red
    exit 1
}

Write-Host "Navigating to Web project directory: $webProjectPath" -ForegroundColor Yellow
Set-Location $webProjectPath

Write-Host ""

# Check node_modules
if (-not (Test-Path "node_modules")) {
    Write-Host "Installing dependencies..." -ForegroundColor Yellow
    Write-Host "This may take a few minutes..." -ForegroundColor Gray
    Write-Host ""
    
    & $npmPath install
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Dependency installation failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "Dependencies installed successfully" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "node_modules exists, skipping installation" -ForegroundColor Green
    Write-Host ""
}

# Build project
Write-Host "Building Web frontend project..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Gray
Write-Host ""

& $npmPath run build

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Built files are located at: ErpCore\src\ErpCore.Web\dist\" -ForegroundColor Cyan
Write-Host ""
