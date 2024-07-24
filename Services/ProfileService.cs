using Microsoft.AspNetCore.Identity;
using OtlobLap.Models;


namespace OtlobLap.Services
{
    public interface IProfileService
    {
         Task<ApplicationUser> GetUserAsync ();

    }
    public class ProfileService(UserManager<ApplicationUser> userManager,IHttpContextAccessor httpContextAccessor) : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;



        public async Task<ApplicationUser?> GetUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
                    throw new InvalidOperationException("User is not logged in");
            

            var userData = await _userManager.GetUserAsync(user);

            if (userData is null)
                  throw new InvalidOperationException("User data not found");
            

            return userData;
        }

    }
}
