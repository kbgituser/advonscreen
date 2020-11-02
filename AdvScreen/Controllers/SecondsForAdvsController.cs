using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dal.Data;
using Dal.Models;

namespace AdvScreen.Controllers
{
    public class SecondsForAdvsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SecondsForAdvsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SecondsForAdvs
        public async Task<IActionResult> Index()
        {
            return View(await _context.SecondsForAdvs.ToListAsync());
        }

        // GET: SecondsForAdvs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secondsForAdv = await _context.SecondsForAdvs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (secondsForAdv == null)
            {
                return NotFound();
            }

            return View(secondsForAdv);
        }

        // GET: SecondsForAdvs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SecondsForAdvs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Seconds,Name")] SecondsForAdv secondsForAdv)
        {
            if (ModelState.IsValid)
            {
                _context.Add(secondsForAdv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(secondsForAdv);
        }

        // GET: SecondsForAdvs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secondsForAdv = await _context.SecondsForAdvs.FindAsync(id);
            if (secondsForAdv == null)
            {
                return NotFound();
            }
            return View(secondsForAdv);
        }

        // POST: SecondsForAdvs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Seconds,Name")] SecondsForAdv secondsForAdv)
        {
            if (id != secondsForAdv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secondsForAdv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecondsForAdvExists(secondsForAdv.Id))
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
            return View(secondsForAdv);
        }

        // GET: SecondsForAdvs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secondsForAdv = await _context.SecondsForAdvs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (secondsForAdv == null)
            {
                return NotFound();
            }

            return View(secondsForAdv);
        }

        // POST: SecondsForAdvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var secondsForAdv = await _context.SecondsForAdvs.FindAsync(id);
            _context.SecondsForAdvs.Remove(secondsForAdv);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecondsForAdvExists(int id)
        {
            return _context.SecondsForAdvs.Any(e => e.Id == id);
        }
    }
}
