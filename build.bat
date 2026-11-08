@echo off
cd /d "%~dp0"
setlocal

echo ============================================
echo   AganBrowser - Derleme
echo ============================================
echo.

REM --- dotnet kontrolu ---
where dotnet >nul 2>nul
if errorlevel 1 (
    echo [HATA] dotnet CLI bulunamadi. Lutfen .NET SDK kurun.
    pause
    exit /b 1
)

echo [1/2] dotnet clean...
dotnet clean -c Release
if errorlevel 1 (
    echo [UYARI] Clean basarisiz oldu, devam ediliyor...
)

echo.
echo [2/2] dotnet build (Release)...
dotnet build -c Release
if errorlevel 1 (
    echo.
    echo [HATA] Derleme basarisiz oldu.
    pause
    exit /b 1
)

echo.
echo ============================================
echo   Derleme basariyla tamamlandi.
echo ============================================
echo.
pause
endlocal