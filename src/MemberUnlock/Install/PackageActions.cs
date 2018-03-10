using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using umbraco.interfaces;
using Umbraco.Core.IO;

namespace MemberUnlock.Install
{
    /// <summary>
    /// Implement the MemberUnlock package action
    /// </summary>
    public class PackageActions : IPackageAction
    {
        private const string APP_KEY_GUID = "memberUnlockAppKey";
        private const string APP_KEY_TIMEOUT = "memberLockedOutInMinutes";
        private Guid appKey;

        private static string UmbracoConfig
        {
            get
            {
                return SystemDirectories.Config + "/umbracoSettings.config";
            }
        }

        public string Alias()
        {
            return "memberUnlock";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            appKey = Guid.NewGuid();

            ExecuteUmbracoConfig();
            ExecuteWebConfig();

            return true;
        }

        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"" + Alias() + "\"/>";
            return umbraco.cms.businesslogic.packager.standardPackageActions.helper.parseStringToXmlNode(sample);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            UndoUmbracoConfig();
            UndoWebConfig();

            return true;
        }

        #region Private Methods

        /// <summary>
        /// Add new key to appSettings section in the webConfig file
        /// </summary>
        private void ExecuteWebConfig()
        {
            try
            {
                bool saveFile = false;

                var configFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                var settings = configFile.AppSettings.Settings;

                // Add or update the `memberLockedOutInMinutes` appSettings key
                if (settings[APP_KEY_TIMEOUT] == null)
                {
                    settings.Add(APP_KEY_TIMEOUT, "10");
                    saveFile = true;
                }
                else
                {
                    settings[APP_KEY_TIMEOUT].Value = "10";
                    saveFile = true;
                }

                // Add or update the `memberUnlockAppKey` appSettings key
                if (settings[APP_KEY_GUID] == null)
                {
                    settings.Add(APP_KEY_GUID, appKey.ToString());
                    saveFile = true;
                }
                else
                {
                    settings[APP_KEY_GUID].Value = appKey.ToString();
                    saveFile = true;
                }

                // Save webConfig file
                if (saveFile) configFile.Save();
            }
            catch { }
        }

        /// <summary>
        /// Add new scheduled task
        /// </summary>
        private void ExecuteUmbracoConfig()
        {
            bool saveFile = false;

            // Get the umbracoSettings.config file
            var configFile = IOHelper.MapPath(UmbracoConfig);

            // Load in the xml
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(configFile);

            // Remove the old task
            RemoveOldTask(xdoc);

            // Get scheduledTasks node
            var xnode = xdoc.SelectSingleNode("//scheduledTasks");

            if(xnode != null)
            {
                // Get authority domain
                Uri currentUri = new Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri);
                string leftPart = currentUri.GetLeftPart(UriPartial.Authority);

                var xmlTask = new StringBuilder();
                xmlTask.AppendLine(
                    "<task log=\"true\" alias=\"memberUnlock\" interval=\"60\" url=\"" + 
                    leftPart + 
                    "/umbraco/memberunlock/memberunlockapi/dounlock?appkey=" + 
                    appKey + 
                    "\" />"
                );

                // Create xml document of the StringBuilder
                XmlDocument xmlNodeToAdd = new XmlDocument();
                xmlNodeToAdd.LoadXml(xmlTask.ToString());

                // Get node to add
                var nodeToAdd = xmlNodeToAdd.SelectSingleNode("*");

                // Import the node into the configFile
                xnode.AppendChild(xnode.OwnerDocument.ImportNode(nodeToAdd, true));

                saveFile = true;
            }

            // Save the configFile
            if(saveFile)
            {
                xdoc.Save(configFile);
            }
        }

        /// <summary>
        /// Remove key from appSettings section in the webConfig file
        /// </summary>
        private void UndoWebConfig()
        {
            try
            {
                bool saveFile = false;

                var configFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                var settings = configFile.AppSettings.Settings;

                if (settings[APP_KEY_TIMEOUT] != null)
                {
                    settings.Remove(APP_KEY_TIMEOUT);
                    saveFile = true;
                }

                if(settings[APP_KEY_GUID] != null)
                {
                    settings.Remove(APP_KEY_GUID);
                    saveFile = true;
                }

                // Save the webConfig file
                if (saveFile) configFile.Save();
            }
            catch { }
        }

        /// <summary>
        /// Remove scheduled task
        /// </summary>
        private void UndoUmbracoConfig()
        {
            // Get the umbracoSettings.config file
            var configFile = IOHelper.MapPath(UmbracoConfig);

            // Load in the xml
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(configFile);

            if (RemoveOldTask(xdoc))
            {
                xdoc.Save(configFile);
            }
        }

        /// <summary>
        /// Remove the old task if present
        /// </summary>
        /// <param name="xdoc"></param>
        /// <returns></returns>
        private bool RemoveOldTask(XmlDocument xdoc)
        {
            var xnode = xdoc.SelectSingleNode("//task [@alias='" + Alias() + "']");

            if (xnode != null)
            {
                xdoc.SelectSingleNode("//scheduledTasks").RemoveChild(xnode);
                return true;
            }

            return false;
        }
        #endregion
    }
}