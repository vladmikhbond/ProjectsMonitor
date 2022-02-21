using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsMonitor.Data;
using ProjectsMonitor.Models;
using System.Net.Http.Headers;
using System.Net.Sockets;
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
            using HttpClient client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("product", "1"));

            var gr = _db.Grs.Find(id);
            var students = _db.Students.Where(s => s.GrId == id).ToList();
            var dashes = new List<DashViewModel>();

            foreach (var student in students)
            {
                var modelItem = new DashViewModel { Student = student };                
                try
                {
                    var uri = GetCommitsUri(student.Repo);
                    modelItem.Commits = (await RequestData<List<Node>>(client, uri))
                        .GroupBy(n => n.commit.committer.date.Date)
                        .Select(g => new { Date = g.Key, Number = g.Count() })
                        .ToDictionary(x => x.Date, x => x.Number);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                dashes.Add(modelItem);
            }
            
            dashes = dashes.OrderBy(d => d.Student.Name).ToList();

            // min & max date
            var dates = dashes.SelectMany(d => d.Commits.Keys).Distinct().ToList();
            ViewBag.StartDate = dates.Min();
            ViewBag.DatesCount = (dates.Max() - dates.Min()).Days + 1;           
            //ViewBag.StartDate = gr.StartDate.Date;

            return View(dashes);
        }

        #region Utils
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
