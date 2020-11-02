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
    public class DaysForAdvsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DaysForAdvsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DaysForAdvs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DaysForAdvs.ToListAsync());
        }

        // GET: DaysForAdvs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysForAdv = await _context.DaysForAdvs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (daysForAdv == null)
            {
                return NotFound();
            }

            return View(daysForAdv);
        }

        // GET: DaysForAdvs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DaysForAdvs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Days,Name")] DaysForAdv daysForAdv)
        {
            if (ModelState.IsValid)
            {
                _context.Add(daysForAdv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(daysForAdv);
        }

        // GET: DaysForAdvs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysForAdv = await _context.DaysForAdvs.FindAsync(id);
            if (daysForAdv == null)
            {
                return NotFound();
            }
            return View(daysForAdv);
        }

        // POST: DaysForAdvs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Days,Name")] DaysForAdv daysForAdv)
        {
            if (id != daysForAdv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(daysForAdv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DaysForAdvExists(daysForAdv.Id))
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
            return View(daysForAdv);
        }

        // GET: DaysForAdvs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daysForAdv = await _context.DaysForAdvs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (daysForAdv == null)
            {
                return NotFound();
            }

            return View(daysForAdv);
        }

        // POST: DaysForAdvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var daysForAdv = await _context.DaysForAdvs.FindAsync(id);
            _context.DaysForAdvs.Remove(daysForAdv);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DaysForAdvExists(int id)
        {
            return _context.DaysForAdvs.Any(e => e.Id == id);
        }
    }
}
