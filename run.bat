@echo off
cd /d "%~dp0"

if exist "bin\Release\net8.0-windows\AganBrowser.exe" (
    start "" "bin\Release\net8.0-windows\AganBrowser.exe"
    exit /b
)

if exist "bin\Debug\net8.0-windows\AganBrowser.exe" (
    start "" "bin\Debug\net8.0-windows\AganBrowser.exe"
    exit /b
)

if exist "publish\AganBrowser.exe" (
    start "" "publish\AganBrowser.exe"
    exit /b
)

echo Derlenmis AganBrowser.exe bulunamadi.
echo Lutfen once build.bat dosyasini calistirin.
pause