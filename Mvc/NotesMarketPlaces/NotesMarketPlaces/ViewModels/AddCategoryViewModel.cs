using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AddCategoryViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "This is required")]
        [MaxLength(100, ErrorMessage = "This Name is too long")]
        public string Category { get; set; }

        [Required(ErrorMessage = "This is required")]
        public string Description { get; set; }
    }
}