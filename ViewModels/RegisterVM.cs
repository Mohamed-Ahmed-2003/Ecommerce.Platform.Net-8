using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtlobLap.ViewModels
{
    [NotMapped]
    public class RegisterVM
    {
        public required string FullName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        [Phone]
        public  string? Phone { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
        public IFormFile? UserImageFile { get; set; }
        public string? Role { get; set; }
    }

}
