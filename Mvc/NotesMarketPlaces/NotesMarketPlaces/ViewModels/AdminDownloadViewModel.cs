using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AdminDownloadViewModel
    {
        public int NoteID { get; set; }
        public int BuyerID { get; set; }
        public int SellerID { get; set; }
        public string NoteTitle { get; set; }
        public string Category { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public string SellType { get; set; }
        public decimal? Price { get; set; }
        public DateTime? DownloadDate { get; set; }
    }
}