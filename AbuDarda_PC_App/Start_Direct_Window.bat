@echo off
title Abu Darda v4.2 - Desktop Mode
cd /d "%~dp0"

echo ========================================================
echo          ABU DARDA v4.2 - DESKTOP CONSULTANT
echo ========================================================
echo.
echo Launching standalone app window...
echo.

if exist "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" (
    start "" "%ProgramFiles(x86)%\Microsoft\Edge\Application\msedge.exe" --app="file:///%~dp0app_data\index.html" --user-data-dir="%~dp0app_data\user_profile" --window-size=1300,850
    goto end
)

if exist "%ProgramFiles%\Google\Chrome\Application\chrome.exe" (
    start "" "%ProgramFiles%\Google\Chrome\Application\chrome.exe" --app="file:///%~dp0app_data\index.html" --user-data-dir="%~dp0app_data\user_profile" --window-size=1300,850
    goto end
)

start "" "%~dp0app_data\index.html"

:end
exit
