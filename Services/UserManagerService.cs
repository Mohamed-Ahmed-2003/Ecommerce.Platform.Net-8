using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace OtlobLap.Services
{
    public interface IUserManagerService
    {
        public Task<IdentityResult> RegisterAsync(RegisterVM model);
        public Task<SignInResult> SignInAsync(LoginVM model);
        public Task ChangePasswordAsync(int user, ChangePasswordVM profile);
        public Task<ApplicationUser>  GetUserAsync(ClaimsPrincipal User);
	    public Task UpdateUserAsync(int userId,ProfileVM profile);
	    public Task UpdateUserRoleAsync(int userId, string role);
        public  Task DeleteUserAsync(int userId);
        public  Task<IdentityResult> AddUserAsync(ApplicationUser user);
        public Task LogOut();
        public Task<List<ApplicationUser>> GetApplicationUsers();
        public Task<List<string>> GetRolesAsync();
        public Task<bool> ToggleLockUser(int userId, DateTime? endDate);
    }
    public class UserManagerService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IImageService imageService,
        AppDbContext context
        )
        : IUserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IImageService _imageService= imageService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly AppDbContext _context = context;
        private readonly RoleManager<IdentityRole<int>> _roleManager = roleManager;


		public async Task<IdentityResult> RegisterAsync(RegisterVM model)
        {

            var user = new ApplicationUser
            {
                Name = model.FullName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Role = string.IsNullOrWhiteSpace(model.Role) ? Utility.Roles.Customer.ToString() : model.Role
            };

            if (model.UserImageFile != null)
            {
                user.UserImage = await _imageService.UploadToDb(model.UserImageFile);
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result; // Return failure result if user creation failed
            }
           
                var addToRoleResult = await _userManager.AddToRoleAsync(user, user.Role.ToString());
                if (!addToRoleResult.Succeeded)
                {
                    // Rollback user creation if adding to role fails
                    await _userManager.DeleteAsync(user);
                    return addToRoleResult; // Return failure result
                }
            
            
            return IdentityResult.Success;
        }


        public async Task<SignInResult> SignInAsync(LoginVM model)
        {
           return  (await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true));

        }
        public async Task LogOut()
        {

             await (_signInManager.SignOutAsync());
        }
        public async Task ChangePasswordAsync(int userId, ChangePasswordVM model)
        {
            var currentUser = await _userManager.FindByIdAsync(userId.ToString());

            if (currentUser != null )
            {
                 await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);
            }
            
        }

        public async Task UpdateUserAsync(int userId, ProfileVM profile)
        {
            try
            {
                var targetUser = await _userManager.FindByIdAsync(userId.ToString());

                if (targetUser == null || profile is null) return;

                if (profile.UserInfo != null)
                {
                    targetUser.Name = profile.UserInfo.Name;
                    targetUser.Email = profile.UserInfo.Email;
                }

                if (profile.file != null)
                {
                    // Upload user image
                    targetUser.UserImage = await _imageService.UploadToDb(profile.file);
                }

                await _userManager.UpdateAsync(targetUser);
            }
            catch
            {
                throw new Exception();
            }
        }



        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null) { 

            await _userManager.DeleteAsync(user);
        }
          
       
        }

        public async Task<IdentityResult> AddUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            
            return result;
        }


        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User)
        {
            return await _userManager.GetUserAsync(User);
        }

		public async Task<List<ApplicationUser>> GetApplicationUsers()
		{
            var users = await _context.Users.ToListAsync();
            return users;
		}

        public async Task UpdateUserRoleAsync(int userId, string role)
        {
            try
            {
                var targetUser = await _userManager.FindByIdAsync(userId.ToString());

                if (targetUser == null)
                {
                    // Log or handle the case where the user is not found
                    return;
                }

                // Check if the user already has the target role
                var currentRoles = await _userManager.GetRolesAsync(targetUser);
            
           
               await _userManager.RemoveFromRolesAsync(targetUser, currentRoles);
                
                // Add the user to the new role
                await _userManager.AddToRoleAsync(targetUser, role);
                targetUser.Role = role;
                // Update the user
                await _userManager.UpdateAsync(targetUser);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error updating user role: {ex.Message}");
            }
        }


        public async Task<List<string>> GetRolesAsync()
        {
            return await _roleManager.Roles.Select(r=>r.Name).ToListAsync();
        }
        private async Task<bool> LockUser(ApplicationUser user, DateTime? endDate)
        {
            try
            {
                if (endDate == null)
                    endDate = DateTime.MaxValue;


                if (user == null)
                    return false;

                var lockResult = await _userManager.SetLockoutEnabledAsync(user, true);
                var lockDateResult = await _userManager.SetLockoutEndDateAsync(user, endDate);

                if( lockResult.Succeeded && lockDateResult.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, user.Role);
                    await _userManager.AddToRoleAsync(user, Utility.Roles.Blocked.ToString());

                    return true;
                }
            }
            catch 
            {
                // Handle or log the exception
                return false;
            }
            return false;
        }

        private async Task<bool> UnlockUser(ApplicationUser user)
        {
            try
            {
                if (user == null)
                    return false;

                var setLockoutEndDateResult = await _userManager.SetLockoutEndDateAsync(user, null); // Unlocks the user
                var lockDisabledResult = await _userManager.SetLockoutEnabledAsync(user, false);


                if (setLockoutEndDateResult.Succeeded && lockDisabledResult.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, Utility.Roles.Blocked.ToString());
                    await _userManager.AddToRoleAsync(user, user.Role);

                    return true;
                }
            }
            catch 
            {
                // Handle or log the exception
                return false;
            }
            return false;
        }

        public async Task<bool> ToggleLockUser(int userId, DateTime? endDate)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                    return false;

                var currentState = await _userManager.IsLockedOutAsync(user);
                if (currentState)
                    return await UnlockUser(user);
                else
                    return await LockUser(user, endDate);
            }
            catch
            {
                return false;
            }
        }
    }
}
