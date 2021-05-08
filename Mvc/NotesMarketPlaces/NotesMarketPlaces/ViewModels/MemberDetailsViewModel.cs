using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class MemberDetailsViewModel
    {
        public int RegisterID { get; set; }
        public string DisplayImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string College { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode{get;set;}

        public IEnumerable<Notes> Note { get; set; }
    }

    public class Notes
    {
        public int NoteID { get; set; }
        public string NoteTitle { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int? TotalEarning { get; set; }
        public int? DownloadedNotes { get; set; }
    }
}