using System.ComponentModel.DataAnnotations;

namespace OtlobLap.ViewModels
{
    public class ChangePasswordVM
    {
        [DataType(DataType.Password)]
        [MinLength(8)]
        public  required string CurrentPassword { get; set; }
        
        [DataType(DataType.Password)]
        [MinLength(8)]
        public  required string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public  required string ConfirmPassword { get; set; }
    }
}
