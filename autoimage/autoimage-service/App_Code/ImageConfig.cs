using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for ImageConfig
/// </summary>

public class ImageConfig
{
    private string m_machinename;
    private bool m_sendmail;
    private string m_disks;
    private string m_mapdrive;
    private string m_mapshare;
    private string m_mapuser;
    private string m_mappasswd;

    public string Machinename { get { return m_machinename; } set { m_machinename = value; } }
    public string Disks { get { return m_disks; } set { m_disks = value; } }
    public bool SendMail { get { return m_sendmail; } set { m_sendmail = value; } }
    public string MapDrive { get { return m_mapdrive; } set { m_mapdrive = value; } }
    public string MapShare { get { return m_mapshare; } set { m_mapshare = value; } }
    public string MapUser { get { return m_mapuser; } set { m_mapuser = value; } }
    public string MapPassword { get { return m_mappasswd; } set { m_mappasswd = value; } }


    public ImageConfig()
    {
    
    }
}
