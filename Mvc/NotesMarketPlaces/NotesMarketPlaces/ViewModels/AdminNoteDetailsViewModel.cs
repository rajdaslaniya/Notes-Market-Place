using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class AdminNoteDetailsViewModel
    {
        public int? UserID { get; set; }
        public int? SellerID { get; set; }
        public int? AverageRating { get; set; }
        public int? TotalSpamReport { get; set; }
        public int? TotalRate { get; set; }
        public SellerNote sellerNote { get; set; }
        public IEnumerable<UserReviewViewModel> NoteReview{get;set;}
    }

    public class UserReviewViewModel
    {
        public  User tblUser{get;set;}
        public UserProfile tblUserProfile { get; set; }
        public SellerNotesReview tblReview { get; set; }
    }

}