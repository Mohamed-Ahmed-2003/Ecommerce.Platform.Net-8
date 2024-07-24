using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OtlobLap.Auth;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Services;
using OtlobLap.ViewModels;
using System.Security.Claims;

namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class PermissionsController (IProfileService profileService, RoleManager<IdentityRole<int>> roleManager) : BaseController(profileService)
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager = roleManager;
        public async  Task  <IActionResult> Index(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);  

            var claims =  _roleManager.GetClaimsAsync(role).Result.Select(c=>c.Value).ToList();

            var allPermissions = PermissionsAuth.GetPermissionsList().
                Select(p => new RoleClaimsVM { Value = p })
                .ToList();

            foreach (var permission in allPermissions)
            {
                if (claims.Contains(permission.Value))
                    permission.Selected = true;
            }

            var model = new PermissionsVM
            {
                RoleId = roleId,
                RoleName = role.Name,
                RoleClaims = allPermissions
            };
            ViewBag.view = Utility.GetTranslation(model.RoleName);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(PermissionsVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            var claims = await _roleManager.GetClaimsAsync(role);

            foreach (var  claim in claims)
                    await _roleManager.RemoveClaimAsync(role, claim);

            var newClaims = model.RoleClaims.Where(r => r.Selected).ToList();

            foreach (var claim in newClaims)
               await _roleManager.AddClaimAsync(role, new Claim("Permission",claim.Value));
            return RedirectToAction("Index","Roles");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.Controller = Utility.GetTranslation("RolesManagement");
        }

    }
}
