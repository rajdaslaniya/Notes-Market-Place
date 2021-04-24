using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NotesMarketPlaces.Models;

namespace NotesMarketPlaces.ViewModels
{
    public class SearchNotesViewModel
    {
        public SellerNote note { get; set; }
        public int AverageRating { get; set; }
        public int TotalSpam { get; set; }
        public int TotalRating { get; set; }
    }
}