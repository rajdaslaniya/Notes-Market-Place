using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class ManageAdministratorViewModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Active { get; set; }
    }
}