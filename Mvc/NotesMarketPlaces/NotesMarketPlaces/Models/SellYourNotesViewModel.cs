using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.Models
{
    public class SellYourNotesViewModel
    {
        public IEnumerable<SellerNote> InProressNotes { get; set; }
        public IEnumerable<SellerNote> PublishedNotes { get; set; }
        public int MyDownload { get; set; }
        public int NumberOfSoldNotes { get; set; }
        public int MoneyEarned { get; set; }
        public int MyRejectedNotes { get; set; }
        public int BuyerRequestNotes { get; set; }
    }
}