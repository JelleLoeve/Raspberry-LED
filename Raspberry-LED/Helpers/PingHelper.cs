using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Raspberry_LED.Models;

namespace Raspberry_LED.Helpers
{
    public class PingHelper
    {
        private static PingResultDBContext db = new PingResultDBContext();
        public static string Ping(string ip)
        {
            var datum = DateTime.Now.ToString("dd-MM-yyyy");
            var tijd = DateTime.Now.ToString("HH:mm:ss");
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
                db.PingResults.Add(new PingResult
                {
                    Date = datum,
                    Time = tijd,
                    Ping = "Ping failed"
                });
                db.SaveChanges();
            return "";
            }
            else
            {
                var pingResults = strA[10];
                db.PingResults.Add(new PingResult
                {
                    Date = datum,
                    Time = tijd,
                    Ping = pingResults
                });
                db.SaveChanges();
                return pingResults;
            }
        }
    }
}