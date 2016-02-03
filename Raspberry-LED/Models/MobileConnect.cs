using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;


namespace Raspberry_LED.Models
{
    public class MobileConnect
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool isConnected { get; set; }
    }
    public class MobileConnectDBContext : DbContext
    {
        public DbSet<MobileConnect> MobileConnect { get; set; }

    }
}