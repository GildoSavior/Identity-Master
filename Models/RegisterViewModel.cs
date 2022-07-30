using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Erro", MinimumLength = 6),]
        [DataType(DataType.Password)]
        [Display(Name = "PassWord")]
        public String Password { get; set; }

        [Compare("Password", ErrorMessage = "Incompatible"),]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm PassWord")]
        public String ConfirmPassword { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}