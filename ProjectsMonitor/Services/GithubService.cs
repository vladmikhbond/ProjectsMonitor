namespace ProjectsMonitor.Services
{
    //public class GithubService
    using Microsoft.AspNetCore.Http;
    using ProjectsMonitor.Data;
    using ProjectsMonitor.Models;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    public class GithubService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _clientFactory;

        public GithubService(IHttpContextAccessor httpContextAccessor)
        {
            var http = httpContextAccessor.HttpContext;
            _db = http.RequestServices.GetService<ApplicationDbContext>();
            _clientFactory = http.RequestServices.GetService<IHttpClientFactory>();
        }

        public async Task<List<DashViewModel>> HashedRequestDashViewModels(int grId)
        {
            List<DashViewModel> dashes = null;
            var hash = _db.Hashs.SingleOrDefault(h => h.GrId == grId);
            if (hash != null && hash.IsNotExpired)
            {
                // return hashed data
                dashes = JsonSerializer.Deserialize<List<DashViewModel>>(new ReadOnlySpan<byte>(hash.Blob));
                return dashes;
            }

            // send request to github
            dashes = await RequestDashViewModels(grId);

            if (hash == null)
            {
                // no record in DB
                hash = new Hash { GrId = grId };
                _db.Hashs.Add(hash);
            }
            // put hash to DB
            hash.Blob = JsonSerializer.SerializeToUtf8Bytes(dashes);
            hash.Date = DateTime.Now;

            _db.SaveChanges();
            return dashes;
        }
        
        public async Task<List<DashViewModel>> RequestDashViewModels(int grId)
        {
            using HttpClient client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("product", "1"));

            var gr = _db.Grs.Find(grId);
            var students = _db.Students.Where(s => s.GrId == grId).ToList();
            var dashes = new List<DashViewModel>();

            foreach (var student in students)
            {
                var modelItem = new DashViewModel { StudentRepo = student.Repo, StudentName = student.Name };
                var uri = GetCommitsUri(student.Repo);

                modelItem.Commits = (await RequestData<List<CommitNode>>(client, uri))
                    .GroupBy(n => n.commit.committer.date.Date)
                    .Select(g => new { Date = g.Key, Number = g.Count() })
                    .ToDictionary(x => x.Date, x => x.Number);

                dashes.Add(modelItem);
            }

            dashes = dashes.OrderBy(d => d.StudentName).ToList();
            return dashes;
        }

        private static string GetCommitsUri(string studentRepo)
        {
            Regex regex = new Regex("(https://)(github.com/)(.*)");
            Match match = regex.Match(studentRepo);
            if (!match.Success)
                throw new ApplicationException("Wrong student repo.");

            return $"{match.Groups[1]}api.{match.Groups[2]}repos/{match.Groups[3]}/commits";
        }

        private static async Task<T> RequestData<T>(HttpClient client, string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException("Response fail: " + response.ReasonPhrase);
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }

    }

}
