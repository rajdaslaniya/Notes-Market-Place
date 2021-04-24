using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class SellYourNotesViewModel
    {
        public IEnumerable<InProgressNote> InProressNotes { get; set; }
        public IEnumerable<PublishedNote> PublishedNotes { get; set; }
        public int? MyDownload { get; set; }
        public int? NumberOfSoldNotes { get; set; }
        public int? MoneyEarned { get; set; }
        public int? MyRejectedNotes { get; set; }
        public int? BuyerRequestNotes { get; set; }
    }
    public class InProgressNote
    {
        public int NoteID { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
    }
    public class PublishedNote
    {
        public int NoteID { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SellType { get; set; }
        public decimal? Price { get; set; }
    }
}