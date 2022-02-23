namespace ProjectsMonitor.Models
{
    public class DashViewModel
    {
        public string StudentRepo { get; set; }
        public string StudentName { get; set; }
        public Dictionary<DateTime, int> Commits { get; set; }
    }
}
