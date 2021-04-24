using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50,ErrorMessage ="Name should contain max 50 character")]
        [DisplayName("Full Name*")]
        public string FullName { get; set; }


        [Required(ErrorMessage ="This field is required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(30, ErrorMessage = "Email should contain max 50 character")]
        [DisplayName("Email*")]
        public string EmailID { get; set; }


        
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(100, ErrorMessage = "Subject should contain max 50 character")]
        [DisplayName("Subject*")]
        public string Subject { get; set; }



        [Required(ErrorMessage = "This field is required")]
        [DisplayName("Comments*")]
        public string Comments { get; set; }
    }
}