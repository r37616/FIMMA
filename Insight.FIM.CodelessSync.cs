
using System;
using Microsoft.MetadirectoryServices;
using Westwind.wwScripting;
using System.Collections.Generic;

namespace Mms_Metaverse
{
	/// <summary>
	/// Summary description for MVExtensionObject.
	/// </summary>
    public class MVExtensionObject : IMASynchronization
    {
        #region interface implementation

        public DeprovisionAction Deprovision(CSEntry csentry)
        {
            throw new NotImplementedException();
        }

        public bool FilterForDisconnection(CSEntry csentry)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void MapAttributesForExport(string FlowRuleName, MVEntry mventry, CSEntry csentry)
        {
            runCommand(FlowRuleName, mventry, csentry, "csentry");
        }

        public void MapAttributesForImport(string FlowRuleName, CSEntry csentry, MVEntry mventry)
        {
            runCommand(FlowRuleName, mventry, csentry, "mventry");
        }

        public void MapAttributesForJoin(string FlowRuleName, CSEntry csentry, ref ValueCollection values)
        {
            throw new NotImplementedException();
        }

        public bool ResolveJoinSearch(string joinCriteriaName, CSEntry csentry, MVEntry[] rgmventry, out int imventry, ref string MVObjectType)
        {
            throw new NotImplementedException();
        }

        public bool ShouldProjectToMV(CSEntry csentry, out string MVObjectType)
        {
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            //throw new NotImplementedException();
        }
        
        #endregion

        #region private methods

        private void runCommand(string flowRuleCode, MVEntry mventry, CSEntry csentry, string target)
        {
            string source = target == "mventry" ? "csentry" : "mventry";
                             
            wwScripting wwScript = null;

            //example of what we will be getting in: 
            //mventry["attrib1"].Value = csentry["attrib1"].Value.Substring(0, 1);
            //what the code engine will expect:
            //string param0 = (string)parameter[0];
            //return param0.Substring(0,1);

            //we are importing into a mv attribute, extract the attribute from 
            //the flow rule name
            string targetAttrib = flowRuleCode.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            targetAttrib = targetAttrib.ToLower().Replace(target + "[\"", "").Replace("\"].values", "").Replace("\"].value", "");

            //now extract the code to execute
            string code = " " + flowRuleCode.Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            code = code.Replace("  ", " ");
                       
            //convert any other references to the cs or mv attributes to a
            //parameter array we can pass on
            List<string> paramAttribs = new List<string>();
            List<object> paramValues = new List<object>();
            string[] codeBreaks = code.Split(new string[] {source + "[\""}, StringSplitOptions.None);
            
            for (int i = 1; i < codeBreaks.Length; i++)
			{
                string attrib = codeBreaks[i].Substring(0, codeBreaks[i].IndexOf("\"]"));
                if (!paramAttribs.Contains(attrib))
                {
                    //TODO:  is this the best way to handle the isPresent check??
                    paramAttribs.Add(attrib);
                    if (source == "csentry")
                    {
                        paramValues.Add(csentry[attrib].IsPresent ? csentry[attrib].Value : "");
                    }
                    else
                    {
                        paramValues.Add(mventry[attrib].IsPresent ? mventry[attrib].Value : "");
                    }
                }
			}

            code = "return " + code;
            
            //replace attribute references in code with paramaterized references
            int j = 0;
            foreach (string key in paramAttribs)
            {
                AttributeType type;
                bool isMultiValued = false;

                if (source == "csentry")
                {
                    type = csentry[key].DataType;
                    isMultiValued = csentry[key].IsMultivalued;
                }
                else
                {
                    type = mventry[key].DataType;
                    isMultiValued = mventry[key].IsMultivalued;
                }

                switch (type)
                {
                    case AttributeType.Binary:
                        if (isMultiValued)
                        {
                        }
                        else
                        {
                            code = "byte[] param" + j.ToString() + " = (byte[])Parameters[" + j.ToString() + "]; " + code;
                        }
                        break;
                    case AttributeType.Boolean:
                        if (isMultiValued)
                        {
                        }
                        else
                        {
                            code = "bool param" + j.ToString() + " = bool.Parse((string)Parameters[" + j.ToString() + "]); " + code;
                        }
                        break;
                    case AttributeType.Integer:
                        if (isMultiValued)
                        {
                        }
                        else
                        {
                            code = "int param" + j.ToString() + " = int.Parse((string)Parameters[" + j.ToString() + "]); " + code;
                        }
                        break;
                    case AttributeType.Reference:
                        if (isMultiValued)
                        {
                        }
                        else
                        {
                            code = "Guid param" + j.ToString() + " = new Guid((string)Parameters[" + j.ToString() + "]); " + code;
                        }
                        break;
                    case AttributeType.String:
                        if (isMultiValued)
                        {
                        }
                        else
                        {
                            code = "string param" + j.ToString() + " = (string)Parameters[" + j.ToString() + "]; " + code;                                                     
                        }

                        break;
                    default:
                        break;
                }

                code = code.Replace(source + "[\"" + key + "\"].Value", "param" + j.ToString());  

                j++;
            }            

            if (!code.EndsWith(";"))
            {
                code = code + ";";
            }

            try
            {
                wwScript = new wwScripting("CSharp");
                wwScript.lSaveSourceCode = false;

                //TODO: figure out how to add these from the GAC or by relative path
                //It looks like we have to use a file path: http://www.databaseforum.info/25/860284.aspx
                //however GAC does have a file path behind it (i.e. C:\Windows\assembly\GAC_MSIL\wwScripting\1.0.4486.26865__e7bb1946e9e55389\wwScripting.dll)
                wwScript.cSupportAssemblyPath = "C:\\Installs\\";
                Environment.CurrentDirectory = System.IO.Path.GetTempPath();

                // force into AppDomain
                wwScript.CreateAppDomain("Insight.FIM.CodlessSync");

                //use flow rule name as code to run
                var retVal = wwScript.ExecuteCode(code, paramValues.ToArray());

                if (wwScript.bError)
                {
                    throw new Exception("Error from wwScript - " + wwScript.cErrorMsg);
                }

                AttributeType type;
                bool isMultiValued = false;

                if (target == "mventry")
                {
                    type = mventry[targetAttrib].DataType;
                    isMultiValued = mventry[targetAttrib].IsMultivalued;
                }
                else
                {
                    type = csentry[targetAttrib].DataType;
                    isMultiValued = csentry[targetAttrib].IsMultivalued;
                }


                //set value on target attribute
                switch (type)
                {
                    case AttributeType.Binary:
                        if (target == "mventry")
                        {
                            if (isMultiValued)
                            {
                                mventry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                mventry[targetAttrib].BinaryValue = (byte[])retVal;
                            }
                        }
                        else
                        {
                            if (isMultiValued)
                            {
                                csentry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                csentry[targetAttrib].BinaryValue = (byte[])retVal;
                            }
                        }
                        break;
                    case AttributeType.Boolean:
                        if (target == "mventry")
                        {
                            if (isMultiValued)
                            {
                                mventry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                mventry[targetAttrib].BooleanValue = (bool)retVal;
                            }
                        }
                        else
                        {
                            if (isMultiValued)
                            {
                                csentry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                csentry[targetAttrib].BooleanValue = (bool)retVal;
                            }
                        }
                        break;
                    case AttributeType.Integer:
                        if (target == "mventry")
                        {
                            if (isMultiValued)
                            {
                                mventry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                mventry[targetAttrib].IntegerValue = (int)retVal;
                            }
                        }
                        else
                        {
                            if (isMultiValued)
                            {
                                csentry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                csentry[targetAttrib].IntegerValue = (int)retVal;
                            }
                        }
                        break;
                    case AttributeType.Reference:
                        if (target == "mventry")
                        {
                            if (isMultiValued)
                            {
                                mventry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                mventry[targetAttrib].ReferenceValue = (ReferenceValue)retVal;
                            }
                        }
                        else
                        {
                            if (isMultiValued)
                            {
                                csentry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                csentry[targetAttrib].ReferenceValue = (ReferenceValue)retVal;
                            }
                        }
                        break;
                    case AttributeType.String:
                        if (target == "mventry")
                        {
                            if (isMultiValued)
                            {
                                mventry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                mventry[targetAttrib].StringValue = (string)retVal;
                            }
                        }
                        else
                        {
                            if (isMultiValued)
                            {
                                csentry[targetAttrib].Values = (Microsoft.MetadirectoryServices.ValueCollection)retVal;
                            }
                            else
                            {
                                csentry[targetAttrib].StringValue = (string)retVal;
                            }
                        }
                        break;
                    default:
                        break;
                }                

                //log.Debug("Custom code execution complete.  Return value: " + CodeReturnValue);
            }
            catch (Exception ex)
            {
                //log.Error("An exception occured while executing the custom code.  Details: " + ex.ToString());

                if (ex.InnerException != null)
                {
                    //log.Error("InnerException: " + ex.InnerException.ToString());
                }

                throw;
            }
            finally
            {
                if (wwScript != null)
                {
                    wwScript.Dispose();
                }
            }
        }

        #endregion
    }
}
