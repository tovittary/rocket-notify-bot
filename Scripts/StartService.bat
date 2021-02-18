@echo off
set service-name=%1
set service-bin-path=%2

sc.exe query %service-name% > nul
if errorlevel 1060 goto create-service

:stop-service
echo Stopping existing service '%service-name%'...
call RemoveService.bat %service-name%

:create-service
echo Creating service '%service-name%'...
sc.exe create %service-name% type= own start= auto binpath= %service-bin-path%

echo Service '%service-name%' has been installed

echo Starting '%service-name%'...
sc.exe start %service-name%
