using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class AddAdministrativeViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage ="This field is required")]
        [MaxLength(50,ErrorMessage ="This name is too long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50, ErrorMessage = "This name is too long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(30, ErrorMessage = "This name is too long")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(20, ErrorMessage = "Number is not valid")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "Only Numbers allowed")]
        public string PhoneCode { get; set; }
        public IEnumerable<Country> PhoneCodeList { get; set; }
    }
}