@echo off

gacutil /i "..\Assemblies\wwScripting.dll"
gacutil /i "..\Assemblies\RemoteLoader.dll"

mkdir "C:\Installs"
xcopy "..\Assemblies\wwScripting.dll" /Y "C:\Installs"
xcopy "..\Assemblies\RemoteLoader.dll" /Y "C:\Installs"

pause
