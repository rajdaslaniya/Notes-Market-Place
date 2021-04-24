using NotesMarketPlaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class MySoldNotesViewModel
    {
        public int DownloadID { get; set; }
        public int NoteID { get; set; }
        public string NoteTitle { get; set; }
        public string SellType { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public DateTime? DownloadedDate { get; set; }
        public string Buyer { get; set; }
    }
}