using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;

/// <summary>
/// Summary description for Config
/// </summary>
public class Config
{
	public Config()
	{
        NameValueCollection cfg = (NameValueCollection) ConfigurationManager.GetSection("appSettings");
        m_tnsname = cfg["TnsName"];
        m_orauser = cfg["OraUser"];
        m_orapw = cfg["OraPass"];
        m_mapdrive = cfg["MapDrive"];
        m_mappwd = cfg["MapPasswd"];
        m_mapshare = cfg["MapShare"];
        m_mapuser = cfg["MapUser"];
        m_logloc = cfg["LogLocation"];
        m_isdebug = bool.Parse(cfg["Debug"]);
	}
    private bool m_isdebug;
    private string m_tnsname;
    private string m_orauser;
    private string m_orapw;
    private string m_mapdrive;
    private string m_mapshare;
    private string m_mapuser;
    private string m_mappwd;
    private string m_logloc;
    public bool IsDebug { get { return m_isdebug; } set { m_isdebug = value; } }
    public string OracleTNS { get { return m_tnsname; } set { m_tnsname = value; } }
    public string OracleUser { get { return m_orauser; } set { m_orauser = value; } }
    public string OraclePasswd { get { return m_orapw; } set { m_orapw = value; } }
    public string MapDrive { get { return m_mapdrive; } set { m_mapdrive = value; } }
    public string MapShare { get { return m_mapshare; } set { m_mapshare = value; } }
    public string MapUser { get { return m_mapuser; } set { m_mapuser = value; } }
    public string MapPassword { get { return m_mappwd; } set { m_mappwd = value; } }
    public string LogLocation { get { return m_logloc; } set { m_logloc = value; } }
}
