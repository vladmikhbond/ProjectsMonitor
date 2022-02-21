namespace ProjectsMonitor.Models
{
    public class DashViewModel
    {
        public Student Student { get; set; }
        public Dictionary<DateTime, int> Commits { get; set; }
    }
}
