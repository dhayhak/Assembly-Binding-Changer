using System;
using System.Configuration;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();

                XmlDocument configDoc = new XmlDocument();
                configDoc.Load(machineConfig.FilePath);


                XmlNamespaceManager nsmgr = new XmlNamespaceManager(configDoc.NameTable);
                nsmgr.AddNamespace("bind", "urn:schemas-microsoft-com:asm.v1");

                var assemblyName = "Company.MyAssemblyName";
                var newVersionNumber = "5.1.0.2";

                var g1Node = configDoc.SelectNodes("//bind:assemblyIdentity[@name='" + assemblyName + "']", nsmgr);
                if (g1Node?[0]?.NextSibling?.Attributes != null)
                {
                    var oldVersion = g1Node[0].NextSibling.Attributes["oldVersion"];
                    var newVersion = g1Node[0].NextSibling.Attributes["newVersion"];

                    var fromVersion = oldVersion.InnerText.Split('-')[0];
                    oldVersion.InnerText = fromVersion + "-" + newVersionNumber;
                    newVersion.InnerText = newVersionNumber;
                }
                else
                {
                    // Add new one
                    var publicKeyToken = "7783b5f11ba5d2a9";
                    var container = configDoc.SelectSingleNode("//bind:assemblyBinding", nsmgr);

                    var dependentAssembly = configDoc.CreateElement("dependentAssembly", "urn:schemas-microsoft-com:asm.v1");

                    var assemblyIdentity = configDoc.CreateNode(XmlNodeType.Element, "assemblyIdentity", "urn:schemas-microsoft-com:asm.v1");
                    var bindingRedirect = configDoc.CreateNode(XmlNodeType.Element, "bindingRedirect", "urn:schemas-microsoft-com:asm.v1");

                    var nameAttr = configDoc.CreateAttribute("name");
                    var tokenAttr = configDoc.CreateAttribute("publicKeyToken");
                    var cultureAttr = configDoc.CreateAttribute("culture");


                    var oldVersionAttr = configDoc.CreateAttribute("oldVersion");
                    var newVersionAttr = configDoc.CreateAttribute("newVersion");

                    cultureAttr.Value = "neutral";
                    nameAttr.Value = assemblyName;
                    tokenAttr.Value = publicKeyToken;
                    oldVersionAttr.Value = "1.0.0.0" + "-" + newVersionNumber;
                    newVersionAttr.Value = newVersionNumber;

                    assemblyIdentity.Attributes.Append(nameAttr);
                    assemblyIdentity.Attributes.Append(tokenAttr);
                    assemblyIdentity.Attributes.Append(cultureAttr);


                    bindingRedirect.Attributes.Append(oldVersionAttr);
                    bindingRedirect.Attributes.Append(newVersionAttr);

                    dependentAssembly.AppendChild(assemblyIdentity);
                    dependentAssembly.AppendChild(bindingRedirect);
                    container?.AppendChild(dependentAssembly);
                }

                configDoc.Save(machineConfig.FilePath.Replace(".config", "_backup.config"));
            }
            catch (Exception ex)
            {
                // Log?

                throw;
            }
        }
    }
}