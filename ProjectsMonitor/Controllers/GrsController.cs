using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsMonitor.Data;
using ProjectsMonitor.Models;

namespace ProjectsMonitor.Controllers
{
    [Authorize]
    public class GrsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GrsController(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Grs
        public async Task<IActionResult> Index()
        {
            return View(await _db.Grs.Where(g => g.TutorName == User.Identity.Name).ToListAsync());
        }

        // GET: Grs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gr = await _db.Grs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gr == null)
            {
                return NotFound();
            }

            return View(gr);
        }

        // GET: Grs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Grs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Gr gr)
        {
            if (ModelState.IsValid)
            {
                gr.TutorName = User.Identity.Name;
                gr.StartDate = DateTime.Now;
                _db.Add(gr);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gr);
        }


        // GET: Grs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gr = await _db.Grs.FindAsync(id);
            if (gr == null)
            {
                return NotFound();
            }
            return View(gr);
        }

        // POST: Grs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TutorName,StartDate")] Gr gr)
        {
            if (id != gr.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(gr);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrExists(gr.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gr);
        }

        // GET: Grs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gr = await _db.Grs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gr == null)
            {
                return NotFound();
            }

            return View(gr);
        }

        // POST: Grs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gr = await _db.Grs.FindAsync(id);
            _db.Grs.Remove(gr);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrExists(int id)
        {
            return _db.Grs.Any(e => e.Id == id);
        }
    }
}
