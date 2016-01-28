using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Raspberry_LED.Models;
using System.Net;
using System.Threading;

namespace Raspberry_LED.Helpers
{
    public class PingHelper
    {
        private static PingResultsDBContext db = new PingResultsDBContext();
        private static MobileConnectDBContext mobiledb = new MobileConnectDBContext();
        
        public static string Ping(string ip)
        {
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
                return "Ping failed";
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

        public static void MobilePing(string ip)
        {
            var datum = DateTime.Now.ToString("dd-MM-yyyy");
            var tijd = DateTime.Now.ToString("HH:mm:ss");
            Process proc = new Process();
            proc.StartInfo.FileName = "ping.exe";
            proc.StartInfo.Arguments = ip + " -n 2 -w 500";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();

            string[] strA = proc.StandardOutput.ReadToEnd().Split('\n');
            string one = strA[2];
            string two = strA[3];
            if ((one.Contains("unreachable") && two.Contains("unreachable")) || (one.Contains("timed") && two.Contains("timed")))
            {
                var sql = mobiledb.MobileConnect.SqlQuery($"SELECT TOP 1 * FROM MobileConnects WHERE IPAddress='{ip}' ORDER BY ID DESC");
                var results = sql.ToList();
                if (results[0].isConnected) //the last item isConnected = true
                {
                    var dbItem = mobiledb.MobileConnect.Add(new MobileConnect
                    {
                        Name = results[0].Name,
                        IPAddress = results[0].IPAddress,
                        isConnected = false,
                        Date = datum,
                        Time = tijd,
                    });
                    mobiledb.SaveChanges();
                }
                else if (!results[0].isConnected) // last result is not connected
                {
                    var dbItem = mobiledb.MobileConnect.Find(results[0].ID);
                    dbItem.Date = datum;
                    dbItem.Time = tijd;
                    mobiledb.Entry(dbItem).State = System.Data.Entity.EntityState.Modified;
                    mobiledb.SaveChanges();
                }
            }
            else
            {
                var sql = mobiledb.MobileConnect.SqlQuery($"SELECT TOP 1 * FROM MobileConnects WHERE IPAddress='{ip}' ORDER BY ID DESC");
                var results = sql.ToList();
                if (!results[0].isConnected) //check the last isConnected item in database 
                {
                    var dbItem = mobiledb.MobileConnect.Add(new MobileConnect
                    {
                        Name = results[0].Name,
                        IPAddress = results[0].IPAddress,
                        isConnected = true,
                        Date = datum,
                        Time = tijd,            
                    });
                    mobiledb.SaveChanges();
                }
                else if(results[0].isConnected) // last result is connected
                {
                    var dbItem = mobiledb.MobileConnect.Find(results[0].ID);
                    dbItem.Date = datum;
                    dbItem.Time = tijd;
                    mobiledb.Entry(dbItem).State = System.Data.Entity.EntityState.Modified;
                    mobiledb.SaveChanges();
                }
            }
        }
    }
}