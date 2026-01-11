@echo off
chcp 65001 >nul
echo ========================================
echo 自动点击程序启动
echo ========================================
echo.

echo 检查 Python...
python --version
if %ERRORLEVEL% NEQ 0 (
    echo 错误: Python 未安装或不在 PATH 中
    pause
    exit /b 1
)

echo.
echo 检查图片文件...
if exist a.png (
    echo [OK] a.png
) else if exist a.PNG (
    echo [OK] a.PNG
) else (
    echo [错误] 找不到 a.png 或 a.PNG
)

if exist b.PNG (
    echo [OK] b.PNG
) else if exist b.png (
    echo [OK] b.png
) else (
    echo [错误] 找不到 b.PNG 或 b.png
)

if exist c.PNG (
    echo [OK] c.PNG
) else if exist c.png (
    echo [OK] c.png
) else (
    echo [错误] 找不到 c.PNG 或 c.png
)

echo.
echo 启动程序...
echo ========================================
echo.

python -u auto_clicker.py

echo.
echo ========================================
echo 程序已退出，错误代码: %ERRORLEVEL%
echo ========================================
pause
