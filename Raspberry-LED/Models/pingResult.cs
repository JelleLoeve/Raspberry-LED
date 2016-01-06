using System.Data.Entity;

namespace Raspberry_LED.Models
{
    public class PingResult
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string IP { get; set; }
        public string Ping { get; set; }
    }
    public class PingResultsDBContext : DbContext
    {
        public DbSet<PingResult> PingResults { get; set; }
    }
}