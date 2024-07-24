using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class ProfileVM
    {
        public ApplicationUser? UserInfo { get; set; }
        public string? Biography;
        public ChangePasswordVM? ChangePassword { get; set; }
        public IFormFile? file { get; set; }

    }
}
