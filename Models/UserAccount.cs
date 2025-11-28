using System.ComponentModel.DataAnnotations;

namespace SignalR.Models
{
    public class UserAccount
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
