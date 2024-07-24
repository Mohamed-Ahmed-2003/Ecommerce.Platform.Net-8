using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Areas.Admin.Controllers
{

    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class UserController( IUserManagerService userManagerService  , IProfileService profileService) : BaseController(profileService)
    {
        private readonly IUserManagerService _userManagerService = userManagerService;


        [Authorize(Utility.Permissions.User.View)]
        public IActionResult Index()
        {
            var model = new UserManagementVM
            {
                Users = _userManagerService.GetApplicationUsers().Result,
                Roles = new SelectList(_userManagerService.GetRolesAsync().Result),
            };

			return View( model);
        }

        [Authorize(Utility.Permissions.User.Add)]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserManagementVM model)
        {
            try
            {
              await _userManagerService.RegisterAsync(model.RegisterVM);

                return RedirectToAction("Index");
            }
            catch
            {

                return RedirectToAction("Index");
            }

        }

        [Authorize(Utility.Permissions.User.Edit)]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserManagementVM model)
        {

            if (model.ProfileToUpdate != null && model.ProfileToUpdate.UserInfo?.Id != 0)
            {
              
                // Call the service method to update user information
                await _userManagerService.UpdateUserAsync(model.ProfileToUpdate.UserInfo.Id, model.ProfileToUpdate);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Authorize(Utility.Permissions.User.Edit)]
        public async Task<IActionResult> UpdateUserRole(int userId,string role)
    {
        try
        {
            // Add user to new role
            await _userManagerService.UpdateUserRoleAsync(userId, role);

            return RedirectToAction("Index", "Home");
        }
        catch
        {
            return BadRequest(ModelState);
        }
    }
        [Authorize(Utility.Permissions.User.Remove)]

        [HttpGet]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                // Call the service method to delete the user
                await _userManagerService.DeleteUserAsync(userId);

                // Redirect to the Index page or any other desired page
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return RedirectToAction("Index");
            }
        }


        [Authorize(Utility.Permissions.User.Edit)]

        [HttpPost]
        public async Task<IActionResult> ToggleUserLock(int userId, DateTime? endDate)
        {
            try
            {
                var success = await _userManagerService.ToggleLockUser(userId, endDate);
            }
            catch
            {
                return BadRequest();
            }
            return RedirectToAction("Index");

        }


    }
}
