@echo off

"EventLogInstallApp.exe" Insight.FIM.Workflow

gacutil /i "..\Assemblies\ExpressionEvaluator.dll"
gacutil /i "..\Assemblies\log4net.dll"
gacutil /i "..\Assemblies\Ensynch.dll"
gacutil /i "..\Assemblies\Ensynch.FIM.dll"
gacutil /i "..\Assemblies\Ensynch.FIM.Workflow.dll"
gacutil /i "..\Assemblies\wwScripting.dll"
gacutil /i "..\Assemblies\RemoteLoader.dll"
gacutil /i "..\Insight.FIM.Workflow\bin\debug\Insight.FIM.Workflow.dll"
gacutil /i "..\Insight.FIM.Controls\bin\Insight.FIM.Controls.dll"

mkdir "C:\Installs"
xcopy "..\Assemblies\wwScripting.dll" /Y "C:\Installs"
xcopy "..\Assemblies\RemoteLoader.dll" /Y "C:\Installs"

xcopy "..\Insight.FIM.Controls\CustomCodeControl.ascx" /Y "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\12\TEMPLATE\CONTROLTEMPLATES"

net stop FIMService
net start FIMService

iisreset

pause
