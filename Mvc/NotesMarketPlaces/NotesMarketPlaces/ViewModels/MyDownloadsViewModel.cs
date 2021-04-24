using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class MyDownloadsViewModel
    {
        public int? UserID { get; set; }
        public int? NoteID { get; set; }
        public int? DownloadID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Buyer { get; set; }
        public string SellType { get; set; }
        public int Price { get; set; }
        public DateTime? DownloadDate { get; set; }
        public int? ReviewID { get; set; }
        public int? Ratings { get; set; }
        public string Comments { get; set; }
        
    }
}