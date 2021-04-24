using NotesMarketPlaces.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class MyProfileViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DisplayName("First Name *")]
        [MaxLength(50, ErrorMessage = "Name is too Long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DisplayName("Last Name *")]
        [MaxLength(50, ErrorMessage = "Last Name is too Long")]
        public string LastName { get; set; }

        [DisplayName("Email *")]
        [Required(ErrorMessage = "This field is required")]
        public string Email { get; set; }

        [DisplayName("Date of birth")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DOB { get; set; }

        [DisplayName("Gender")]
        public Nullable<int> Gender { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string PhoneCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("[0-9]+", ErrorMessage = "Only Numbers allowed")]
        public string PhoneNumber { get; set; }

        [DisplayName("Profile Picture")]
        public HttpPostedFileBase ProfilePicture { get; set; }

        [DisplayName("Address Line 1 *")]
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(100, ErrorMessage = "Address is too Long")]
        public string AddressLine1 { get; set; }

        [DisplayName("Address Line 2 *")]
        [MaxLength(100, ErrorMessage = "Address is too Long")]
        [Required(ErrorMessage = "This field is required")]
        public string AddressLine2 { get; set; }

        [DisplayName("City *")]
        [Required(ErrorMessage = "This field is required")]
        public string City { get; set; }

        [DisplayName("State *")]
        [Required(ErrorMessage = "This field is required")]
        public string State { get; set; }

        [DisplayName("Zipcode *")]
        [Required(ErrorMessage = "This field is required")]
        public string ZipCode { get; set; }

        [DisplayName("Country *")]
        [Required(ErrorMessage = "This field is required")]
        public string Country { get; set; }

        [DisplayName("University")]
        public string University { get; set; }

        [DisplayName("College")]
        public string College { get; set; }
        public IEnumerable<Country> CountryList { get; set; }
        public IEnumerable<ReferenceData> GenderList { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}