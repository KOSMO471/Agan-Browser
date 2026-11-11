@echo off
chcp 65001 >nul 2>nul
cd /d "%~dp0"
setlocal enabledelayedexpansion

net session >nul 2>&1
if errorlevel 1 (
    echo [HATA] Bu script yonetici izni gerektirir. Sag tik - Yonetici olarak calistir.
    pause
    exit /b 1
)

set "ESC="
for /f "usebackq delims=" %%a in (`powershell -NoProfile -Command "[char]27" 2^>nul`) do set "ESC=%%a"

set "GREEN=%ESC%[92m"
set "RED=%ESC%[91m"
set "YELLOW=%ESC%[93m"
set "CYAN=%ESC%[96m"
set "GRAY=%ESC%[90m"
set "RESET=%ESC%[0m"

set "MODEL=qwen2.5-coder:7b"
set "FAIL=0"
set "SKIP=0"

cls
echo.
echo %CYAN%============================================%RESET%
echo %CYAN%  AganBrowser - Gereksinim Kurulumu%RESET%
echo %CYAN%============================================%RESET%
echo.

echo %GRAY%[1/6]%RESET% winget kontrol ediliyor...
where winget >nul 2>nul
if errorlevel 1 (
    echo   %RED%[HATA]%RESET% winget bulunamadi.
    echo   Windows 10/11 App Installer'i Microsoft Store'dan kurun.
    pause
    exit /b 1
)
echo   %GREEN%[OK]%RESET% winget hazir.
echo.

echo %GRAY%[2/6]%RESET% .NET 8 SDK kontrol ediliyor...
dotnet --list-sdks 2>nul | findstr /C:"8.0" >nul
if errorlevel 1 (
    echo   %YELLOW%[EKSIK]%RESET% .NET 8 SDK kuruluyor...
    winget install --id Microsoft.DotNet.SDK.8 -e --silent --accept-source-agreements --accept-package-agreements
    if errorlevel 1 (
        echo   %RED%[HATA]%RESET% .NET 8 SDK kurulamadi.
        set /a FAIL+=1
    ) else (
        echo   %GREEN%[OK]%RESET% .NET 8 SDK kuruldu.
    )
) else (
    echo   %GREEN%[OK]%RESET% .NET 8 SDK zaten kurulu.
    set /a SKIP+=1
)
echo.

echo %GRAY%[3/6]%RESET% WebView2 Runtime kontrol ediliyor...
reg query "HKLM\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}" >nul 2>nul
if errorlevel 1 (
    echo   %YELLOW%[EKSIK]%RESET% WebView2 Runtime kuruluyor...
    winget install --id Microsoft.EdgeWebView2Runtime -e --silent --accept-source-agreements --accept-package-agreements
    if errorlevel 1 (
        echo   %RED%[HATA]%RESET% WebView2 Runtime kurulamadi.
        set /a FAIL+=1
    ) else (
        echo   %GREEN%[OK]%RESET% WebView2 Runtime kuruldu.
    )
) else (
    echo   %GREEN%[OK]%RESET% WebView2 Runtime zaten kurulu.
    set /a SKIP+=1
)
echo.

echo %GRAY%[4/6]%RESET% Ollama kontrol ediliyor...
where ollama >nul 2>nul
if errorlevel 1 (
    echo   %YELLOW%[EKSIK]%RESET% Ollama kuruluyor...
    winget install --id Ollama.Ollama -e --silent --accept-source-agreements --accept-package-agreements
    if errorlevel 1 (
        echo   %RED%[HATA]%RESET% Ollama kurulamadi.
        set /a FAIL+=1
    ) else (
        echo   %GREEN%[OK]%RESET% Ollama kuruldu.
        echo   %GRAY%Not: Yeni PATH icin terminali yeniden acmaniz gerekebilir.%RESET%
    )
) else (
    echo   %GREEN%[OK]%RESET% Ollama zaten kurulu.
    set /a SKIP+=1
)
echo.

echo %GRAY%[5/6]%RESET% Ollama servisi ve model kontrol ediliyor...
where ollama >nul 2>nul
if errorlevel 1 (
    echo   %YELLOW%[ATLANDI]%RESET% Ollama PATH'te yok. Terminali yeniden acip tekrar calistirin.
    set /a FAIL+=1
    goto :git_check
)

curl -s --max-time 3 http://localhost:11434/api/tags >nul 2>nul
if errorlevel 1 (
    echo   %YELLOW%[BILGI]%RESET% Ollama servisi kapali, baslatiliyor...
    start "" /B ollama serve
    timeout /t 5 /nobreak >nul
    curl -s --max-time 3 http://localhost:11434/api/tags >nul 2>nul
    if errorlevel 1 (
        echo   %YELLOW%[UYARI]%RESET% Ollama servisi baslatilamadi. Manuel: ollama serve
        set /a FAIL+=1
        goto :git_check
    )
)

echo   %GREEN%[OK]%RESET% Ollama servisi calisiyor.

ollama list 2>nul | findstr /C:"%MODEL%" >nul
if errorlevel 1 (
    echo   %YELLOW%[EKSIK]%RESET% %MODEL% indiriliyor... ^(yaklasik 4.5 GB^)
    ollama pull %MODEL%
    if errorlevel 1 (
        echo   %RED%[HATA]%RESET% Model indirilemedi.
        set /a FAIL+=1
    ) else (
        echo   %GREEN%[OK]%RESET% %MODEL% indirildi.
    )
) else (
    echo   %GREEN%[OK]%RESET% %MODEL% zaten mevcut.
    set /a SKIP+=1
)
echo.

:git_check
echo %GRAY%[6/6]%RESET% Git kontrol ediliyor ^(istege bagli^)...
where git >nul 2>nul
if errorlevel 1 (
    echo   %YELLOW%[EKSIK]%RESET% Git kuruluyor...
    winget install --id Git.Git -e --silent --accept-source-agreements --accept-package-agreements
    if errorlevel 1 (
        echo   %YELLOW%[UYARI]%RESET% Git kurulamadi ^(zorunlu degil^).
    ) else (
        echo   %GREEN%[OK]%RESET% Git kuruldu.
    )
) else (
    echo   %GREEN%[OK]%RESET% Git zaten kurulu.
    set /a SKIP+=1
)
echo.

echo %CYAN%============================================%RESET%
if !FAIL! == 0 (
    echo %GREEN%  Tum gereksinimler hazir.%RESET%
    echo   Atlanan ^(zaten kurulu^): !SKIP!
    echo.
    echo   Sonraki adim: %WHITE%build.bat%RESET%
) else (
    echo %YELLOW%  !FAIL! adet gereksinim eksik veya hatali.%RESET%
    echo   Yukaridaki mesajlari kontrol edin.
)
echo %CYAN%============================================%RESET%
echo.
pause
endlocal
exit /b 0