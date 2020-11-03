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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AdvScreen.Controllers
{
    [Authorize]
    public class AdvertisementsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdvertisementsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index()
        {
            ApplicationUser CurrentUser = GetCurrentUser();            
            return View(await _context.Advertisements.Where(a=>a.UserId == CurrentUser.Id).ToListAsync());
            
        }

        // GET: Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // GET: Advertisements/Create
        public IActionResult Create()
        {            
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
            return View();
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Duration,DurationInDays,Title,Text,PointId")] Advertisement advertisement)
        {            
            if (ModelState.IsValid)
            {

                var currentUser = GetCurrentUser();
                advertisement.UserId = currentUser.Id;
                advertisement.ApplicationUser = currentUser;
                advertisement.CreateDate = DateTime.Now;
                advertisement.FontSize = advertisement.Point.RecommendedFontSize;
                advertisement.Price = Pricing(advertisement.Duration, advertisement.DurationInDays, advertisement.PointId);
                _context.Add(advertisement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
            return View(advertisement);
        }

        public string GenerateAdvertisementNumber(Advertisement Advertisement)
        {
            DateTime date = Advertisement.CreateDate;

            return "";
        }

        public float Pricing(int seconds, int days, int pointId)
        {
            var pointPrice = _context.PointPrices.Where(p => p.PointId == pointId && p.Seconds == 10).FirstOrDefault();
            var sum = (seconds / pointPrice.Seconds) * pointPrice.Price * days;
            return sum;
        }

        // GET: Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Duration,DurationInDays,CreateDate,StartDate,EndDate,Title,Text,AdNumber,Price,UserId,PointId,FontSize")] Advertisement advertisement)
        {
            if (id != advertisement.Id)
            {
                return NotFound();
            }
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    var curAdv = _context.Advertisements.FirstOrDefault(a => a.Id == id);
                    
                    curAdv.PointId = advertisement.PointId;
                    curAdv.Title = advertisement.Title;
                    curAdv.Text = advertisement.Text;
                    curAdv.Duration = advertisement.Duration;
                    curAdv.DurationInDays = advertisement.DurationInDays;
                    curAdv.FontSize = advertisement.FontSize;
                    curAdv.Price = Pricing(curAdv.Duration, curAdv.DurationInDays, curAdv.PointId);                    
                    _context.Update(curAdv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementExists(advertisement.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                var adv = _context.Advertisements.FirstOrDefault(a => a.Id == id);
                return View(adv);
            }
            
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.Id == id);
        }

        public ApplicationUser GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (user != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userId != null)
                {
                    var apuser = _context.ApplicationUsers.SingleOrDefault(u => u.Id == userId);
                    return apuser;
                }
            }

            return null;
        }
    }
}
