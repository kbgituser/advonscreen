using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dal.Data;
using Dal.Models;
using Microsoft.AspNetCore.Authorization;

namespace AdvScreen.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PointPricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PointPricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PointPrices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PointPrices.Include(p => p.Point);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PointPrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointPrice = await _context.PointPrices
                .Include(p => p.Point)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointPrice == null)
            {
                return NotFound();
            }

            return View(pointPrice);
        }

        // GET: PointPrices/Create
        public IActionResult Create()
        {
            ViewData["PointId"] = new SelectList(_context.Set<Point>(), "Id", "Name");
            return View();
        }

        // POST: PointPrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PointId,Seconds,Price")] PointPrice pointPrice)
        {
            if (ModelState.IsValid)
            {
                pointPrice.SetDate = DateTime.Now;
                _context.Add(pointPrice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PointId"] = new SelectList(_context.Set<Point>(), "Id", "Id", pointPrice.PointId);
            return View(pointPrice);
        }

        // GET: PointPrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointPrice = await _context.PointPrices.FindAsync(id);
            if (pointPrice == null)
            {
                return NotFound();
            }
            ViewData["PointId"] = new SelectList(_context.Set<Point>(), "Id", "Id", pointPrice.PointId);
            return View(pointPrice);
        }

        // POST: PointPrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PointId,Seconds,Price,SetDate")] PointPrice pointPrice)
        {
            if (id != pointPrice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pointPrice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointPriceExists(pointPrice.Id))
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
            ViewData["PointId"] = new SelectList(_context.Set<Point>(), "Id", "Id", pointPrice.PointId);
            return View(pointPrice);
        }

        // GET: PointPrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pointPrice = await _context.PointPrices
                .Include(p => p.Point)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointPrice == null)
            {
                return NotFound();
            }

            return View(pointPrice);
        }

        // POST: PointPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pointPrice = await _context.PointPrices.FindAsync(id);
            _context.PointPrices.Remove(pointPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointPriceExists(int id)
        {
            return _context.PointPrices.Any(e => e.Id == id);
        }
    }
}
