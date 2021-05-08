using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AddCountryViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50, ErrorMessage = "This name is too long")]
        public string CountryName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MaxLength(50, ErrorMessage = "This name is too long")]
        public string Code { get; set; }

        
    }
}