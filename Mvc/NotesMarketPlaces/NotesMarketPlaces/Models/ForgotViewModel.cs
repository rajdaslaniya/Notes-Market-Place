using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.Models
{
    public class ForgotViewModel
    {
        [Required(ErrorMessage ="This field is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        
    }
}