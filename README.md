# FIMMA

Project Description
 
This is a Forefront Identity Manager Connector Space Extension that will allow a user to define the C# code they want executed for a flow rule at run time. The Synchronization Engine will compile the code and run it in a separate app domain when its initiated (http://www.west-wind.com/presentations/dynamicCode/DynamicCode.htm).

This project became a natural extension to the work I did for the Last FIM Workflow You Will Ever Need http://www.apollojack.com/2012/02/last-fim-workflow-you-will-ever-need.html. This project is still in its infancy and has only been unit tested, but I wanted to get it out to you in case it might help anyone, as well as get improvements from the community.

More details can be found at http://www.apollojack.com/2013/02/the-last-fim-management-agent-rules.html.

Installation
1. Rebuild the code using the appropriate Microsoft.MetadirectoryServicesEx assemblies for your FIM environment
2. Make sure the project dll gets placed into the FIM Extensions Folder
3. Run Install.bat - installs assemblies to gac, creates C:\Installs (cant be changed) folder for dlls that must be 
   in a known location for run time complication of custom code
4. Create your MA, using the code you want run as the flow name
5. Reference the Insight.FIM.CodelessSync.dll in the "Configure Extensions" tab of your MA
