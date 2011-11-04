using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.OracleClient;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class AIService : System.Web.Services.WebService
{
    private Config m_config;
    private string m_constr_temp = "Data Source={0};User Id={1};Password={2};";
    
    public AIService () {

        //Uncomment the following line if using designed components
        //InitializeComponent(); 
        m_config = new Config();
    }


    [WebMethod]
    public void LinuxSheduleAdd(DateTime date, string command) 
    {
        string time = String.Format("{0}:{1} {2}{3}{4}", date.Hour, date.Minute, date.Day, date.Month, date.Year);
        string atcommand = String.Format("echo {0}|at {1}", command, time);
        RunCommand(atcommand, "", null);
    }

    [WebMethod]
    public void LinuxSheduleRemove(int jobid)
    {
        string atcommand = String.Format("atrm {0}", jobid);
        RunCommand(atcommand, "", null);
    }

    [WebMethod]
    public List<ATJob> LinuxSheduleList()
    {

        List<ATJob> jobs = new List<ATJob>();
        string output;
        Runcommand("atq", "", output);
        //michi@105:~$ atq
        //4       Wed Nov 19 20:00:00 2008 a michi

        foreach(string line in output.Split("\n"))
        {
            int bigsep = line.IndexOf("       ");
            int id = line.Substring(0,bigsep);
            line = line.Remove(0, bigsep);
            string date = line.Substring(0, line.LastIndexOf(" ") - 2);
            Console.WriteLine("ID: " + id);
            Console.WriteLine("Date: " + date);
        }

    }

    [WebMethod]
    public void SwitchBootConfImage(string ip, string machinename)
    {
        string output;
        if (m_config.IsDebug)
            ServerLog(string.Format("switching bootconfig to image for client: {0}", machinename));
        RunCommand("pxe-helper", ip + " image", out output);
    }

    [WebMethod]
    public void SwitchBootConfLocal(string ip,string machinename) 
    {   
    string output;
    if (m_config.IsDebug)
        ServerLog(string.Format("switching bootconfig to localboot for client: {0}", machinename));
    RunCommand("pxe-helper", ip+" local", out output);
    }

    public void RunCommand(string command,string args,out string output) 
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = command;
        psi.Arguments = args;
        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        if (m_config.IsDebug)
            ServerLog(string.Format("running command: {0} with arguments: {1}", command, args));
        Process proc = Process.Start(psi);
        proc.WaitForExit();
        if(output != null)
           output = proc.StandardOutput.ReadToEnd();
    }

    [WebMethod]
    public void ClientLog(string message, string client) 
    {
        Log(message, client);
    } 

    public void ServerLog(string message) {

        Log(message, "AIService");
    }

    public void Log(string message, string client) 
    {
        using (StreamWriter sw = new StreamWriter(m_config.LogLocation)) 
        { 
            sw.WriteLine(string.Format("{0}|{1}:{2}",System.DateTime.Now,client,message));
            sw.Flush();
        }
    
    }

    [WebMethod]
    public ImageConfig GetConfig(string mac) {

        Log("Client " + mac + " requested config..", "autoimage-server");
        ImageConfig cfg = new ImageConfig(); 
        OracleConnection con = new OracleConnection(string.Format(m_constr_temp,m_config.OracleTNS,m_config.OracleUser,m_config.OraclePasswd));
        con.Open();
        OracleCommand cmd = con.CreateCommand();
        string sql_getconfig = string.Format("Select name,discs from zuordnung_imagage_backup where mac='{0}'", mac);
        cmd.CommandText = sql_getconfig;
        OracleDataReader reader2 = cmd.ExecuteReader();
        reader2.Read();
        cfg.Machinename = reader2.GetString(0);
        cfg.Disks = reader2.GetString(1);
        //cfg.SendMail = bool.Parse(reader2.GetString(2));
        reader2.Close();
        cfg.MapUser = m_config.MapUser;
        cfg.MapShare = m_config.MapShare;
        cfg.MapPassword = m_config.MapPassword;
        cfg.MapDrive = m_config.MapDrive;

        con.Close();
        
        return cfg;
    }

    [WebMethod]
    public void CreateConfig(string[] macs, string machinename, string disks) 
    {
        OracleConnection con = new OracleConnection(string.Format(m_constr_temp, m_config.OracleTNS, m_config.OracleUser, m_config.OraclePasswd));
        con.Open();
        OracleCommand cmd = con.CreateCommand();
        string sql_insert_config_temp = "insert into zuordnung_imagage_backup(mac,name,discs) values('{0}','{1}','{2}')";
        foreach (string mac in macs) 
        {
            if(!string.IsNullOrEmpty(mac)){

            cmd.CommandText = string.Format(sql_insert_config_temp, mac,machinename, disks);
            cmd.ExecuteNonQuery();
            
            }
        }

        con.Close();
    
    }
}
