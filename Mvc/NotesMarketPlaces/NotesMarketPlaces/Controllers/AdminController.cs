using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using NotesMarketPlaces.SendMail;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    [OutputCache(Duration = 0)]
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Dashboard")]
        public ActionResult Dashboard(string dashboardsearch,string monthsearch, string sortorder, string sortby, int page = 1)
        {
            ViewBag.Dashboard = "active";

            ViewBag.DashboardSearch = dashboardsearch;
            ViewBag.SoryBy = sortby;
            ViewBag.SortOrder = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.Month = monthsearch;
            //Find a user is logged in or not
            var users = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name && x.RoleID==1 ||x.RoleID==2).FirstOrDefault();

           

            //Create a list of Object Published Notes
            var notelist =new List<AdminDashboardViewModel.PublishedNotes>();

            DateTime todayDate = DateTime.Now;
            DateTime lastDate = todayDate.AddDays(-7);

            //Find a published notes
            var note = _dbContext.SellerNotes.Where(x => x.Status == 9 && x.IsActive == true);

            //Find a Total Number for Review for Publish
            int reviewforpublish = _dbContext.SellerNotes.Where(x => x.Status == 7 && x.IsActive == true).Count();
            //To find a last 7 days new notes downloads
            int notesdownloads = _dbContext.Downloads.Where(x=>x.AttachmentDownloadedDate > lastDate).Count();
            //To New registration in last 7 Days
            int newregistration = _dbContext.Users.Where(x => x.RoleID == 3 && x.CreatedDate>lastDate).Count();
            

                foreach(var item in note)
                {
                    //Find a note attachment in Sellernotesattachment
                    var noteattachment = _dbContext.SellerNotesAttachements.Where(x => x.NoteID == item.ID);
                    float filesize = 0;
                    foreach (var attachment in noteattachment)
                    {
                        string filepath = Server.MapPath(attachment.FilePath+attachment.FileName);
                        FileInfo file = new FileInfo(filepath);
                        filesize = +file.Length;
                    }

                //Count how many person download this note
                int downloadNotes = _dbContext.Downloads.Where(x => x.NoteTitle == item.Title).Count();
                    //Find a Name of publisher in user table
                    var publisher = _dbContext.Users.Where(x => x.ID == item.ActionedBy).FirstOrDefault();

                    //Create a object of publishednotes
                    var notedetail = new AdminDashboardViewModel.PublishedNotes();

                    //Assigning a value
                    notedetail.Title = item.Title;
                    notedetail.NoteID = item.ID;
                    notedetail.Price = item.SellingPrice;
                    notedetail.Publisher = publisher.FirstName +" "+ publisher.LastName;
                    notedetail.SellType = item.IsPaid==true?"Paid":"Free";
                    notedetail.Category = item.NotesCategory.Name;
                    notedetail.PublishedDate = item.PublishedDate;
                    notedetail.NumberOfDownloads = downloadNotes;
                    //Check Size of file
                    if ((filesize/1024) > 1024)
                    {
                         var AttachmentSize = (int)filesize/(1024*1024);
                        notedetail.FileSizeName = AttachmentSize+" "+"MB";
                    }
                    else
                    {
                        var AttachmentSize = (int)filesize/1024;
                        notedetail.FileSizeName = AttachmentSize+" "+"Kb";
                    }
                    //Add A item in list
                    notelist.Add(notedetail);
                }
            //Create a object of AdminDashboardViewModel
            AdminDashboardViewModel adminDashboardView = new AdminDashboardViewModel
            {
                PublishedList = notelist.AsEnumerable(),
                NotesReviewPublish = reviewforpublish,
                NewRegistration = newregistration,
                NotesDownloads = notesdownloads
            };
            ViewBag.MonthSearch = new List<SelectListItem> { new SelectListItem { Text="1",Value="1"},
                                                              new SelectListItem { Text="2",Value="2" },
                                                             new SelectListItem { Text="3",Value="3" },
                                                             new SelectListItem { Text="4",Value="4" },
                                                             new SelectListItem { Text="5",Value="5" },
                                                             new SelectListItem { Text="6",Value="6" },
                                                             new SelectListItem { Text="7",Value="7" },
                                                             new SelectListItem { Text="8",Value="8" },
                                                             new SelectListItem { Text="9",Value="9" },
                                                             new SelectListItem { Text="11",Value="11" },
                                                             new SelectListItem { Text="12",Value="12" }};
            if (!string.IsNullOrEmpty(dashboardsearch))
            {
                dashboardsearch = dashboardsearch.ToLower();
                adminDashboardView.PublishedList = notelist.Where(x => x.Title.ToLower().Contains(dashboardsearch)
                                                                       || x.Category.ToLower().Contains(dashboardsearch) ||
                                                                       x.SellType.ToLower().Contains(dashboardsearch) ||
                                                                       x.Price.ToString().ToLower().Contains(dashboardsearch)||
                                                                       x.Publisher.ToLower().Contains(dashboardsearch)||
                                                                       x.PublishedDate.Value.ToString("dd-MM-yyyy,hh:mm").Contains(dashboardsearch)||
                                                                       x.NumberOfDownloads.ToString().Contains(dashboardsearch)||
                                                                       x.FileSizeName.ToString().Contains(dashboardsearch)).AsEnumerable();
            }

            if (!string.IsNullOrEmpty(monthsearch))
            {
                adminDashboardView.PublishedList = notelist.Where(x => x.PublishedDate.Value.ToString("MM").Contains(monthsearch)).AsEnumerable();
            }

            //Sorting and searching 
            //Going for sorting 
           adminDashboardView.PublishedList = SortTableMyDownload(sortorder, sortby, adminDashboardView.PublishedList);

            //Count a total Number of Page
            ViewBag.TotalPages = Math.Ceiling(adminDashboardView.PublishedList.Count() / 5.0);
            //Skip Next or previous
            adminDashboardView.PublishedList=adminDashboardView.PublishedList.Skip((page - 1) * 5).Take(5);
            return View(adminDashboardView);
        }

        private IEnumerable<AdminDashboardViewModel.PublishedNotes> SortTableMyDownload(string sortorder, string sortby, IEnumerable<AdminDashboardViewModel.PublishedNotes> publishedList)
        {
            switch (sortby)
            {
                case "Title":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.Title);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.Title);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.Title);
                                    return publishedList;
                                }
                        }
                    }
                case "AttchmentSize":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.FileSizeName);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.FileSizeName);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.FileSizeName);
                                    return publishedList;
                                }
                        }
                    }
                case "NumberDownloads":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.NumberOfDownloads);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.NumberOfDownloads);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.NumberOfDownloads);
                                    return publishedList;
                                }
                        }
                    }
                case "Category":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.Category);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.Category);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.Category);
                                    return publishedList;
                                }
                        }
                        
                     }
                case "SellType":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.SellType);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.SellType);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.SellType);
                                    return publishedList;
                                }
                        }
                    }
                case "Price":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.Price);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.Price);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.Price);
                                    return publishedList;
                                }
                        }
                    }
                case "Publisher":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.Publisher);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.Publisher);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.Publisher);
                                    return publishedList;
                                }
                        }
                    }
                case "PublishedDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.PublishedDate);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.PublishedDate);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.PublishedDate);
                                    return publishedList;
                                }
                        }
                    }
                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    publishedList = publishedList.OrderBy(x => x.Title);
                                    return publishedList;
                                }
                            case "Desc":
                                {
                                    publishedList = publishedList.OrderByDescending(x => x.Title);
                                    return publishedList;
                                }
                            default:
                                {
                                    publishedList = publishedList.OrderBy(x => x.Title);
                                    return publishedList;
                                }
                        }
                    }
            }
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Notes/{noteid}/Download")]
        public ActionResult AdminDownloadNote(int noteid)
        {
            //To Find Note
            var note = _dbContext.SellerNotes.Find(noteid);
            if (note == null)
            {
                return HttpNotFound();
            }


            //Find Attachment in Database of this note
            var attachment = _dbContext.SellerNotesAttachements.Where(x => x.NoteID == note.ID).FirstOrDefault();

            //Get Logged in user
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Get id of admin and super admin
            int memberid = _dbContext.Users.Where(x => x.ID==user.ID&& (x.RoleID == 1 || x.RoleID == 2)).Select(x => x.ID).FirstOrDefault();
            string path;

            //Download note is user we not enter in data download table
            //Then we need to provide a download note without entry in download
            if (user.ID==memberid)
            {
                //Get Attachment path
                path = Server.MapPath(attachment.FilePath);
                DirectoryInfo dir = new DirectoryInfo(path);
                //Create Zip of attachment
                using (var memoryStream = new MemoryStream())
                {
                    using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in dir.GetFiles())
                        {
                            //filepath is attachment path+file name
                            string filepath = path + item.ToString();
                            ziparchive.CreateEntryFromFile(filepath, item.ToString());
                        }
                    }
                    //Return Zip File
                    return File(memoryStream.ToArray(), "application/zip", note.Title + ".Zip");
                }
            }

            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("UnPublishNote")]
        public ActionResult UnPublish(FormCollection form)
        {
            int noteid = Convert.ToInt32(form["noteid"]);
            string remark = form["Remark"];
            //Find a notes in seller notes
            var note = _dbContext.SellerNotes.Where(x=>x.ID==noteid).FirstOrDefault();

            //Check if user logged in or not
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            //Find a seller details
            var seller = _dbContext.Users.Where(x => x.ID == note.SellerID).FirstOrDefault();
            if (note == null)
            {
                return HttpNotFound();
            }
            if (remark == null)
            {
                ModelState.AddModelError("Remark","This field is required");
                return View();
            }

            if(user.RoleID == 2 || user.RoleID == 1)
            {
                note.Status = 11;
                note.ActionedBy = user.ID;
                note.AdminRemarks = remark;
                note.ModifiedBy = user.ID;
                note.ModifiedDate = DateTime.Now;
                note.IsActive = false;
                _dbContext.Entry(note).State = EntityState.Modified;
                _dbContext.SaveChanges();
                string sellername = seller.FirstName+" "+seller.LastName;
                string email = seller.EmailID;
                string notetitle = note.Title;
                string adminremark = note.AdminRemarks;
                BuildMailUnpublishNote(notetitle,adminremark,sellername,email);
            }

            return RedirectToAction("Dashboard", "Admin");
        }

        private void BuildMailUnpublishNote(string notetitle, string adminremark, string sellername, string email)
        {
            //Read All Context from Email Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "PublishNote" + ".cshtml");

            //SellerName Replace in Template
            body = body.Replace("ViewBag.SellerName", sellername);
            //Notes title Replace in Template
            body = body.Replace("ViewBag.NoteTitle", notetitle);
            //
            body = body.Replace("ViewBag.Remark", adminremark);
            body = body.ToString();
            //Get Support Email
            var fromemail = _dbContext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();
            //Mail From 
            string from, subject, to;
            from = fromemail.Value.Trim();
            subject = "Sorry! We need to remove your notes from our portalr";
            to = email.Trim();

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

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Notes/{noteid}")]
        public ActionResult AdminNoteDetails(int noteid)
        {
            //Find a note in SellerNotes Table
            var notedetail = _dbContext.SellerNotes.Find(noteid);
            if (notedetail == null)
            {
                return HttpNotFound();
            }
            //Create a object of AdminNoteDetails
            AdminNoteDetailsViewModel adminNote = new AdminNoteDetailsViewModel();
            adminNote.sellerNote = notedetail;

            //Count a total spam records
            int SpamRecord = _dbContext.SellerNotesReportedIssues.Where(x => x.NoteID == noteid).Count();
            adminNote.TotalSpamReport = SpamRecord;

            //Get reviews
            var reviewer = _dbContext.SellerNotesReviews.Where(x => x.NoteID == noteid).Select(x => x.Rating);
            //Count Total reviews
            var count = reviewer.Count();
            //Get Average Rating
            var avgrating = count > 0 ? Math.Ceiling(reviewer.Average()) : 0;

            adminNote.AverageRating = (int)avgrating;
            adminNote.TotalRate = count;

            //Review and user profile from database
            IEnumerable<UserReviewViewModel>  reviews =from review in _dbContext.SellerNotesReviews
                                                       join user in _dbContext.Users on review.ReviewedBYID equals user.ID
                                                       join userprofile in _dbContext.UserProfiles on user.ID equals userprofile.UserID
                                                       where review.NoteID==notedetail.ID && review.IsActive==true orderby review.Rating descending
                                                       select new UserReviewViewModel { tblReview = review, tblUser = user,tblUserProfile = userprofile };
            adminNote.NoteReview = reviews;
            return View(adminNote);
        }
    }
}