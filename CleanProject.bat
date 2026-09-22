@echo off
setlocal
cd /d "%~dp0"

echo Cleaning Visual Studio build folders...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

echo.
echo Ensuring wwwroot exists...
if not exist wwwroot mkdir wwwroot

echo.
echo Done.
echo Open InventorySystem.slnx in Visual Studio and choose Build ^> Rebuild Solution.
pause
