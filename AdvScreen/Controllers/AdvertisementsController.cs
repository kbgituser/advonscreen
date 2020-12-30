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


using System.IO;
using CoreHtmlToImage;
using Microsoft.AspNetCore.Hosting;
using AdvScreen.Models;

namespace AdvScreen.Controllers
{
    [Authorize]
    public class AdvertisementsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IWebHostEnvironment _env;

        public AdvertisementsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor
            , IWebHostEnvironment appEnvironment
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _env = appEnvironment;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string adNumber
            , string advertisementStatus
            ,string pointId
            , string price
            , string sortOrder
            , int? p
            )
        {
            //ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["points"] = new SelectList(_context.Points, "Id", "Name");
            ViewData["advertisementStatuses"] = new SelectList(_context.AdvertisementStatuses, "Id", "NameRu");
            
            ApplicationUser CurrentUser = GetCurrentUser();
            
            //var advertisements = _context.Advertisements.AsQueryable();
            var advertisements = _context.Advertisements.AsQueryable();

            ///////
            //advertisements = null;
            //var test = advertisements.First();
            //var test2 = test.AdNumber;
            ///////
            

            AdvertisementStatus curStatus;
            int curPointId, curPrice;
            if (!string.IsNullOrEmpty(adNumber))
            {
                advertisements = advertisements.Where(a => a.AdNumber.Contains(adNumber));
                ViewData["adNumber"] = adNumber;
            }

            if (!string.IsNullOrEmpty(advertisementStatus))
            {
                curStatus = _context.AdvertisementStatuses.FirstOrDefault(s => s.Id == Int32.Parse(advertisementStatus));
                advertisements = advertisements.Where(a => a.AdvertisementStatus == curStatus);
                ViewData["advertisementStatuses"] = new SelectList(_context.AdvertisementStatuses, "Id", "NameRu", curStatus.Id);
                ViewData["advertisementStatus"] = curStatus.Id;
            }

            if (!string.IsNullOrEmpty(pointId))
            {
                curPointId = Int32.Parse(pointId);
                advertisements = advertisements.Where(a => a.PointId == curPointId);                
                ViewData["points"] = new SelectList(_context.Points, "Id", "Name", curPointId);
                ViewData["pointId"] = curPointId;
            }

            if (!string.IsNullOrEmpty(price))
            {
                curPrice = Int32.Parse(price);
                advertisements = advertisements.Where(a => a.Price<=curPrice);                
            }
            
            if (await _userManager.IsInRoleAsync(CurrentUser, "Admin"))
            {
                advertisements = advertisements.Include(a => a.AdvertisementStatus);
                //return View("IndexAdmin",  advertisements.Include(a=>a.AdvertisementStatus).AsQueryable());
            }
            else
            {
                //return View(advertisements.Where(a => a.UserId == CurrentUser.Id).Include(a => a.AdvertisementStatus));
                advertisements = advertisements.Where(a => a.UserId == CurrentUser.Id).Include(a => a.AdvertisementStatus);
            }
            ViewData["adNumberSort"] = sortOrder == "adNumber" ? "adNumberDesc" : "adNumber";
            ViewData["createDateSort"] = sortOrder == "createDate" ? "createDateDesc" : "createDate";
            switch (sortOrder)
            {
                case "adNumberDesc":
                    advertisements = advertisements.OrderByDescending(s => s.AdNumber);
                    break;
                case "adNumber":
                    advertisements = advertisements.OrderBy(s => s.AdNumber);
                    break;
                case "createDateDesc":
                    advertisements = advertisements.OrderByDescending(s => s.CreateDate);
                    break;
                case "createDate":
                    advertisements = advertisements.OrderBy(s => s.CreateDate);
                    break;
                default:
                    advertisements = advertisements.OrderBy(s => s.AdNumber);
                    break;
            }

            int pageSize = 10;
            int pN = p ?? 1;
            
            ViewBag.PageNo = pN;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = advertisements.Count();

            if (await _userManager.IsInRoleAsync(CurrentUser, "Admin"))
            {
                return View("IndexAdmin",await PaginatedList<Advertisement>.CreateAsync(advertisements.AsNoTracking(), pN, pageSize));
                //return View("IndexAdmin", advertisements);
            }
            else
            {                
                return View(await PaginatedList<Advertisement>.CreateAsync(advertisements.AsNoTracking(), pN, pageSize));
                //return View(advertisements);
            }
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
        public async Task<IActionResult> Create([Bind("Id,Duration,DurationInDays,Title,Text,PointId,AdvertisementType")] Advertisement advertisement)
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
                advertisement.BackgroundColor = "#FFFFFF";                
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
            //ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            //ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            //ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
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
            ViewData["Seconds"] = new SelectList(_context.SecondsForAdvs, "Seconds", "Name");
            ViewData["Days"] = new SelectList(_context.DaysForAdvs, "Days", "Name");
            ViewData["Points"] = new SelectList(_context.Points, "Id", "Name");
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

        //public async Task<IActionResult> HtmlToPng(Advertisement model)
        public async Task<FileResult> HtmlToPng(int id)
        {            
            var adv = _context.Advertisements.FirstOrDefault(a => a.Id == id);
            var converter = new HtmlConverter();


            

            var html = "<div><strong>Hello</strong> World!</div>";

            html = "<style>";
            html += ".PointScreen {height: 960px; width: 540px; border: 2px solid; font-size: 99px;}";
            html += "</style>";
            html += "<div class='PointScreen'>";
            html += adv.Text;
            html += "</div>";
            var bytes = converter.FromHtmlString(html);
            return File(bytes, "image/png");            
        }
        

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Duration,DurationInDays,CreateDate,StartDate,EndDate,Title,Text,AdNumber,Price,UserId,PointId,FontSize,BackgroundColor,AdvertisementType,Video")] Advertisement advertisement, IFormFile uploadedFile, bool forModeration)
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

                    ApplicationUser CurrentUser = GetCurrentUser();
                    if (!await _userManager.IsInRoleAsync(CurrentUser, "Admin")
                        && !(curAdv.AdvertisementStatus.Name == AdvertisementStatusEnum.Created.ToString()
                        || curAdv.AdvertisementStatus.Name == AdvertisementStatusEnum.Finished.ToString()
                        )
                        )
                    {
                        TempData["Message"] = "Можно сохранять объявления только со статусом Активный и Завершенный";
                        return RedirectToAction("ErrorHandle", "Home");
                    }

                    curAdv.PointId = advertisement.PointId;
                    curAdv.Title = advertisement.Title;
                    curAdv.Text = advertisement.Text;
                    curAdv.Duration = advertisement.Duration;
                    curAdv.DurationInDays = advertisement.DurationInDays;
                    curAdv.FontSize = advertisement.FontSize;
                    curAdv.Price = Pricing(curAdv.Duration, curAdv.DurationInDays, curAdv.PointId);
                    curAdv.AdNumber = GenerateAdvertisementNumber(curAdv);
                    curAdv.BackgroundColor = advertisement.BackgroundColor;
                    curAdv.AdvertisementType = advertisement.AdvertisementType;
                    if (await _userManager.IsInRoleAsync(CurrentUser, "Admin"))
                        {
                        curAdv.Video = advertisement.Video;
                    }
                    
                    _context.Update(curAdv);


                    if (uploadedFile != null)
                    {
                        // путь к папке Files
                        // Initialization.  
                        var fileSize = uploadedFile.Length;
                        int allowedFileSize = 1000000;
                        // Settings.  
                        bool isValid = fileSize <= allowedFileSize;

                        if (isValid)
                        {
                            string fileExtension = DateTime.Now.Ticks + System.IO.Path.GetExtension(uploadedFile.FileName);
                            //string path = _env.ContentRootPath + "/Files/Images/" + curAdv.AdNumber + fileExtension;
                            string path = _env.WebRootPath + "/Files/Images/" + curAdv.AdNumber  + fileExtension;
                            string curFilePath = _env.WebRootPath + curAdv.ImagePath;
                            if (System.IO.File.Exists(curFilePath))
                            {
                                System.IO.File.Delete(curFilePath);
                            }

                            // сохраняем файл в папку Files в каталоге wwwroot
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(fileStream);
                            }
                            curAdv.ImagePath = "/Files/Images/" + curAdv.AdNumber +fileExtension;
                        }
                        else
                        {
                            TempData["photoMessage"] = "Фотография должна быть меньше 1 Мегабайта!!!";
                        }
                    }

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
                //return View(adv);

                if (forModeration)
                {
                    return RedirectToAction("SendToModeration", new { adv.Id });
                }

                return RedirectToAction("Edit", new { adv.Id });
            }
            return RedirectToAction("Edit", new { id });
            //return View(advertisement);
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
                advertisement.AdvertisementStatusId = _context.AdvertisementStatuses.FirstOrDefault(s => s.Name == AdvertisementStatusEnum.Waiting.ToString()).Id;
            }
            else
            {
                var activeStatus = _context.AdvertisementStatuses.FirstOrDefault(a => a.Name == AdvertisementStatusEnum.Active.ToString());
                int takenTime = _context.Advertisements.Where(a => a.PointId == advertisement.PointId && a.AdvertisementStatusId == activeStatus.Id).Sum(a => a.Duration);
                if (takenTime <= advertisement.Point.CycleSize)
                {
                    advertisement.AdvertisementStatusId = activeStatus.Id;
                    advertisement.StartDate = DateTime.Now;
                    advertisement.EndDate = advertisement.StartDate.AddDays(advertisement.DurationInDays);
                }
                else
                {
                    TempData["Message"] = "Цикл полный. Объявление не помещяется.";
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

        public async Task<IActionResult> MakeWaitingAdsActive()
        {
            int pointId;
            int count = 0;
            AdvertisementStatus activeStatus;
            activeStatus = _context.AdvertisementStatuses.SingleOrDefault(a => a.Name == AdvertisementStatusEnum.Active.ToString());

            string test2 = AdvertisementStatusEnum.Waiting.ToString();
            var test = _context.Advertisements.Where(a =>
                a.AdvertisementStatus.Name == AdvertisementStatusEnum.Waiting.ToString()).ToList();

            foreach (var point in _context.Points.ToList())
            {
                pointId = point.Id;
                
                //выбираем все объявления со статусом "в ожидании" для каждой точки отдельно
                // сортируем их по дате создания платежа, чтобы те которые оплачивали раньше, были в очереди первыми
                var waitingAdvertisements = _context.Advertisements.Where(a => 
                a.AdvertisementStatus.Name == AdvertisementStatusEnum.Waiting.ToString()
                && a.PointId == pointId)
                    // берем последнюю оплату конкретного объявления. по дате оплаты сортируем все объявления.
                    .OrderBy(a=>a.Payments.OrderBy(p=>p.CreateDate).Last().CreateDate)
                    .Include(a => a.AdvertisementStatus).ToList();
                int freeSpaceInPoint = point.CycleSize - _context.Advertisements.Where(a => a.AdvertisementStatus.Name == "Active" && a.PointId == pointId).Sum(a => a.Duration);
                
                var waitingToActive = waitingAdvertisements.TakeWhile(x => (count += x.Duration) <= freeSpaceInPoint);

                
                foreach (var ad in waitingToActive)
                {
                    ad.AdvertisementStatusId = activeStatus.Id;
                    ad.StartDate = DateTime.Now;
                    ad.EndDate = ad.StartDate.AddDays(ad.DurationInDays);
                }                
            }
            _context.SaveChangesAsync();
            return View();
        }
    }
}
