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

namespace AdvScreen.Controllers
{
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
                , Blocked = user.Blocked
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
