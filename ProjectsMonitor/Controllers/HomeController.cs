using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsMonitor.Data;
using ProjectsMonitor.Models;
using ProjectsMonitor.Services;
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
        private readonly GithubService _github;

        public HomeController(GithubService github, ApplicationDbContext db)
        {
            _github = github;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _db.Grs
                .Where(g => g.TutorName == User.Identity.Name)
                .ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> ClearHash(int id)  // id = GrId
        {
            var hash = await _db.Hashs.FindAsync(id);
            if (hash != null)
            {
                _db.Hashs.Remove(hash);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DashBoard(int id) // id = grId
        {
            // get model from github or hashe
            List<DashViewModel> dashes = null;
            try
            {   
                dashes = await _github.HashedRequestDashViewModels(id);               
            } 
            catch (Exception ex)
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
            //ViewBag.StartDate = gr.StartDate.Date;   // TODO:

            return View(dashes);
        }

    }
}
