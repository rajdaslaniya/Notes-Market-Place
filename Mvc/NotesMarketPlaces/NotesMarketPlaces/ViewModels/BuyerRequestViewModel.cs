using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class BuyerRequestViewModel
    {
        public int? NoteID { get; set; }
        public int? UserID { get; set; }
        public int? DownloadID { get; set; }
        public string NoteTitle { get; set; }
        public string Category { get; set; }
        public string Buyer { get; set; }
        public string PhoneNumber { get; set; }
        public string SellType { get; set; }
        public int? Price { get; set; }
        public DateTime? DownloadDate { get; set; }
        
    }
}