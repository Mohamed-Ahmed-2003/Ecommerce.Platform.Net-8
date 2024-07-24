using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Controllers
{
    public class AccountController 
        (IUserManagerService userManagerService,
        IProfileService profileService
        ) : BaseController(profileService)
    {
        private readonly IUserManagerService _userManagerService = userManagerService;
        private readonly IProfileService _profileService = profileService;
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var res = await _userManagerService.RegisterAsync(model);
                if (res.Succeeded)
                {
                     
                      return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                if ((await _userManagerService.SignInAsync(model)).Succeeded)
                {
                    var user = await _userManagerService.GetUserAsync(User);
                    if (user != null)
                    {
                        CookieOptions options = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7),
                            Secure = true,
                        };
                        Response.Cookies.Append("UserName", user.Name,options);
                        Response.Cookies.Append("UserId", user.Id.ToString(), options);

                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewData["ErrorMessage"] = "Email or password is incorrect!";
            }
            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
			foreach (var cookie in Request.Cookies.Keys)
			{
				Response.Cookies.Delete(cookie);
			}
			await _userManagerService.LogOut();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Settings()
        {

            if (User.Identity?.IsAuthenticated ?? false)
            {

                var user = await _profileService.GetUserAsync();
                ProfileVM model = new ProfileVM
                {
                    UserInfo = user,
                 };
                return View(model);
            }
            return RedirectToAction("Login");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUserInfo(ProfileVM profile )
        {
 
            await _userManagerService.UpdateUserAsync((int) ViewBag.UserId  , profile );

            return RedirectToAction("Settings");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ProfileVM profile)
        {
            if (profile.ChangePassword != null)
            {
                await _userManagerService.ChangePasswordAsync((int) ViewBag.UserId, profile.ChangePassword);
          }
            return RedirectToAction("Settings");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
