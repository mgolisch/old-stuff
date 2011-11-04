using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Text;

namespace MGO.AutoImage.Client
{
    class Imager
    {
        private ImageConfig m_imgcfg;
        private Config m_appconfig;
        private AIService m_serviceproxy;



        public Imager()
        {
            this.m_appconfig = new Config();
            this.m_serviceproxy = new AIService();
            this.m_serviceproxy.Url = this.m_appconfig.WebServiceUrl;
        }

        public string GetActiveMac() 
        {
            PhysicalAddress address;
                        
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                NetworkInterface connected_nic = nics[0];
                foreach (NetworkInterface iface in nics)
                {
                    if (iface.NetworkInterfaceType != NetworkInterfaceType.Loopback) {
                        if (iface.GetIPProperties().UnicastAddresses.Count > 0)
                            connected_nic = iface;
                    }


                }

                address = connected_nic.GetPhysicalAddress();
                Console.WriteLine(address.ToString());    
            return address.ToString();
            
            
        }

        public List<string> GetAllMacs() 
        {

            List<string> macs = new List<string>();

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface iface in nics)
            {

                macs.Add(iface.GetPhysicalAddress().ToString());

            }

            return macs;
        }

        private bool MapDrive(string drive,string share,string user,string pw) 
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "net.exe";
            psi.Arguments = @"use "+drive+ " " + share+ " " +pw+ " /user:" + user;
            Console.WriteLine(psi.Arguments);
            Process proc = Process.Start(psi);
            proc.WaitForExit();
            if (proc.ExitCode != 0)
                return false;
            else
                return true;
        }

        public bool DoImage() 
        {
            this.m_imgcfg = this.m_serviceproxy.GetConfig(GetActiveMac());
            bool is_maped = MapDrive(m_imgcfg.MapDrive, m_imgcfg.MapShare, m_imgcfg.MapUser, m_imgcfg.MapPassword);
            //needs loging
            if (!is_maped)
            {
                m_serviceproxy.ClientLog("Failed to map share..rebooting", m_imgcfg.Machinename);
                Environment.Exit(1);
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Environment.ExpandEnvironmentVariables(@"%systemdrive%\Programs\trueimageEs9\trueimagecmd.exe");
            psi.Arguments = string.Format(@"/create /progress:on /harddisk:{0} /filename:{2}\{1}.tib", m_imgcfg.Disks, m_imgcfg.Machinename, m_imgcfg.MapDrive);
            Console.WriteLine(psi.Arguments);
            m_serviceproxy.ClientLog(string.Format("starting image with command: {0} {1}", psi.FileName, psi.Arguments), m_imgcfg.Machinename);
            Process proc = Process.Start(psi);
            proc.WaitForExit();
            return true;

        }

        private void Notify(NotifyData data) 
        {
            SmtpClient smtp = new System.Net.Mail.SmtpClient("10.0.0.5");
            MailMessage message = new MailMessage();
            message.From = new MailAddress("trueimage@bartpe","Autoimager v0.1");
            message.To.Add(new MailAddress("mgolisch@insight-health.de"));
            message.Body = "Some Text!";
            smtp.Send(message);

        }

        private void DisplayDisks() 
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = Environment.ExpandEnvironmentVariables(@"%systemdrive%\Programs\trueimageEs9\trueimagecmd.exe");
            psi.Arguments = "/list";
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            Process proc = Process.Start(psi);
            proc.WaitForExit();
            while (proc.StandardOutput.Peek() != -1)
                Console.WriteLine(proc.StandardOutput.ReadLine());

        }

        public void CreateConfig() 
        {
            List<string> macs = GetAllMacs();
            Console.WriteLine("Bitte Namen eingeben:");
            string machinename = Console.ReadLine();
            DisplayDisks();
            Console.WriteLine("Bitte zu sichernde Disks durch komma getrent angeben:");
            string disks = Console.ReadLine();
            //call webservice here
            m_serviceproxy.CreateConfig(macs.ToArray(), machinename, disks);
        
        }    
        
    }
}
