﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dal;
using Microsoft.AspNetCore.Authorization;

namespace AdvScreen.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<ApplicationUser> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
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
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UserList() => View(_userManager.Users.ToList());

        public async Task<IActionResult> EditRole(string roleId)
        {
            // получаем пользователя
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {                
                return View(role);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> EditRole([Bind("Id,Name")] IdentityRole role)
        {
            try
            {
                IdentityRole thisRole = await _roleManager.FindByIdAsync(role.Id);
                thisRole.Name = role.Name;
                await _roleManager.UpdateAsync(thisRole);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }            
        }

        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return RedirectToAction("UserList");
            }

            return NotFound();
        }
    }
}