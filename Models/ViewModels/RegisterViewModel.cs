using System.ComponentModel.DataAnnotations;

namespace SignalR.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters")]
        [Display(Name = "User name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
