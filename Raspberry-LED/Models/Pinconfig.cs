using System;
using System.Data.Entity;

namespace Raspberry_LED.Models
{
    public class Pinconfig
    {
        public int pinnumber { get; set; }

        public string color { get; set; }

        public bool isSet { get; set; }

        public bool isOn { get; set; }

    }
    public class PinConfigDBContext : DbContext
    {
        public DbSet<Pinconfig> pinconfig { get; set; }

    }
}