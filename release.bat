@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Build
"%programfiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" FlexDMD.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir Build
copy FlexDMDUI\bin\Release\FlexDMDUI.exe Build\FlexDMDUI.exe
copy FlexDMD\bin\Release\ILMerge\FlexDMD.dll Build\FlexDMD.dll
copy FlexUDMD\bin\Release\FlexUDMD.dll Build\FlexUDMD.dll
