@echo off
set service-name=%1

sc.exe query %service-name% > nul
if errorlevel 1060 goto service-not-found

echo Stopping service '%service-name%'
sc.exe stop %service-name%

echo Marking service '%service-name%' for deletion
sc.exe delete %service-name%

echo Service '%service-name%' has been marked for deletion
goto end

:service-not-found
echo Service '%service-name%' not found

:end
