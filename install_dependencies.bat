@echo off
chcp 65001 >nul
echo ========================================
echo Installing Python Dependencies
echo ========================================
echo.

set PYTHON_PATH=C:\Python314\python.exe

if not exist "%PYTHON_PATH%" (
    echo ERROR: Python not found at %PYTHON_PATH%
    pause
    exit /b 1
)

echo Using Python: %PYTHON_PATH%
"%PYTHON_PATH%" --version
echo.

echo Step 1: Ensuring pip is available...
"%PYTHON_PATH%" -m ensurepip --upgrade --default-pip
if %ERRORLEVEL% NEQ 0 (
    echo WARNING: ensurepip failed, trying alternative method...
    echo You may need to download get-pip.py from https://bootstrap.pypa.io/get-pip.py
)

echo.
echo Step 2: Installing required modules...
echo.

echo Installing pyautogui...
"%PYTHON_PATH%" -m pip install pyautogui
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to install pyautogui
    pause
    exit /b 1
)

echo.
echo Installing Pillow...
"%PYTHON_PATH%" -m pip install Pillow

echo.
echo Installing numpy...
"%PYTHON_PATH%" -m pip install numpy

echo.
echo Installing opencv-python (optional but recommended)...
"%PYTHON_PATH%" -m pip install opencv-python

echo.
echo Installing pywin32 (optional but recommended)...
"%PYTHON_PATH%" -m pip install pywin32

echo.
echo ========================================
echo Installation complete!
echo ========================================
echo.
echo You can now run: run_auto_clicker_fixed.bat
pause
