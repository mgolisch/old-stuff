// Utils.cs created with MonoDevelop
// User: michi at 20:21 22.10.2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace MGO.AutoImage.Client
{
	
	
	public static class Utils
	{
		
		private static List<string> m_LogFilesystemsUnix = new List<string>();
		private static List<string> m_LogFilesystemsWindows = new List<string>();
		private static string LogLocation = string.Empty;
		
		static Utils(){
			
			m_LogFilesystemsUnix.Add("ext3");
			m_LogFilesystemsUnix.Add("reiserfs");
			m_LogFilesystemsWindows.Add("NTFS");
			LogLocation = GetFatalLogFileLocations()[0];
		}
		
		public static bool IsUnix {
			get{
				if(Environment.OSVersion.Platform == PlatformID.Unix)
					return true;
				else
					return false;
			}
		}
		public static List<string> GetFatalLogFileLocations(){
			List<string> paths = new List<string>();
			DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    if (IsUnix)
                    {
                        if (m_LogFilesystemsUnix.Contains(drive.DriveFormat) && drive.DriveType == DriveType.Fixed)
                            paths.Add(drive.RootDirectory.FullName);
                    }

                    else
                    {
                        if (m_LogFilesystemsWindows.Contains(drive.DriveFormat) && drive.DriveType == DriveType.Fixed)
                            paths.Add(drive.RootDirectory.FullName);
                    }
                }
            }
			return paths;
		}
		
		public static List<string> DumpSystemInfo(){
			List<string> info = new List<string>();
			string template = "{0} - {1}";
			//todo
			return info;
		}
		
		public static int RunShell(string cmd,string args,out string output){
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = cmd;
			psi.Arguments = args;
			psi.RedirectStandardOutput = true;
			psi.UseShellExecute = false;
			Process proc = Process.Start(psi);
			proc.WaitForExit();
			output = proc.StandardOutput.ReadToEnd();
			return proc.ExitCode;
		}
		
		public static bool MountShare(string user,string password,string mountpoint,string share){
			string output = string.Empty;
			string cmd = string.Empty;
			string args_template = string.Empty;
			if(IsUnix){
				cmd = "mount.cifs";
				//Share - Mountpoint - User - Password
				args_template = "{0} {1} -o user={2},password={3}";
			}
			else {
				cmd = "net";
				//Share - Mountpoint - User - Password
				args_template = "/USER:{2} {1} {0} {3}";
			}
			int result = RunShell(cmd,string.Format(args_template,share,mountpoint,user,password),out output);
			if(result == 0)
				return true;
			else
				return false;
		}
		
		public static void LogFatal(string Message){
			string logfile = Path.Combine(LogLocation,"autoimage_fatal.log");
			using (StreamWriter sw = new StreamWriter(logfile))
				sw.WriteLine(Message);
		}
	}
}
