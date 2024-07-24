using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
    public class UserManagementVM
    {
        public List<ApplicationUser> Users { get; set; }
        public SelectList Roles { get; set; }
        public ProfileVM ProfileToUpdate { get; set; }
        public RegisterVM RegisterVM { get; set; }

    }

}
