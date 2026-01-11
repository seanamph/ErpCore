@echo off
chcp 65001 >nul
echo ========================================
echo Auto Clicker Program
echo ========================================
echo.

REM 设置 Python 路径
set PYTHON_PATH=C:\Python314\python.exe

echo Checking Python...
if not exist "%PYTHON_PATH%" (
    echo ERROR: Python not found at %PYTHON_PATH%
    echo Trying to find Python in PATH...
    python --version
    if %ERRORLEVEL% NEQ 0 (
        echo ERROR: Python not found
        pause
        exit /b 1
    )
    set PYTHON_PATH=python
) else (
    echo Found Python at: %PYTHON_PATH%
    "%PYTHON_PATH%" --version
)

echo.
echo Checking image files...
if exist a.png (
    echo [OK] a.png
) else if exist a.PNG (
    echo [OK] a.PNG
) else (
    echo [ERROR] a.png or a.PNG not found
)

if exist b.PNG (
    echo [OK] b.PNG
) else if exist b.png (
    echo [OK] b.png
) else (
    echo [ERROR] b.PNG or b.png not found
)

if exist c.PNG (
    echo [OK] c.PNG
) else if exist c.png (
    echo [OK] c.png
) else (
    echo [ERROR] c.PNG or c.png not found
)

echo.
echo Starting program...
echo ========================================
echo.

"%PYTHON_PATH%" -u auto_clicker.py

echo.
echo ========================================
echo Program exited with code: %ERRORLEVEL%
echo ========================================
pause
