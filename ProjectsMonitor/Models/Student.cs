using System.ComponentModel.DataAnnotations;

namespace ProjectsMonitor.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Repo { get; set; }
        [Required]
        public int GrId { get; set; }
        //
        public Gr Gr { get; set; }

    }
}
