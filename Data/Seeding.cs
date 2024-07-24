using Microsoft.AspNetCore.Identity;
using OtlobLap.Auth;
using OtlobLap.Models;
using OtlobLap.Services;
using System.Security.Claims;
using static OtlobLap.Data.Utility;

namespace OtlobLap.Data
{
    public static class Seeding
    {
        public static async Task SeedUserAdminAsync(IImageService imageService,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole<int>> roleManager)
        {
            var adminData = new ApplicationUser
            {
                Name = "Mohamed Ahmed",
                Email = "Admin@gmail.com",
                UserName = "Admin@gmail.com",
                Role = Roles.Admin.ToString(),
            };

            try
            {
          
                // Check if the admin user exists, if not, create it
                if (await userManager.FindByEmailAsync(adminData.Email) == null)
                {
                    adminData.UserImage = await imageService.UploadToDb(imageService.GetFormFileFromImagePath("~/Images/Users/Admin.png"));

                    var result = await userManager.CreateAsync(adminData, "11111111");

                    if (result.Succeeded)
                    {
                        // Add the admin user to the Admin role
                        await userManager.AddToRoleAsync(adminData, Roles.Admin.ToString());
                        await roleManager.SeedClaimsAsync();
                    }
                    else
                    {
                        // User creation failed, handle errors
                        foreach (var error in result.Errors)
                        {
                            // Handle each error, for example:
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public static async Task SeedRolesAsync(this RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
        public static async Task SeedClaimsAsync(this RoleManager<IdentityRole<int>> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin.ToString());

            if (adminRole is null)
            {
                throw new Exception("Admin role not found.");
            }

            var modules = Enum.GetNames<Modules>();
            foreach (var module in modules)
            {
                await roleManager.AddPermissionClaims(adminRole,module);
            }
        }
        public static async Task AddPermissionClaims (this RoleManager<IdentityRole<int>> roleManager , IdentityRole<int> role , string module )
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var permissions =  PermissionsAuth.GetPermissions(module);
            foreach (var permission in permissions)
            {
                if (!allClaims.Any(c =>c.Type == "Permission" && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }

            }
        }
      

    }
}
