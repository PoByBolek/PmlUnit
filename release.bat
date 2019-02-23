@echo off
setlocal EnableExtensions

for %%X in (msbuild.exe) do (
    set "msbuild.exe=%%~$PATH:X"
)
if defined msbuild.exe (
    if exist "%msbuild.exe" (
        goto msbuild_found
    )
)
for /f "delims=" %%i in ('powershell.exe -ExecutionPolicy bypass "& '%~dp0\find-msbuild.ps1'"') do (
    set "msbuild.exe=%%i"
)
if not defined msbuild.exe (
    goto end
)
if not exist "%msbuild.exe%" (
    goto end
)

:msbuild_found

nuget restore

call :build "PDMS 12.1" "pdms-12.1"
call :build "E3D 1.1" "e3d-1.1"
call :build "E3D 2.1" "e3d-2.1"

xcopy /S /E /F /I pmllib build\pmllib
xcopy /S /E /F /I pmllib-tests build\pmllib-tests

pushd build
"C:\Program Files\7-Zip\7z.exe" a PmlUnit.zip *
move PmlUnit.zip ..
popd

rmdir /S /Q build


goto :eof

:build
set "platform=%~1"
set "bin_dir=build\%~2\bin\"
set "caf_dir=build\%~2\caf\"

"%msbuild.exe%" /p:Configuration=Release "/p:Platform=%platform%" PmlUnit.sln

if not exist %bin_dir% (
    mkdir %bin_dir%
)
if not exist %caf_dir% (
    mkdir %caf_dir%
)

copy "PmlUnit\bin\Release\%platform%\*" "%bin_dir%"
copy "caf\%platform%\*" %caf_dir%
