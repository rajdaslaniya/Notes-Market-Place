using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class ForgotViewModel
    {
        [Required(ErrorMessage ="This field is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        
    }
}