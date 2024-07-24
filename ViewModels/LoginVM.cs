using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtlobLap.ViewModels
{
    [NotMapped]
    public class LoginVM
    {
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(8)]
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

}
