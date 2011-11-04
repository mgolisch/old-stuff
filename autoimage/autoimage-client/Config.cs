using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MGO.AutoImage.Client
{
    /// <summary>
    /// Description of config.
    /// </summary>
    public class Config
    {
        private string m_webserviceurl;
        private bool m_isdebug;
        private bool m_isunix;
        
        public Config()
        {
            try
            {
                NameValueCollection configsettings = (NameValueCollection)ConfigurationSettings.GetConfig("appSettings");
                m_webserviceurl = configsettings["WebServiceUrl"];
                m_isdebug = bool.Parse(configsettings["Debug"]);
                m_isunix = bool.Parse(configsettings["Unix"]);
            }
            catch (Exception e) { }
        }

        public bool IsDebug { get { return m_isdebug; } set { m_isdebug = value; } }
        public bool IsUnix { get { return m_isunix; } set { m_isunix = value; } }

        public string WebServiceUrl
        {

            get { return m_webserviceurl; }
        }

    }
}
