@echo off
cd %~dp0

SETLOCAL
SET CACHED_NUGET="%LocalAppData%\NuGet\NuGet.exe"

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST .nuget\nuget.exe goto restore
md .nuget
copy %CACHED_NUGET% .nuget\nuget.exe > nul

:restore
.nuget\nuget.exe update -self

IF EXIST packages\psake goto cli
.nuget\NuGet.exe install psake -version 4.5.0 -ExcludeVersion -o packages -nocache

:cli
if '%1'=='--bootstrap' goto exit
if '%1'=='/?' goto help
if '%1'=='--help' goto help
if '%1'=='-h' goto help
if '%1'=='-t' goto docs
if '%1'=='-T' goto docs

@powershell -NoProfile -ExecutionPolicy unrestricted -Command "& .\packages\psake\tools\psake.ps1 .\psakefile.ps1 %*; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"
goto exit

:help
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "& .\packages\psake\tools\psake.ps1 -help"
goto exit

:docs
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "& .\packages\psake\tools\psake.ps1 .\psakefile.ps1 -docs"
goto exit

:exit
exit /B %errorlevel%
