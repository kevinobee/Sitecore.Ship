@echo Off
cd %~dp0

SETLOCAL

set target=%1
if "%target%" == "" (
   set target=Full
)
set config=%2
if "%config%" == "" (
   set config=Debug
)

git rev-parse HEAD > sha.txt
for /f "delims=" %%a in (sha.txt) do set SHA=%%a

set NuGetExe=nuget
nuget restore

msbuild Build\Build.proj /t:"%target%" /p:Configuration="%config%" /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

git checkout src\Common\CommonVersionInfo.cs