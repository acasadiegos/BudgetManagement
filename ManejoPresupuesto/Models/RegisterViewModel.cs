using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="The field {0} is required")]
        [EmailAddress(ErrorMessage = "The field must be a valid email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
