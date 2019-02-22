@echo off
setlocal

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
set "build_dir=%~2"
echo MSBuild /p:Configuration=Release "/p:Platform=%platform%" PmlUnit.sln
MSBuild /p:Configuration=Release "/p:Platform=%platform%" PmlUnit.sln

if not exist build (
    mkdir build
)
if not exist build\%build_dir% (
    mkdir build\%build_dir%
)
if not exist build\%build_dir% (
    mkdir build\%build_dir%
)
if not exist build\%build_dir%\bin (
    mkdir build\%build_dir%\bin
)
if not exist build\%build_dir%\caf (
    mkdir build\%build_dir%\caf
)

copy "PmlUnit\bin\Release\%platform%\*" build\%build_dir%\bin\
copy "caf\%platform%\*" build\%build_dir%\caf\
