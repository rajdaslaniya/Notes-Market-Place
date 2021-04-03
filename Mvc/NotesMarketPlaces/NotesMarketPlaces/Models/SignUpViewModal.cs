﻿using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace NotesMarketPlaces.Models
{
    public class SignUpViewModal
    {
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50,ErrorMessage ="Name is too Long")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50, ErrorMessage = "Name is too Long")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Use valid email address")]
        public string EmailID { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"((?=.*\d)(?=.*[A-Z])(?=.*\W).{8,24})", ErrorMessage ="Password must be between 8 and 24 characters and contain one uppercase letter, one lowecase letter,one digit and one special symbol")]
        public string Password { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}