using System;
using System.Data.Entity;

namespace Raspberry_LED.Models
{
    public class Pinconfig
    {
        public int ID { get; set; }
        public string ConnectorPinName { get; set; }
        public int PinNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool isSet { get; set; }
        public bool isOn { get; set; }

    }
    public class PinConfigDBContext : DbContext
    {
        public DbSet<Pinconfig> PinConfigs { get; set; }

    }
}