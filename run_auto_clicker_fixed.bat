@echo off
chcp 65001 >nul
echo ========================================
echo Auto Clicker Program
echo ========================================
echo.

REM 设置 Python 路径（优先使用完整安装路径）
if exist "C:\Users\win10-1\AppData\Local\Programs\Python\Python314\python.exe" (
    set PYTHON_PATH=C:\Users\win10-1\AppData\Local\Programs\Python\Python314\python.exe
) else if exist "C:\Python314\python.exe" (
    set PYTHON_PATH=C:\Python314\python.exe
) else (
    set PYTHON_PATH=python
)

echo Checking Python...
if not exist "%PYTHON_PATH%" (
    echo ERROR: Python not found at %PYTHON_PATH%
    pause
    exit /b 1
)

echo Found Python at: %PYTHON_PATH%
"%PYTHON_PATH%" --version
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
echo Checking Python modules...
"%PYTHON_PATH%" -c "import pyautogui; print('[OK] pyautogui')" 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] pyautogui not installed
    echo.
    echo Please install required modules:
    echo   "%PYTHON_PATH%" -m pip install pyautogui Pillow numpy opencv-python pywin32
    echo.
    echo Or if pip is not available, you may need to reinstall Python with pip included.
    pause
    exit /b 1
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
