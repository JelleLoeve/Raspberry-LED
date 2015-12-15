using System.Data.Entity;

namespace Raspberry_LED.Models
{
    public class Upload
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string Alias { get; set; }
        public string Type { get; set; }
    }
    public class UploadDBContext : DbContext
    {
        public DbSet<Upload> Uploads { get; set; }

    }
}