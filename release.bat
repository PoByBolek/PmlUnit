@echo off
setlocal EnableExtensions

set "BASE_DIR=%~dp0"

set "PmlUnit.sln=%BASE_DIR%\PmlUnit.sln"
set "PmlUnit=%BASE_DIR%\PmlUnit"
set "PmlUnit.Tests=%BASE_DIR%\PmlUnit.Tests"

for /f "delims=" %%i in ('powershell.exe -ExecutionPolicy bypass "& '%BASE_DIR%\get-version.ps1' '%PmlUnit%'"') do (
    set "VERSION=%%i"
)
if not defined VERSION (
    call :write_error "Unable to determine PmlUnit version"
    goto end
)
call :write_info "Building PmlUnit %VERSION%"
set "BUILD_DIR=PmlUnit-%VERSION%"
set "RELEASE_FILE=PmlUnit-%VERSION%.zip"

set "nunit.exe=%BASE_DIR%\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe"

for %%X in (msbuild.exe) do (
    set "msbuild.exe=%%~$PATH:X"
)
if defined msbuild.exe (
    if exist "%msbuild.exe" (
        goto msbuild_found
    )
)
for /f "delims=" %%i in ('powershell.exe -ExecutionPolicy bypass "& '%BASE_DIR%\find-msbuild.ps1'"') do (
    set "msbuild.exe=%%i"
)
if not defined msbuild.exe (
    call :write_error "Couldn't find msbuild.exe on your system"
    goto end
)
if not exist "%msbuild.exe%" (
    call :write_error "'%msbuild.exe%' not found"
    goto end
)

:msbuild_found
call :write_info "Using msbuild from '%msbuild.exe%'"

pushd %~dp0
nuget restore
set _restore_error_level=%errorlevel%
popd
if %_restore_error_level% neq 0 (
    call :write_error "Failed to restore dependencies"
    goto end
)

call :build "PDMS 12.1" "pdms-12.1"
if %errorlevel% neq 0 (
    goto end
)
call :build "E3D 1.1" "e3d-1.1"
if %errorlevel% neq 0 (
    goto end
)
call :build "E3D 2.1" "e3d-2.1"
if %errorlevel% neq 0 (
    goto end
)

call :write_info "Copying PMLLIB files to output directory"

xcopy /S /E /F /I "%BASE_DIR%\pmllib" "%BUILD_DIR%\pmllib"
if %errorlevel% neq 0 (
    call :write_error "Failed to copy PMLLIB to output directory"
    goto end
)
xcopy /S /E /F /I "%BASE_DIR%\pmllib-tests" "%BUILD_DIR%\pmllib-tests"
if %errorlevel% neq 0 (
    call :write_error "Failed to copy PMLLIB tests to output directory"
    goto end
)

call :write_info "Copying README and LICENSE to output directory"
copy "%BASE_DIR%\README.md" "%BUILD_DIR%\README.txt"
if %errorlevel% neq 0 (
    call :write_error "Failed to copy README to output directory"
    goto end
)
copy "%BASE_DIR%\LICENSE" "%BUILD_DIR%\LICENSE.txt"
if %errorlevel% neq 0 (
    call :write_error "Failed to copy LICENSE to output directory"
    goto end
)

call :write_info "Creating zip file"
"C:\Program Files\7-Zip\7z.exe" a "%RELEASE_FILE%" "%BUILD_DIR%"
if %errorlevel% neq 0 (
    call :write_error "Failed to zip build directory"
    goto end
)
rmdir /S /Q "%BUILD_DIR%"

call :write_success "%RELEASE_FILE% created sucessfully"
goto end


:build
set "platform=%~1"
set "bin_dir=%BUILD_DIR%\%~2\bin\"
set "caf_dir=%BUILD_DIR%\%~2\caf\"

call :write_info "Building solution for %platform%"
"%msbuild.exe%" /p:Configuration=Release "/p:Platform=%platform%" "%PmlUnit.sln%"
if %errorlevel% neq 0 (
    call :write_error "Failed to build solution for %platform%"
    exit /B 1
)
call :write_info "Running tests for %platform%"
"%nunit.exe%" --noresult "%PmlUnit.Tests%\bin\Release\%platform%\PmlUnit.Tests.dll"
if %errorlevel% neq 0 (
    call :write_error "Tests for %platform% failed"
    exit /B 2
)

if not exist %bin_dir% (
    mkdir %bin_dir%
    if %errorlevel% neq 0 (
        call :write_error "Unable to create output directory %bin_dir%"
        exit /B 11
    )
)
if not exist %caf_dir% (
    mkdir %caf_dir%
    if %errorlevel% neq 0 (
        call :write_error "Unable to create output directory %caf_dir%"
        exit /B 12
    )
)

copy "%PmlUnit%\bin\Release\%platform%\*" "%bin_dir%"
if %errorlevel% neq 0 (
    call :write_error "Failed to copy PmlUnit.dll to output directory %bin_dir%"
    exit /B 13
)
copy "%BASE_DIR%\caf\%platform%\*" %caf_dir%
if %errorlevel% neq 0 (
    call :write_error "Failed to copy CAF XML files to output directory %caf_dir%"
    exit /B 14
)

goto :eof


:write_info
echo.
echo [97m[INFO] %~1[0m
echo.
goto :eof


:write_error
echo.
echo [91m[ERROR] %~1[0m
goto :eof


:write_success
echo.
echo [92m[SUCCESS] %~1[0m
goto :eof


:end
(((echo.%cmdcmdline%) | find /I "%~0") > nul)
if %errorlevel% equ 0 (
    rem We were called from Windows Explorer. Pause so that people can actually
    rem look at the command output
    pause
)