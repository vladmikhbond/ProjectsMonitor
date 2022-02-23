using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsMonitor.Data;
using ProjectsMonitor.Models;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ProjectsMonitor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ApplicationDbContext db, IHttpClientFactory clientFactory)
        {
            _db = db;
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Grs.Where(g => g.TutorName == User.Identity.Name).ToListAsync());
        }


        public async Task<IActionResult> DashBoard(int id) // id = grId
        {
            List<DashViewModel> dashes = null;
            try
            {
                dashes = await HashedRequestDashViewModels(id);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            // min & max date
            var dates = dashes.SelectMany(d => d.Commits.Keys).Distinct().ToList();
            if (dates.Count == 0)
            {
                return Content("No commits at all here.");
            }
            ViewBag.StartDate = dates.Min().Date;
            ViewBag.DatesCount = (dates.Max().Date - dates.Min().Date).Days + 1;           
            //ViewBag.StartDate = gr.StartDate.Date;

            return View(dashes);
        }

        #region Utils

        async Task< List<DashViewModel>> HashedRequestDashViewModels(int grId)
        {
            List<DashViewModel> dashes = null;
            var hash = _db.Hashs.SingleOrDefault(h => h.Tag == "commits" && h.UserName == User.Identity.Name);
            if (hash != null && hash.IsNotExpired)
            {
                // deserialize
                dashes = JsonSerializer.Deserialize<List<DashViewModel>>(new ReadOnlySpan<byte>(hash.Blob));
                return dashes;
            }
            // fresh dashes
            using HttpClient client = _clientFactory.CreateClient();
            dashes = await RequestDashViewModels(client, _db, grId);

            if (hash == null)
            {
                hash = new Hash { Tag = "commits", UserName = User.Identity.Name };
                _db.Hashs.Add(hash);
            }

            // serialize
            hash.Blob = JsonSerializer.SerializeToUtf8Bytes(dashes);
            hash.Date = DateTime.Now;
            _db.SaveChanges();

            return dashes;
        }
        static async Task<List<DashViewModel>> RequestDashViewModels (HttpClient client, ApplicationDbContext _db, int grId)
        {            
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
        #endregion
    }
}
