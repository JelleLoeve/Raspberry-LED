using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Raspberry_LED.Helpers
{
    public class PingHelper
    {
        public static string Ping(string ip)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "ping.exe";
            proc.StartInfo.Arguments = ip;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            string[] strA = proc.StandardOutput.ReadToEnd().Split('\n');
            if (strA.Length == 10)
            {
                return "";
            }
            else
            {
                var pingResults = strA[10];
                return pingResults;
            }
        }
    }
}