using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class UnderReviewViewModel
    {
        public int? NoteID { get; set; }
        public int? UserID { get; set; }
        public int? SellerID { get; set; }
        public string NoteTitle { get; set; }
        public string NoteCategory { get; set; }
        public string Seller { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Status { get; set; }
    }

}