using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage ="This field is required")]
        [DisplayName("Old Password*")]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,24})", ErrorMessage = "Password must be between 8 and 24 characters and contain one uppercase letter, one lowecase letter,one digit and one special symbol")]
        [DisplayName("New Password*")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Password doesn't match.")]
        [DisplayName("Confirm Password*")]
        public string ConfirmPassword { get; set; }
    }
}