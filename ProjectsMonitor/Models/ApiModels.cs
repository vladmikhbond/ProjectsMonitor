// Типы для разбора json, получаемого от github api
// 
namespace ProjectsMonitor.Models
{
    public class CommitNode
    {
        public Commit commit { get; set; }
        public DateTime date => commit.committer.date;
        public string treeUrl => commit.tree.url;
    }
    public class Commit
    {
        public Committer committer { get; set; }
        public string message { get; set; }
        public Tree tree { get; set; }

    }
    public class Committer
    {
        public DateTime date { get; set; }
        public string name { get; set; }
    }

    public class Tree
    {
        public string url { get; set; }
    }

}
