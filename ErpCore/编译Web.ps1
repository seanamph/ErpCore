# ErpCore Web 前端编译脚本
# 用途: 编译 Vue 前端项目

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore Web 前端编译脚本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 查找 Node.js
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
        Write-Host "✓ 找到 Node.js: $path" -ForegroundColor Green
        break
    }
}

if (-not $nodePath) {
    Write-Host "✗ 未找到 Node.js" -ForegroundColor Red
    Write-Host ""
    Write-Host "请执行以下操作之一:" -ForegroundColor Yellow
    Write-Host "1. 重新启动 PowerShell 或命令提示符" -ForegroundColor White
    Write-Host "2. 确认 Node.js 已正确安装" -ForegroundColor White
    Write-Host "3. 手动将 Node.js 添加到 PATH 环境变量" -ForegroundColor White
    Write-Host ""
    Write-Host "或者，如果您知道 Node.js 的安装路径，请手动执行:" -ForegroundColor Yellow
    Write-Host "  cd ErpCore\src\ErpCore.Web" -ForegroundColor White
    Write-Host "  npm install" -ForegroundColor White
    Write-Host "  npm run build" -ForegroundColor White
    exit 1
}

# 设置 Node.js 路径到环境变量
$nodeDir = Split-Path $nodePath -Parent
$env:Path = "$nodeDir;$env:Path"

# 检查 Node.js 和 npm
Write-Host "检查 Node.js 和 npm..." -ForegroundColor Yellow
try {
    $nodeVersion = & $nodePath --version
    Write-Host "✓ Node.js 版本: $nodeVersion" -ForegroundColor Green
    
    $npmPath = Join-Path $nodeDir "npm.cmd"
    if (Test-Path $npmPath) {
        $npmVersion = & $npmPath --version
        Write-Host "✓ npm 版本: $npmVersion" -ForegroundColor Green
    } else {
        Write-Host "✗ 未找到 npm" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ 无法执行 Node.js" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 进入 Web 项目目录
$webProjectPath = "ErpCore\src\ErpCore.Web"
if (-not (Test-Path $webProjectPath)) {
    Write-Host "✗ Web 项目目录不存在: $webProjectPath" -ForegroundColor Red
    exit 1
}

Write-Host "进入 Web 项目目录: $webProjectPath" -ForegroundColor Yellow
Set-Location $webProjectPath

Write-Host ""

# 检查 node_modules
if (-not (Test-Path "node_modules")) {
    Write-Host "安装依赖套件..." -ForegroundColor Yellow
    Write-Host "这可能需要几分钟时间..." -ForegroundColor Gray
    Write-Host ""
    
    & $npmPath install
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ 依赖安装失败!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "✓ 依赖安装完成" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "✓ node_modules 已存在，跳过安装" -ForegroundColor Green
    Write-Host ""
}

# 编译项目
Write-Host "开始编译 Web 前端项目..." -ForegroundColor Yellow
Write-Host "这可能需要几分钟时间..." -ForegroundColor Gray
Write-Host ""

& $npmPath run build

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "✗ 编译失败!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "编译成功完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "编译后的文件位于: ErpCore\src\ErpCore.Web\dist\" -ForegroundColor Cyan
Write-Host ""
