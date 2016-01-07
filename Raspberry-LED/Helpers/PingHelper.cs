using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Raspberry_LED.Models;
using System.Net;

namespace Raspberry_LED.Helpers
{
    public class PingHelper
    {
        private static PingResultsDBContext db = new PingResultsDBContext();
        
        public static string Ping(string ip)
        {
            var datum = DateTime.Now.ToString("dd-MM-yyyy");
            var tijd = DateTime.Now.ToString("HH:mm:ss");
            Process proc = new Process();
            proc.StartInfo.FileName = "ping.exe";
            proc.StartInfo.Arguments = ip + "-w 500";
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
                    IP = ip,
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
                    IP = ip,
                    Ping = "Ping successful"
                });
                db.SaveChanges();
                return pingResults;
            }
        }

        public static string Ping(IPAddress IP)
        {
            string ip = IP.ToString();
            var datum = DateTime.Now.ToString("dd-MM-yyyy");
            var tijd = DateTime.Now.ToString("HH:mm:ss");
            Process proc = new Process();
            proc.StartInfo.FileName = "ping.exe";
            proc.StartInfo.Arguments = ip + " -w 500";
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
                    IP = ip,
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
                    IP = ip,
                    Ping = "Ping successful"
                });
                db.SaveChanges();
                return pingResults;
            }
        }
    }
}