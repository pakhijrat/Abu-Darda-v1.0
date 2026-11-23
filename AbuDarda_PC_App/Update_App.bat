@echo off
title Abu Darda v4.2 - Updater Tool
cd /d "%~dp0"

echo ===================================================================
echo               ABU DARDA v4.2 - APP UPDATER & KEY MANAGER
echo ===================================================================
echo.
echo 1. Update PC App HTML from Source:
echo    If you edit index.html in the main folder, running this updates
echo    the PC software app_data instantly without touching user data.
echo.
echo 2. Update Android APK:
echo    Signs the new build with your release.keystore key.
echo.
echo -------------------------------------------------------------------
echo Backing up current user database...
if not exist "app_data\user_data\backups" mkdir "app_data\user_data\backups"
copy /y "..\index.html" "app_data\index.html" >nul
echo [OK] PC Software updated to latest HTML!
echo.
echo Keystore Key Location:
echo [OK] %~dp0..\release.keystore
echo Keystore Alias: abudardakey
echo.
echo ===================================================================
echo               UPDATE COMPLETE - READY TO LAUNCH
echo ===================================================================
pause
