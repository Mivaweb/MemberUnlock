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
        private const string APP_KEY = "memberLockedOutInMinutes";

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
                var configFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                var settings = configFile.AppSettings.Settings;

                if (settings[APP_KEY] == null)
                {
                    settings.Add(APP_KEY, "10");
                    configFile.Save();
                }
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

            var xnode = xdoc.SelectSingleNode("//scheduledTasks");

            if(xnode != null)
            {
                var xmlTask = new StringBuilder();
                xmlTask.AppendLine("<task log=\"true\" alias=\"memberUnlock\" interval=\"60\" url=\"/umbraco/memberunlock/memberunlockapi/dounlock\" />");

                // Create xml document of the StringBuilder
                XmlDocument xmlNodeToAdd = new XmlDocument();
                xmlNodeToAdd.LoadXml(xmlTask.ToString());

                var nodeToAdd = xmlNodeToAdd.SelectSingleNode("*");

                xnode.AppendChild(xnode.OwnerDocument.ImportNode(nodeToAdd, true));

                saveFile = true;
            }

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
                var configFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                var settings = configFile.AppSettings.Settings;

                if (settings[APP_KEY] != null)
                {
                    settings.Remove(APP_KEY);
                    configFile.Save();
                }
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

            var xnode = xdoc.SelectSingleNode("//task [@alias='" + Alias() + "']");

            if (xnode != null)
            {
                xdoc.SelectSingleNode("//scheduledTasks").RemoveChild(xnode);
                xdoc.Save(configFile);
            }
        }

        #endregion
    }
}