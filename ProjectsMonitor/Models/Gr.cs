using System.ComponentModel.DataAnnotations;

namespace ProjectsMonitor.Models
{
    public class Gr
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string TutorName { get; set; }
        public DateTime StartDate { get; set; }
        //
        public List<Student> Students { get; set; }


    }
}
