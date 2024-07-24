using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Services;

namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class RolesController (IProfileService profileService , RoleManager<IdentityRole<int>> roleManager) : BaseController (profileService)
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager = roleManager;

    
        public async Task<IActionResult> Index()
        {
            ViewBag.Controller = Utility.GetTranslation("RolesManagement");
        

            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string roleName)
        {
            try
            {
                if (roleName != null && _roleManager.FindByNameAsync(roleName).Result is null)
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName.Trim()));
                    var role = await _roleManager.FindByNameAsync(roleName);
                    return PartialView("_RoleRow",role);
                }
                return BadRequest();
            } catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string roleId, string roleName)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    role.Name = roleName.Trim();
                    await _roleManager.UpdateAsync(role);
                    return Ok(role);
                }
                else
                    throw new Exception();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch 
            {
                return BadRequest();
            }
        }


    }
}
