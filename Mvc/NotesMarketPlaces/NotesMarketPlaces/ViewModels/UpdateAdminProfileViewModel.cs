using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class UpdateAdminProfileViewModel
    {
        [Required(ErrorMessage ="This field is required")]
        [MaxLength(50,ErrorMessage ="Name size is Too long")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50, ErrorMessage = "Name size is Too long")]
        public string LastName { get; set; }

        public string Email { get; set; }
        
        [MaxLength(50, ErrorMessage = "Name size is Too long")]
        public string SecondryEmail { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "Only Numbers allowed")]
        public string PhoneNumber { get; set; }
        public string PhoneCode { get; set; }
        public string Picture { get; set; }
        public HttpPostedFileBase ProfilePicture { get; set; }
        public IEnumerable<Country> PhoneCodeList { get; set; }
    }
}