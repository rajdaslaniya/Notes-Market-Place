using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int? NotesReviewPublish { get; set; }
        public int? NotesDownloads { get; set; }
        public int? NewRegistration { get; set; }
        public IEnumerable<PublishedNotes> PublishedList { get; set; }
        public class PublishedNotes
        {
            public int NoteID { get; set; }
            public string Title { get; set; }
            public string Category { get; set; }
            public string FileSizeName { get; set; }
            public string SellType { get; set; }
            public decimal? Price { get; set; }
            public string Publisher { get; set; }
            public DateTime? PublishedDate { get; set; }
            public int NumberOfDownloads { get; set; }

        }
    }
    
}