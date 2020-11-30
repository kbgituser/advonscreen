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
    public class AdvertisementStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvertisementStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdvertisementStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdvertisementStatuses.ToListAsync());
        }

        // GET: AdvertisementStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisementStatus = await _context.AdvertisementStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisementStatus == null)
            {
                return NotFound();
            }

            return View(advertisementStatus);
        }

        // GET: AdvertisementStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdvertisementStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NameRu")] AdvertisementStatus advertisementStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(advertisementStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(advertisementStatus);
        }

        // GET: AdvertisementStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisementStatus = await _context.AdvertisementStatuses.FindAsync(id);
            if (advertisementStatus == null)
            {
                return NotFound();
            }
            return View(advertisementStatus);
        }

        // POST: AdvertisementStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NameRu")] AdvertisementStatus advertisementStatus)
        {
            if (id != advertisementStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisementStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementStatusExists(advertisementStatus.Id))
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
            return View(advertisementStatus);
        }

        // GET: AdvertisementStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisementStatus = await _context.AdvertisementStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisementStatus == null)
            {
                return NotFound();
            }

            return View(advertisementStatus);
        }

        // POST: AdvertisementStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisementStatus = await _context.AdvertisementStatuses.FindAsync(id);
            _context.AdvertisementStatuses.Remove(advertisementStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertisementStatusExists(int id)
        {
            return _context.AdvertisementStatuses.Any(e => e.Id == id);
        }
    }
}
