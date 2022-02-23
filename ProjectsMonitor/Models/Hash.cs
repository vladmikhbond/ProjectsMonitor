using System.ComponentModel.DataAnnotations;

namespace ProjectsMonitor.Models
{
    public class Hash
    {
        const int EXPIRATION_IN_HOURS = 6;

        [Key]
        public int GrId { get; set; }
        public DateTime Date { get; set; }
        public byte[] Blob { get; set; }
        public bool IsNotExpired => 
           Date + TimeSpan.FromHours(EXPIRATION_IN_HOURS) > DateTime.Now;
    }
}
