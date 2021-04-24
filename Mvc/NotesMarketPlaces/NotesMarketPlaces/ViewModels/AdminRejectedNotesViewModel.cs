using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AdminRejectedNotesViewModel
    {
        public int NoteID { get; set; }
        public int SellerID { get; set; }
        public string Seller { get; set; }
        public string NoteTitle { get; set; }
        public string Category { get; set; }
        public string RejectedBy { get; set; }
        public string Remark { get; set; }
        public DateTime? DateEdited { get; set; }
    }
}