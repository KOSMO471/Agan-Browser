@echo off
chcp 65001 >nul 2>nul
cd /d "%~dp0"
setlocal enabledelayedexpansion

set "ESC="
for /f "usebackq delims=" %%a in (`powershell -NoProfile -Command "[char]27" 2^>nul`) do set "ESC=%%a"

set "GREEN=%ESC%[92m"
set "RED=%ESC%[91m"
set "YELLOW=%ESC%[93m"
set "CYAN=%ESC%[96m"
set "GRAY=%ESC%[90m"
set "WHITE=%ESC%[97m"
set "RESET=%ESC%[0m"

set "CONFIG=Debug"
if /I "%~1"=="release" set "CONFIG=Release"
if /I "%~1"=="debug" set "CONFIG=Debug"

set "OUTDIR=bin\%CONFIG%\net8.0-windows"
set "START_TIME=%TIME%"

cls
echo.
echo %CYAN%============================================%RESET%
echo %CYAN%  AganBrowser - Derleme (%CONFIG%)%RESET%
echo %CYAN%============================================%RESET%
echo.

echo %GRAY%[Ortam Bilgisi]%RESET%
where dotnet >nul 2>nul
if errorlevel 1 (
    echo   %RED%[HATA]%RESET% dotnet CLI bulunamadi. .NET SDK kurun.
    pause
    exit /b 1
)
for /f "tokens=*" %%v in ('dotnet --version 2^>nul') do set "SDK_VER=%%v"
echo   .NET SDK:     !SDK_VER!
echo   Yapilandirma: %CONFIG%
echo   Baslangic:    %START_TIME%
if not exist "AganBrowser.csproj" (
    echo   %RED%[HATA]%RESET% AganBrowser.csproj bulunamadi.
    pause
    exit /b 1
)
echo   Proje:        AganBrowser.csproj
echo.

echo %GRAY%[1/4]%RESET% Calisan islemler kapatiliyor...
taskkill /F /IM AganBrowser.exe >nul 2>nul
taskkill /F /IM msedgewebview2.exe >nul 2>nul
timeout /t 1 /nobreak >nul
echo   %GREEN%[OK]%RESET% Islemler kapatildi.
echo.

echo %GRAY%[2/4]%RESET% bin ve obj klasorleri siliniyor...
if exist "bin" (
    rmdir /s /q "bin" 2>nul
    if exist "bin" (
        echo   %RED%[HATA]%RESET% bin silinemedi. Dosyalar kilitli olabilir.
        pause
        exit /b 1
    )
    echo   %GREEN%[OK]%RESET% bin silindi.
) else (
    echo   %YELLOW%[SKIP]%RESET% bin zaten yok.
)
if exist "obj" (
    rmdir /s /q "obj" 2>nul
    if exist "obj" (
        echo   %RED%[HATA]%RESET% obj silinemedi.
        pause
        exit /b 1
    )
    echo   %GREEN%[OK]%RESET% obj silindi.
) else (
    echo   %YELLOW%[SKIP]%RESET% obj zaten yok.
)
echo.

echo %GRAY%[3/4]%RESET% Paketler geri yukleniyor (dotnet restore)...
dotnet restore --verbosity minimal
if errorlevel 1 (
    echo   %RED%[HATA]%RESET% Paket geri yukleme basarisiz.
    echo   Internet baglantisini kontrol edin.
    pause
    exit /b 1
)
echo   %GREEN%[OK]%RESET% Paketler hazir.
echo.

echo %GRAY%[4/4]%RESET% dotnet build (%CONFIG%)...
dotnet build -c %CONFIG% --no-restore --verbosity minimal
if errorlevel 1 (
    echo.
    echo %RED%============================================%RESET%
    echo %RED%  DERLEME BASARISIZ%RESET%
    echo %RED%============================================%RESET%
    echo   Yukaridaki hata mesajlarini kontrol edin.
    pause
    exit /b 1
)
echo.

set "END_TIME=%TIME%"

echo %GREEN%============================================%RESET%
echo %GREEN%  Derleme basariyla tamamlandi%RESET%
echo %GREEN%============================================%RESET%
echo.
echo   Yapilandirma: %CONFIG%
echo   Baslangic:    %START_TIME%
echo   Bitis:        %END_TIME%
echo   Cikti:        %OUTDIR%
echo.

if exist "%OUTDIR%\AganBrowser.exe" (
    for %%A in ("%OUTDIR%\AganBrowser.exe") do (
        echo   EXE boyutu:   %%~zA bayt
        echo   EXE tarihi:   %%~tA
    )
    echo   %GREEN%[OK]%RESET% AganBrowser.exe olusturuldu.
) else (
    echo   %RED%[HATA]%RESET% AganBrowser.exe bulunamadi!
)
echo.

echo %CYAN%Assets Kontrolu:%RESET%
set "ASSET_OK=0"
set "ASSET_FAIL=0"
for %%F in (
    "NewTab.html"
    "Agan.html"
    "Onboarding.html"
    "Incognito.html"
    "quotes.json"
    "personality.json"
) do (
    if exist "%OUTDIR%\Assets\%%~F" (
        echo   %GREEN%[OK]%RESET%    %%~F
        set /a ASSET_OK+=1
    ) else (
        echo   %YELLOW%[EKSIK]%RESET% %%~F
        set /a ASSET_FAIL+=1
    )
)
if exist "%OUTDIR%\Assets\Extensions" (
    set "EXT_COUNT=0"
    for /d %%D in ("%OUTDIR%\Assets\Extensions\*") do set /a EXT_COUNT+=1
    echo   %GREEN%[OK]%RESET%    Extensions/ ^(!EXT_COUNT! uzanti^)
) else (
    echo   %YELLOW%[EKSIK]%RESET% Extensions/
)
if exist "%OUTDIR%\Assets\AdblockLists" (
    set "AB_COUNT=0"
    for %%F in ("%OUTDIR%\Assets\AdblockLists\*.txt") do set /a AB_COUNT+=1
    echo   %GREEN%[OK]%RESET%    AdblockLists/ ^(!AB_COUNT! liste^)
) else (
    echo   %YELLOW%[EKSIK]%RESET% AdblockLists/
)
echo.

echo %CYAN%Ozet:%RESET%
if !ASSET_FAIL! == 0 (
    echo   %GREEN%Tum Assets dosyalari yerinde ^(!ASSET_OK!/6^).%RESET%
) else (
    echo   %YELLOW%!ASSET_FAIL! adet Assets dosyasi eksik.%RESET%
    echo   Assets klasorunu kontrol edin.
)
echo.

echo %GRAY%Ipuclari:%RESET%
echo   Uygulamayi baslat:        %WHITE%run.bat%RESET%
echo   Release modunda derle:    %WHITE%build.bat release%RESET%
echo   Sadece derle ^(temizleme^): %WHITE%dotnet build -c %CONFIG%%RESET%
echo.

pause
endlocal
exit /b 0
