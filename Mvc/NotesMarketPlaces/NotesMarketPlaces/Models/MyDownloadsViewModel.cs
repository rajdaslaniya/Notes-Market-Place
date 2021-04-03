using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.Models
{
    public class MyDownloadsViewModel
    {
        public Download tblDownload { get; set; }
        public User tblUser { get; set; }
        public SellerNotesReview tblReview { get; set; }
    }
}