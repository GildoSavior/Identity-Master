using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}