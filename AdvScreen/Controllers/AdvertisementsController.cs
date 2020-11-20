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
            if (await _userManager.IsInRoleAsync(CurrentUser, "Admin"))
            {
                return View("IndexAdmin",  _context.Advertisements.Include(a=>a.AdvertisementStatus).AsAsyncEnumerable());
                //return View(await _context.Advertisements.ToListAsync());
            }                
            return View(await _context.Advertisements.Where(a=>a.UserId == CurrentUser.Id).Include(a => a.AdvertisementStatus).ToListAsync());
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
            if (!_context.PointPrices.Any())
            {
                TempData["Message"] = "Цены точек не определены в справочнике";
                return RedirectToAction("ErrorHandle", "Home");
            }
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
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
            if (ModelState.IsValid)
            {
                if (!_context.PointPrices.Any())
                {                    
                    //TempData["Message"] = "Цены точек не определены в справочнике";
                    //return RedirectToAction("ErrorHandle", "Home");

                    ViewData["Message"]= "Цены точек не определены в справочнике";
                    return View();
                }

                var currentUser = GetCurrentUser();
                advertisement.UserId = currentUser.Id;
                advertisement.ApplicationUser = currentUser;
                advertisement.CreateDate = DateTime.Now;
                var point = _context.Points.FirstOrDefault(p => p.Id == advertisement.PointId);
                advertisement.FontSize = point.RecommendedFontSize;
                advertisement.Price = Pricing(advertisement.Duration, advertisement.DurationInDays, advertisement.PointId);
                advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "Created").Id;
                advertisement.AdNumber = GenerateAdvertisementNumber(advertisement);
                _context.Add(advertisement);                
                await _context.SaveChangesAsync();
                
                //AdvertisementStatusHistory history = new AdvertisementStatusHistory();
                //var status = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "Created");
                //history.AdvertisementId = advertisement.Id;
                //history.AdvertisementStatusId = status.Id;
                //_context.AdvertisementStatusHistories.Add(history);
                //await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Edit),new { id = advertisement.Id});
            }
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
            return View(advertisement);
        }

        public async Task<IActionResult> SendToModeration(int? id)
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

            var currentUser = GetCurrentUser();
            if (advertisement.ApplicationUser == currentUser || await _userManager.IsInRoleAsync (currentUser, "Admin"))
            {
                advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "InModeration").Id;
                _context.Update(advertisement);
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.Error = "Только Владлелец может отправить на объявление на модерацию";
                return RedirectToAction("Error", "Home");
            }

            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToAction("Moderate", new { id = id });
            }
            return RedirectToAction("Edit", new { id = id });
        }



        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Moderate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null || advertisement.AdvertisementStatus.Name != "InModeration")
            {
                return NotFound();
            }            
            return View(advertisement);
        }

        public async Task<IActionResult> Finish(int? id)
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

            advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == AdvertisementStatusEnum.Finished.ToString()).Id;
            _context.Update(advertisement);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = id });
        }

        public async Task<IActionResult> PassModeration(int? id)
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

            advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "ForPayment").Id;
            _context.Update(advertisement);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { id = id });
        }

        public async Task<IActionResult> RefuseModeration(int? id)
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

            advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "Created").Id;
            _context.Update(advertisement);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = id });
        }


        public string GenerateAdvertisementNumber(Advertisement Advertisement)
        {
            DateTime date = GetDateTimeWithoutSeconds (Advertisement.CreateDate);

            //int advCount = _context.Advertisements.Where(a => GetDateTimeWithoutSeconds(a.CreateDate)==date).ToList().Count();
            int advCount = _context.Advertisements.Where(a =>a.CreateDate.Date == date && a.CreateDate.Hour == date.Hour && a.CreateDate.Minute == date.Minute).Count();
            string res = date.ToString("ddMMyyyyHHmm") + advCount.ToString().PadLeft(4, '0');
            return res;
        }

        public DateTime GetDateTimeWithoutSeconds(DateTime Dt)
        {
            //var t = new DateTime(Dt.Year,Dt.Month,Dt.Day,Dt.Hour,Dt.Minute,Dt.Second);
            return new DateTime(Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, Dt.Second);
            //return new DateTime(Dt.Year, Dt.Month, Dt.Day, Dt.Hour, Dt.Minute, 0, DateTimeKind.Utc);
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

            ApplicationUser CurrentUser = GetCurrentUser();
            if (await _userManager.IsInRoleAsync(CurrentUser, "Admin"))
            {
                return View("EditAdmin", advertisement);
            }
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
                    curAdv.AdNumber = GenerateAdvertisementNumber(curAdv);

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

        

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Payment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements.FirstOrDefaultAsync(m => m.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PaymentConfirmed(int? id, bool wait)
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

            if (advertisement.AdvertisementStatus.Name != "ForPayment")
            {
                var paymentStatus = _context.AdvertisementStatuses.SingleOrDefault(a => a.Name == "ForPayment");
                TempData["Message"] = "У объявления статус " + advertisement.AdvertisementStatus.NameRu + ". Оплатить можно объявление со статусом " + paymentStatus.NameRu;
                return RedirectToAction("Payment", new { id = id });
            }

            if (wait)
            {
                advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == "Wating").Id;
            }
            else
            {
                var activeStatus = _context.AdvertisementStatuses.FirstOrDefault(a => a.Name == "Active");
                if (_context.Advertisements.Where(a => a.PointId == advertisement.PointId && a.AdvertisementStatusId == activeStatus.Id).Sum(a=>a.Duration) <= advertisement.Point.CycleSize)
                {
                    advertisement.AdvertisementStatusId = activeStatus.Id;
                    advertisement.StartDate = DateTime.Now;
                    advertisement.EndDate = advertisement.StartDate.AddDays(advertisement.DurationInDays);
                }
                else
                {
                    ViewBag.Message = "Цикл полный. Объявление не помещяется.";
                    return RedirectToAction("Payment", new { id = id });
                }
            }

            //advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == (wait? "Wating" : "Active")).Id;
            
            Payment payment = new Payment();
            payment.AdvertisementId = advertisement.Id;
            payment.Days = advertisement.DurationInDays;
            payment.Sum = advertisement.Price;
            payment.CreateDate = DateTime.Now;
            _context.Payments.Add(payment);
            _context.Advertisements.Update(advertisement);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = id});
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

        public async Task<IActionResult> FinishAdvertisements()
        {
            var activeAdvertisementsToFinish = _context.Advertisements.Where(a => a.AdvertisementStatus.Name == "Active" && a.EndDate <= DateTime.Now).Include(a=>a.AdvertisementStatus);
            var finishStatus = _context.AdvertisementStatuses.SingleOrDefault(a => a.Name == "Finished");
            var advCount =  activeAdvertisementsToFinish.Count();
            foreach (var ad in activeAdvertisementsToFinish)
            {
                ad.AdvertisementStatusId= finishStatus.Id;
            }
            _context.SaveChangesAsync();
            ViewBag.Count = advCount;

            //await Task.Factory.StartNew(() => FinishOutdatedAdvertisements());
            return View();
        }

        public void FinishOutdatedAdvertisements()
        {
            var activeAdvertisementsToFinish = _context.Advertisements.Where(a => a.AdvertisementStatus.Name == "Active" && a.EndDate <= DateTime.Now);
            var finishStatus = _context.AdvertisementStatuses.SingleOrDefault(a => a.Name == "Finished");
            foreach (var ad in activeAdvertisementsToFinish)
            {
                ad.AdvertisementStatus.Id = finishStatus.Id;
            }
            _context.SaveChangesAsync();
        }
    }
}
