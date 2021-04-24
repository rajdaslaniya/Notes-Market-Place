using NotesMarketPlaces.Models;
using NotesMarketPlaces.Send_Mail;
using System;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using NotesMarketPlaces.ViewModels;
using System.Collections.Generic;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Notes")]
    public class NotesController : Controller
    {
        readonly NotesMarketPlaceEntities1 _dbcontext = new NotesMarketPlaceEntities1();
        
       [Route("Details/{id}")]
        [Authorize(Roles = "Member")]
        public ActionResult Details(int id)
        {
            ViewBag.SearchNotes = "active";

            //Check if user is logged in or not
            var users = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Find Note details in data base
            var NoteDetail = _dbcontext.SellerNotes.Where(x => x.ID == id).FirstOrDefault();

            if (NoteDetail == null)
            {
                return HttpNotFound();
            }

            //Find Note attachment in database
            var NoteAttachment = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == NoteDetail.ID).FirstOrDefault();

            //Find Seller of that note
            var seller = _dbcontext.Users.Where(x=>x.ID==NoteDetail.SellerID).FirstOrDefault();

            
            
            // join user and userprofile for Review
            IEnumerable<ReviewsViewModel> review =   from rw in _dbcontext.SellerNotesReviews
                                                     join usr in _dbcontext.Users on rw.ReviewedBYID equals usr.ID
                                                     join usrprofile in _dbcontext.UserProfiles on usr.ID equals usrprofile.UserID
                                                     where rw.NoteID==id && rw.IsActive==true orderby rw.Rating descending
                                                     select new ReviewsViewModel { TblSellerNotesReview=rw,TblUser=usr,TblUserProfile=usrprofile};


            //Get reviews
            var reviewer = _dbcontext.SellerNotesReviews.Where(x => x.NoteID == NoteDetail.ID).Select(x => x.Rating);
            //Count Total reviews
            var count = reviewer.Count();
            //Get Average Rating
            var avgrating = count > 0 ? Math.Ceiling(reviewer.Average()) : 0;
            //Find a Total Spam Records
            var spam = _dbcontext.SellerNotesReportedIssues.Where(x => x.NoteID == NoteDetail.ID).ToList();

            

            //Create object of DetailsViewModel
            DetailsViewModel detailsView = new DetailsViewModel();
            detailsView.sellernotes = NoteDetail;
            detailsView.Seller = seller.FirstName + " " + seller.LastName;
            detailsView.NotesReview = review;
            detailsView.TotalReview = count;
            detailsView.AverageRating = (int?)avgrating;
            detailsView.TotalSpamReport = spam.Count();
            detailsView.UserID = users.ID;

            if (User.Identity.IsAuthenticated)
            {
                //Find Note is Request for download
                var downloadRequest = _dbcontext.Downloads.Where(x => x.NoteID == id &&x.Downloader==users.ID &&x.IsPaid == true && x.IsSellerHasAllowedDownload==false).FirstOrDefault();
                //Find Note is Allow download or not
                var allowDownload = _dbcontext.Downloads.Where(x => x.NoteID == NoteDetail.ID&& x.Downloader==users.ID&& x.IsPaid == true && x.IsSellerHasAllowedDownload == true).FirstOrDefault();
                
                if(downloadRequest==null && allowDownload == null)
                {
                    detailsView.NoteRequested = false;
                }
                else
                {
                    detailsView.NoteRequested = true;
                }
                if(downloadRequest==null && allowDownload != null)
                {
                    detailsView.AllowDownload = true;
                }
                else
                {
                    detailsView.AllowDownload = false;
                }
                
            }
            
            return View(detailsView);
        }

        [Route("SearchNotes")]
        [HttpGet]
        public ActionResult SearchNotes(string search, string ratings,string type,string course,string country,string category,string university, int page=1)
        {
            ViewBag.SearchNotes = "active";

            ViewBag.NoteSearch = search;
            ViewBag.Category = category;
            ViewBag.Type = type;
            ViewBag.University = university;
            ViewBag.Course = course;
            ViewBag.Country = country;
            ViewBag.Ratings = ratings;

            //Dropdownlist with Viewbag
            ViewBag.TypeList = _dbcontext.NotesTypes.ToList();
            ViewBag.CategoryList = _dbcontext.NotesCategories.ToList();
            ViewBag.CountryList = _dbcontext.Countries.ToList();
            ViewBag.UniversityList = _dbcontext.SellerNotes.Where(x => x.IsActive == true && x.UniversityName != null && x.Status == 9).ToList();
            ViewBag.CourseList = _dbcontext.SellerNotes.Where(x => x.IsActive == true && x.Course != null && x.Status == 9).Select(x=>x.Course).Distinct();
            ViewBag.RatingList = new List<SelectListItem> { new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text="5",Value="5"} };

            //To find in data base Total no of notes is published
            var noteList = _dbcontext.SellerNotes.Where(x=>x.Status==9).ToList();

            if (!string.IsNullOrEmpty(search)|| !string.IsNullOrEmpty(type)||
                !string.IsNullOrEmpty(category)|| !string.IsNullOrEmpty(university)||
                !string.IsNullOrEmpty(course)|| !string.IsNullOrEmpty(country))
            {
                search = search.ToString().ToLower();
                type = type.ToString().ToLower();
                category = category.ToString().ToLower();
                university = university.ToString().ToLower();
                course = course.ToString().ToLower();
                country = country.ToString().ToLower();
                noteList = noteList.Where(x => x.Title.ToLower().Contains(search)&&
                                                x.NotesType.ToString().ToLower().Contains(type)&&
                                                x.Category.ToString().ToLower().Contains(category)&&
                                                x.UniversityName.ToLower().Contains(university)&&
                                                x.Course.ToLower().Contains(course)&&
                                                x.Country.ToString().ToLower().Contains(country)).ToList();

            }
            
            

            List<SearchNotesViewModel> searchNotes = new List<SearchNotesViewModel>();
            if (String.IsNullOrEmpty(ratings))
            {
                foreach (var item in noteList)
                {
                    //Get reviews
                    var review = _dbcontext.SellerNotesReviews.Where(x => x.NoteID == item.ID).Select(x => x.Rating);
                    //Count Total reviews
                    var count = review.Count();
                    //Get Average Rating
                    var avgrating = count > 0 ?Math.Ceiling(review.Average()) : 0;
                    //Find a Total Spam Records
                    var spam = _dbcontext.SellerNotesReportedIssues.Where(x => x.NoteID == item.ID).ToList();

                    SearchNotesViewModel note = new SearchNotesViewModel
                    {
                        note = item,
                        TotalRating = count,
                        AverageRating = Convert.ToInt32(avgrating),
                        TotalSpam = spam.Count()
                    };
                    searchNotes.Add(note);
                }
            }
            else
            {
                foreach (var item in noteList)
                {
                    //Get reviews
                    var review = _dbcontext.SellerNotesReviews.Where(x => x.NoteID == item.ID).Select(x => x.Rating);
                    //Count Total reviews
                    var count = review.Count();
                    //Get Average Rating
                    var avgrating = count > 0 ? Math.Ceiling(review.Average()) : 0;
                    //Find a Total Spam Records
                    var spam = _dbcontext.SellerNotesReportedIssues.Where(x => x.NoteID == item.ID).ToList();
                    if (avgrating == Convert.ToInt32(ratings))
                    {
                        SearchNotesViewModel note = new SearchNotesViewModel
                        {
                            note = item,
                            TotalRating = count,
                            AverageRating = Convert.ToInt32(avgrating),
                            TotalSpam = spam.Count()
                        };
                        searchNotes.Add(note);
                    }
                }
            }

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = Math.Ceiling(noteList.Count() / 9.0);
            //Skip next or Previous
            // show record according to pagination
            IEnumerable<SearchNotesViewModel> result = searchNotes.AsEnumerable().Skip((page - 1) * 9).Take(9);
            // total result count
            ViewBag.ResultCount = searchNotes.Count();

            return View(searchNotes);
        }


        [Authorize(Roles = "Member")]
        [Route("DownloadNote/{noteid}")]
        public ActionResult DownloadNote(int noteid)
        {
            //To Find Note
            var note = _dbcontext.SellerNotes.Find(noteid);
            if (note == null)
            {
                return HttpNotFound();
            }
            

            //Find Attachment in Database of this note
            var attachment = _dbcontext.SellerNotesAttachements.Where(x=>x.NoteID==note.ID).FirstOrDefault();

            //Get Logged in user
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            string path;

            //Note's seller if loggedin user's id same it means user wants to download his own notes
            //Then we need to provide a download note without entry in download
            if (note.SellerID == user.ID)
            {
                //Get Attachment path
                path = Server.MapPath(attachment.FilePath);
                DirectoryInfo dir = new DirectoryInfo(path);
                //Create Zip of attachment
                using (var memoryStream =new MemoryStream())
                {
                    using (var ziparchive=new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
                    {
                        foreach(var item in dir.GetFiles())
                        {
                            //filepath is attachment path+file name
                            string filepath = path + item.ToString();
                            ziparchive.CreateEntryFromFile(filepath,item.ToString());
                        }
                    }
                    //Return Zip File
                    return File(memoryStream.ToArray(), "application/zip", note.Title + ".Zip");
                }
            }



            //Note is free to Download
            if (note.IsPaid==false)
            {
                //To check if user download note is already or not
                var downloadfreenote = _dbcontext.Downloads.Where(x=>x.NoteID==noteid && x.CreatedBy==user.ID && x.IsSellerHasAllowedDownload==true && x.AttachmentPath!=null).FirstOrDefault();
                if (downloadfreenote == null) {
                    //Create download Object
                    Download download = new Download
                    {
                        NoteID = note.ID,
                        Seller = note.SellerID,
                        Downloader = user.ID,
                        IsSellerHasAllowedDownload = true,
                        AttachmentPath = attachment.FilePath,
                        IsAttachmentDownloaded=true,
                        IsPaid = false,
                        PurchasedPrice = note.SellingPrice,
                        AttachmentDownloadedDate = DateTime.Now,
                        NoteTitle = note.Title,
                        NoteCategory = note.NotesCategory.Name,
                        CreatedDate = DateTime.Now,
                        CreatedBy = user.ID
                    };

                    //Add Object in download table
                    _dbcontext.Downloads.Add(download);
                    //Save changes
                    _dbcontext.SaveChanges();

                    //Path is assign
                    path = Server.MapPath(download.AttachmentPath);
                }
                else
                {
                    //User is download again not enter in again download table
                    path = Server.MapPath(downloadfreenote.AttachmentPath);
                }

                //To Create a zip file
                DirectoryInfo dir = new DirectoryInfo(path);
                using (var memoryStream=new MemoryStream())
                {
                    using (var ziparchive=new ZipArchive(memoryStream,ZipArchiveMode.Create,true))
                    {
                        foreach (var  item in dir.GetFiles())
                        {
                            string fullpath = path + item.ToString();
                            ziparchive.CreateEntryFromFile(fullpath,item.ToString());
                        }
                    }
                    return File(memoryStream.ToArray(), "application/zip", note.Title+".zip");
                }
            }
            //If note is paid
            else
            {
                // get download object
                var downloadpaidnote = _dbcontext.Downloads.Where(x => x.NoteID == noteid && x.CreatedBy == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).FirstOrDefault();

                //If User is already download note
                if (downloadpaidnote != null)
                {
                    // if user is download note first time then we need to update following record in download table 
                    if (downloadpaidnote.IsAttachmentDownloaded == false)
                    {
                        downloadpaidnote.AttachmentDownloadedDate = DateTime.Now;
                        downloadpaidnote.IsAttachmentDownloaded = true;
                        downloadpaidnote.ModifiedDate = DateTime.Now;
                        downloadpaidnote.ModifiedBy = user.ID;

                        // update ans save data in database
                        _dbcontext.Entry(downloadpaidnote).State = EntityState.Modified;
                        _dbcontext.SaveChanges();
                    }

                    // get attachement path
                    path = Server.MapPath(downloadpaidnote.AttachmentPath);

                    DirectoryInfo dir = new DirectoryInfo(path);

                    // create zip of attachement
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var item in dir.GetFiles())
                            {
                                // file path is attachement path + file name
                                string filepath = path + item.ToString();
                                ziparchive.CreateEntryFromFile(filepath, item.ToString());
                            }
                        }
                        // return zip
                        return File(memoryStream.ToArray(), "application/zip", note.Title + ".zip");
                    }
                }
            }

            return RedirectToAction("Notes","Notes",new { id=noteid});
        }

        [Authorize(Roles = "Member")]
        [Route("Request/{noteid}")]
        public ActionResult RequestPaidNotes(int noteid)
        {
            //Get Note Details
            var note = _dbcontext.SellerNotes.Find(noteid);

            //Check User Logged in
            var users = _dbcontext.Users.Where(x => x.EmailID== User.Identity.Name).FirstOrDefault();

            //Find Attachment in Database of this note
            var attachment = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == note.ID).FirstOrDefault();

            //Enter Data in download table
            Download download = new Download {
                NoteID = note.ID,
                Seller = note.SellerID,
                Downloader = users.ID,
                IsSellerHasAllowedDownload = false,
                IsAttachmentDownloaded=false,
                IsPaid=note.IsPaid,
                PurchasedPrice=note.SellingPrice,
                AttachmentDownloadedDate=DateTime.Now,
                NoteTitle=note.Title,
                NoteCategory=note.NotesCategory.Name,
                CreatedDate=DateTime.Now,
                CreatedBy=users.ID
                
            };

            //Save changes in database
            _dbcontext.Downloads.Add(download);
            _dbcontext.SaveChanges();

            //Send Mail
            BuildEmailTemplateForBuyerRequest(download,users);


            return RedirectToAction("Details",new { id=note.ID});
        }

        private void BuildEmailTemplateForBuyerRequest(Download download,User user)
        {
            //Find Name from seller id 
            var seller = _dbcontext.Users.Where(x => x.ID == download.Seller).FirstOrDefault();
            //Read All Context from Email Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "BuyerRequest" + ".cshtml");

            //SellerName Replace in Template
            body = body.Replace("ViewBag.SellerName",seller.FirstName);
            //Buyer Name Replace in Template
            body = body.Replace("ViewBag.BuyerName", user.FirstName);
            body = body.ToString();
            //Get Support Email
            var fromemail = _dbcontext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();
            //Mail From 
            string from, subject, to;
            from = fromemail.Value.Trim();
            subject = user.FirstName+" wants to purchase your notes";
            to=seller.EmailID.Trim();

            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            //Create a mail
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SendingMail.SendEmail(mail);
        }
    }
}