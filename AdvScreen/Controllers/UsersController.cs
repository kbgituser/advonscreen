using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dal.Data;
using Dal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdvScreen.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ApplicationDbContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public UserManager<ApplicationUser> UserManager
        {
            get { return _userManager; }
            private set { _userManager = value; }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get { return _roleManager; }
            private set { _roleManager = value; }
        }

        // GET: Users
        public ActionResult Index()
        {
            var users = UserManager.Users;
            return View(users);
        }

        public ActionResult ChangePassword(string Id)
        {
            return View(model:Id);
        }
        
        [HttpPost]
        public async Task <ActionResult> ChangePassword(string userId, string password)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(userId);
                var token = await UserManager.GeneratePasswordResetTokenAsync(user);
                var result = await UserManager.ResetPasswordAsync(user, token, password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(userId);
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //ApplicationUser user = UserManager.GetUserAsync( .Find(id);
            ApplicationUser user = Context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel user)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser appUser = new ApplicationUser { Email = user.Email, UserName = user.Email };
                appUser.CreateDate = DateTime.Now;
                IdentityResult result = await UserManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id
                //, Email = user.Email
                ,
                Name = user.Name
                , PhoneNumber = user.PhoneNumber
                , Blocked = user.Blocked
                , EmailConfirmed = user.EmailConfirmed
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    //user.Email = model.Email;
                    user.Name = model.Name;
                    user.Blocked = model.Blocked;
                    user.EmailConfirmed = model.EmailConfirmed;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UserPoints(string id)
        {
            var userPoints = _context.UserPoints.Where(u => u.UserId == id.ToString())
                .ToList();
            ViewBag.userId = id;
            ViewBag.points = new SelectList(_context.Points.ToList(), "Id", "Name") ;

            return View(userPoints);
        }

        public async Task<ActionResult> AddPoint(string userId, int pointId)
        {
            var newUserPoint = new UserPoint();
            newUserPoint.UserId = userId;
            newUserPoint.PointId = pointId;

            if (!_context.UserPoints.Any(u => u.UserId == userId && u.PointId == pointId))
            {
                _context.UserPoints.Add(newUserPoint);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction ("UserPoints", new { id = userId});
        }

        public async Task<ActionResult> RemovePoint(string userId, int pointId)
        {
            var delUserPoint = _context.UserPoints.FirstOrDefault(u=>u.UserId == userId && u.PointId == pointId);
            if (delUserPoint != null)
            {
                _context.UserPoints.Remove(delUserPoint);
                await _context.SaveChangesAsync();
            }                
            return RedirectToAction("UserPoints", new { id = userId });
        }

        public ActionResult CurrentUserLink()
        {

            var user = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (user != null)
            {
                ViewBag.UserId = user.Value;
            }
            return PartialView();
        }
    }
}
