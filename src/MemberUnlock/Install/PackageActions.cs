using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using umbraco.interfaces;
using umbraco.IO;

namespace MemberUnlock.Install
{
    /// <summary>
    /// Implement the MemberUnlock package action
    /// </summary>
    public class PackageActions : IPackageAction
    {
        private const string APP_KEY = "memberLockedOutInMinutes";

        public string Alias()
        {
            return "MemberUnlock";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            ExecuteWebConfig();

            return true;
        }

        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"MemberUnlock\"/>";
            return umbraco.cms.businesslogic.packager.standardPackageActions.helper.parseStringToXmlNode(sample);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
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

        #endregion
    }
}