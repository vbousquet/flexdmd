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
del Build\FlexDMDUI.exe
del Build\FlexDMD.zip
del Build\FlexDMD.dll
del Build\FlexUDMD.dll
copy FlexDMDUI\bin\Release\FlexDMDUI.exe Build\FlexDMDUI.exe
copy FlexDMD\bin\Release\ILMerge\FlexDMD.dll Build\FlexDMD.dll
copy FlexUDMD\bin\Release\FlexUDMD.dll Build\FlexUDMD.dll
powershell.exe -nologo -noprofile -command "Compress-Archive -CompressionLevel Optimal -Path Build\Flex*.* -DestinationPath Build\FlexDMD.zip"