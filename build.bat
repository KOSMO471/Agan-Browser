@echo off
cd /d "%~dp0"
setlocal

echo ============================================
echo   AganBrowser - Derleme (Debug)
echo ============================================
echo.

where dotnet >nul 2>nul
if errorlevel 1 (
    echo [HATA] dotnet CLI bulunamadi. Lutfen .NET SDK kurun.
    pause
    exit /b 1
)

echo [0/3] Calisan islemler kapatiliyor...
taskkill /F /IM AganBrowser.exe >nul 2>nul
taskkill /F /IM msedgewebview2.exe >nul 2>nul
timeout /t 1 /nobreak >nul

echo.
echo [1/3] bin ve obj klasorleri siliniyor...
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo Temizlik tamamlandi.

echo.
echo [2/3] Paketler geri yukleniyor (dotnet restore)...
dotnet restore
if errorlevel 1 (
    echo.
    echo [HATA] Paketler geri yuklenemedi. Internet baglantinizi kontrol edin.
    pause
    exit /b 1
)

echo.
echo [3/3] dotnet build (Debug)...
dotnet build -c Debug --no-restore
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
echo Assets klasoru kontrol ediliyor...
if exist "bin\Debug\net8.0-windows\Assets\personality.json" (
    echo [OK] personality.json kopyalandi.
) else (
    echo [UYARI] personality.json bulunamadi!
)
if exist "bin\Debug\net8.0-windows\Assets\quotes.json" (
    echo [OK] quotes.json kopyalandi.
) else (
    echo [UYARI] quotes.json bulunamadi!
)
if exist "bin\Debug\net8.0-windows\Assets\Incognito.html" (
    echo [OK] Incognito.html kopyalandi.
) else (
    echo [UYARI] Incognito.html bulunamadi!
)
echo.
pause
endlocal
