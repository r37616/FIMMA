1. Rebuild the code using the appropriate Microsoft.MetadirectoryServicesEx assemblies for your FIM environment
2. Make sure the project dll gets placed into the FIM Extensions Folder
3. Run Install.bat - installs assemblies to gac, creates C:\Installs (cant be changed) folder for dlls that must be in a known location for run time complication of custom code
4. Create your MA, using the code you want run as the flow name 
5. Reference the Insight.FIM.CodelessSync.dll in the "Configure Extensions" tab of your MA
