using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAn1.Areas.Admin.Models;
using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public RoleManager<IdentityRole> RoleManager;
        public UserManager<KhachHang> userManager;
        public IdentityUser IdentityUser;


        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<KhachHang> userManager)
        {
            this.RoleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRole model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await RoleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                { return RedirectToAction("ListRole"); }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }
 
        public IActionResult ListRole()
        {
            var roles = RoleManager.Roles;
            return View(roles);
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {

                return View("NotFound");
            }

            var model = new EditRole
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in await userManager.GetUsersInRoleAsync(role.Name))
            {
                model.Users.Add(user.UserName);
            }
            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRole model)
        {
            var role = await RoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {

                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return View("NotFound");
            }

            var model = new List<UserRole>();


            foreach (var user in await userManager.Users.ToListAsync())
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    //IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.IsSelected = true;
                }
                else
                {
                    userRole.IsSelected = false;
                }
                model.Add(userRole);

            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRole> model, string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {

                return View("NotFound");
            }
            else
            {
                var result = await RoleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRole");
            }
        }

    }
}