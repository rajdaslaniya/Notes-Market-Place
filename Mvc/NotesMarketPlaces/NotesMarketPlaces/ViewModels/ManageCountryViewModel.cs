using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class ManageCountryViewModel
    {
        public int ID { get; set; }
        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string AddedBy { get; set; }
        public string Active { get; set; }
    }
}