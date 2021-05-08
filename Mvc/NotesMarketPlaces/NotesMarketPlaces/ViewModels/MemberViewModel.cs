using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class MemberViewModel
    {
        public int? RegisterID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? NotesUnderReview { get; set; }
        public int? PublishedNotes { get; set; }
        public int? DownloadedNote { get; set; }
        public decimal? TotalExpensis { get; set; }
        public decimal? TotalEarning { get; set; }
    }
}