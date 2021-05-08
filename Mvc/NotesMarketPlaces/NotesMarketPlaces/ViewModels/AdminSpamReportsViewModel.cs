using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlaces.ViewModels
{
    public class AdminSpamReportsViewModel
    {
        public int SpamID { get; set; }
        public int NoteID { get; set; }
        public string ReportedBy { get; set; }
        public string NoteTitle { get; set; }
        public string Category { get; set; }
        public DateTime? DateEdited { get; set; }
        public string Remark { get; set; }
    }
}