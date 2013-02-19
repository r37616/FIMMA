1. Rebuild the Workflow code usng the appropriate Microsoft.ResourceManagement and Microsoft.IdentityManagement.WFExtensionInterfaces assemblies for your FIM environment
2. Ensure MPR is set up to allow creation/modification of AIC objects
3. Create AIC in FIM
	Activity Name: Insight.FIM.Workflow.Activities.CustomCodeActivity
	Assembly Name: Insight.FIM.Workflow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=daa4bcd323c6d191
	Type Name:     Insight.FIM.WebUI.Controls.CustomCodeActivitySettingsPart
4. Run Install.bat - installs assemblies to gac, assumes sharepoint templates folder location, performs restart of services, creates C:\Installs (cant be changed) folder for dlls that must be in a known location for run time complication of custom code
5. Create a Workflow in the FIM Portal using the new Custom Code Activity
6. Create an MPR to apply Workflow
