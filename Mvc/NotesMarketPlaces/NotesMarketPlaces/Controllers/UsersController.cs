using NotesMarketPlaces.Models;
using NotesMarketPlaces.SendMail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Users")]
    public class UsersController : Controller
    {
        readonly NotesMarketPlaceEntities1 _dbcontext = new NotesMarketPlaceEntities1();

        //Get: MyProfile
        [HttpGet]
        [Authorize(Roles = "Member")]
        [Route("MyProfile")]
        public ActionResult MyProfile()
        {
            ViewBag.MyProfile = "active";
            //If User is Logged in or not
            var user = _dbcontext.Users.Where(x=>x.EmailID==User.Identity.Name).FirstOrDefault();
            //Check this id is already in userprofile or not
            var userprofile = _dbcontext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();
            //Create a object of MyProfileViewModel
            MyProfileViewModel myProfile = new MyProfileViewModel();
            if (userprofile != null)
            {
                myProfile.CountryList = _dbcontext.Countries.Where(x=>x.IsActive==true).ToList();
                myProfile.GenderList = _dbcontext.ReferenceDatas.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                myProfile.UserID = user.ID;
                myProfile.FirstName = user.FirstName;
                myProfile.LastName = user.LastName;
                myProfile.Email = user.EmailID;
                myProfile.DOB = userprofile.DOB;
                myProfile.City = userprofile.City;
                myProfile.College = userprofile.College;
                myProfile.State = userprofile.State;
                myProfile.PhoneCode = userprofile.PhoneNumberCountryCode;
                myProfile.Country = userprofile.Country;
                myProfile.AddressLine1 = userprofile.AddressLine1;
                myProfile.AddressLine2 = userprofile.AddressLine2;
                myProfile.College = userprofile.College;
                myProfile.University = userprofile.University;
                myProfile.ZipCode = userprofile.ZipCode;
                myProfile.Gender = userprofile.Gender;
                myProfile.PhoneNumber = userprofile.PhoneNumber;
                myProfile.ProfilePictureUrl = userprofile.ProfilePicture;   
            }
            else
            {
                //To Add list in MyProfileViewModel
                myProfile.CountryList = _dbcontext.Countries.ToList();
                myProfile.GenderList = _dbcontext.ReferenceDatas.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                myProfile.UserID = user.ID;
                myProfile.FirstName = user.FirstName;
                myProfile.LastName = user.LastName;
                myProfile.Email = user.EmailID;
            }
            return View(myProfile);
        }
        //Post: MyProfile
        [HttpPost]
        [Authorize(Roles = "Member")]
        [Route("MyProfile")]
        public ActionResult MyProfile(MyProfileViewModel myProfile)
        {
            //Get logged in user
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var userProfile = _dbcontext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();

                //if pofile is not null then edit it
                if (userProfile != null)
                {
                    userProfile.DOB = myProfile.DOB;
                    userProfile.Gender = (int)myProfile.Gender ;
                    userProfile.City = myProfile.City;
                    userProfile.State = myProfile.State;
                    userProfile.Country = myProfile.Country;
                    userProfile.PhoneNumberCountryCode = myProfile.PhoneCode;
                    userProfile.PhoneNumber = myProfile.PhoneNumber;
                    userProfile.AddressLine1 = myProfile.AddressLine1;
                    userProfile.AddressLine2 = myProfile.AddressLine2;
                    userProfile.University = myProfile.University;
                    userProfile.College = myProfile.College;
                    userProfile.ModifiedDate = DateTime.Now;
                    userProfile.ModifiedBy = user.ID;
                    

                  

                    //Check if profile picture and myProfile picture is null or not
                    if (myProfile.ProfilePicture != null && userProfile.ProfilePicture!=null)
                    {
                        string path = Server.MapPath(userProfile.ProfilePicture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    //Check if upload Profile picture  then save it in the database
                    if (myProfile.ProfilePicture != null)
                    {
                        string filename = System.IO.Path.GetFileName(myProfile.ProfilePicture.FileName);
                        string fileextension = System.IO.Path.GetExtension(myProfile.ProfilePicture.FileName);
                        string newfilename = "DP_" +DateTime.Now.ToString("ddMMyyyy_hhmmss")+fileextension;
                        string profilepicturepath="~/Members/"+myProfile.UserID+"/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath),newfilename);
                        userProfile.ProfilePicture = profilepicturepath + newfilename;
                        myProfile.ProfilePicture.SaveAs(path);
                    }
                    _dbcontext.Entry(userProfile).State = EntityState.Modified;
                    _dbcontext.SaveChanges();

                    user.ID = myProfile.UserID;
                    user.ModifiedBy = user.ID;
                    user.ModifiedDate = DateTime.Now;
                    _dbcontext.Entry(user).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                }
                //user is not already in userprofile
                else
                {
                    UserProfile userprofile=new UserProfile();


                    userprofile.UserID = user.ID;
                    userprofile.DOB = myProfile.DOB;
                    userprofile.Gender = (int)myProfile.Gender;
                    userprofile.PhoneNumberCountryCode = myProfile.PhoneCode;
                    userprofile.PhoneNumber = myProfile.PhoneNumber;
                    userprofile.AddressLine1 = myProfile.AddressLine1;
                    userprofile.AddressLine2 = myProfile.AddressLine2;
                    userprofile.City = myProfile.City;
                    userprofile.State = myProfile.State;
                    userprofile.Country = myProfile.Country;
                    userprofile.College = myProfile.College;
                    userprofile.University = myProfile.University;
                    userprofile.ZipCode = myProfile.ZipCode;
                    userprofile.CreatedDate = DateTime.Now;
                    userprofile.CreatedBy = user.ID;

                    if (myProfile.ProfilePicture != null)
                    {
                        string filename = System.IO.Path.GetFileName(myProfile.ProfilePicture.FileName);
                        string fileextension = System.IO.Path.GetExtension(myProfile.ProfilePicture.FileName);
                        string newfilename = "DP_"+ DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + myProfile.UserID + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        userprofile.ProfilePicture = profilepicturepath + newfilename;
                        myProfile.ProfilePicture.SaveAs(path);
                    }
                    //Add Object in Userprofile in database
                    _dbcontext.UserProfiles.Add(userprofile);
                    _dbcontext.SaveChanges();


                    user.FirstName = myProfile.FirstName;
                    user.LastName = myProfile.LastName;
                    user.ModifiedBy = user.ID;
                    user.ModifiedDate = DateTime.Now;

                    //Save changes in database
                    _dbcontext.Users.Attach(user);
                    _dbcontext.SaveChanges();
                }
                return RedirectToAction("Index","Home");
            }
            else
            {
                myProfile.CountryList = _dbcontext.Countries.Where(x => x.IsActive == true).ToList();
                myProfile.GenderList = _dbcontext.ReferenceDatas.Where(x => x.RefCategory == "Gender" && x.IsActive == true).ToList();
                return View(myProfile);
            }
           
        }

        private void CreateDirectoryIfMissing(string folderpath)
        {
            bool folderalreadyexists= Directory.Exists(Server.MapPath(folderpath));
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }


        // GET: MyDownloads
        [Authorize(Roles = "Member")]
        [HttpGet]
        [Route("MyDownloads")]
        public ActionResult MyDownloads(string mydownloadsearch,string search,string sortorder,string sortby,int page=1)
        {
            //Navigation is Acitve
            ViewBag.MyDownloads = "active";

            //Sorting ,searching and Pagination
            ViewBag.Search = search;
            ViewBag.Sortorder = sortorder;
            ViewBag.SortBy = sortby;
            ViewBag.PageNumber = page;
            ViewBag.mydownload = mydownloadsearch;

            //Get User is Logged in User
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Get Download 
            IEnumerable<MyDownloadsViewModel> myDownloads = from download in _dbcontext.Downloads
                                                            join users in _dbcontext.Users on download.Downloader equals users.ID
                                                            join review in _dbcontext.SellerNotesReviews on download.Downloader equals review.AgainstDownloadsID into rev
                                                            from notereview in rev.DefaultIfEmpty()
                                                            where download.Downloader == user.ID && download.IsSellerHasAllowedDownload == true && download.AttachmentPath!=null
                                                            select new MyDownloadsViewModel {
                                                                                                NoteID=download.NoteID,
                                                                                                DownloadID=download.ID,
                                                                                                UserID=user.ID,
                                                                                                Buyer=users.EmailID,
                                                                                                Title=download.NoteTitle,
                                                                                                Category=download.NoteCategory,
                                                                                                DownloadDate=download.AttachmentDownloadedDate,
                                                                                                Price=(int)download.PurchasedPrice,
                                                                                                SellType=download.IsPaid == true ? "Paid" : "Free",
                                                                                                ReviewID=notereview.ID,
                                                                                                Ratings=(int)notereview.Rating,
                                                                                                Comments=notereview.Comments,
                                                                                              };

            //Search For in table
            if (!string.IsNullOrEmpty(mydownloadsearch))
            {
                mydownloadsearch = mydownloadsearch.ToLower();
                myDownloads = myDownloads.Where(x => x.Title.ToLower().Contains(mydownloadsearch)||
                                                x.Category.ToLower().Contains(mydownloadsearch)||
                                                x.Buyer.ToLower().Contains(mydownloadsearch)||
                                                x.Price.ToString().ToLower().Contains(mydownloadsearch)||
                                                x.SellType.Contains(mydownloadsearch)||
                                                x.DownloadDate.Value.ToString("dd MMM yyyy,hh:mm:ss").Contains(mydownloadsearch));
            }

            //Sorting and searching 
            //Going for sorting 
            myDownloads = SortTableMyDownload(sortorder, sortby,myDownloads);

            //Count a total Page Number
            ViewBag.TotalPages = Math.Ceiling((myDownloads.Count()) / 10.0);

            //Skip a next or Previous
            myDownloads = myDownloads.Skip((page - 1) * 10).Take(10);
            return View(myDownloads);
        }

        private IEnumerable<MyDownloadsViewModel> SortTableMyDownload(string sortorder, string sortby, IEnumerable<MyDownloadsViewModel> table)
        {
            switch (sortby)
            {
                case "NoteTitle":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Title);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                        }
                    }
                case "NoteCategory":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Category);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Category);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Category);
                                    return table;
                                }
                        }
                    }
                case "Buyer":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Buyer);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Buyer);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Buyer);
                                    return table;
                                }
                        }
                    }
                case "NotePrice":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Price);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Price);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Price);
                                    return table;
                                }
                        }
                    }
                case "DownloadDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.DownloadDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DownloadDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DownloadDate);
                                    return table;
                                }
                        }
                    }
                case "SellType":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.SellType);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.SellType);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.SellType);
                                    return table;
                                }
                        }
                    }
                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Title);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                        }
                    }
            }
        }

        //Post : AddReview
        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        [Route("Note/AddReview")]
        public ActionResult AddReview(FormCollection form)
        {
            //Check comment is null or not
            if (String.IsNullOrEmpty(form["Comments"]))
            {
                return RedirectToAction("MyDownloads");
            }
            int downloadid = Convert.ToInt32(form["DownloadID"]);
            int noteid = Convert.ToInt32(form["NoteID"]);
            //get Download object for check if user download note or not
            var notedownloaded = _dbcontext.Downloads.Where(x => x.ID == downloadid && x.IsAttachmentDownloaded==true).FirstOrDefault();

            //User can provide review after downloading the notes
            if (notedownloaded != null)
            {
                //get loggedin  user
                var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                //if check user is already 
                var already = _dbcontext.SellerNotesReviews.Where(x => x.AgainstDownloadsID == downloadid && x.IsActive == true).FirstOrDefault();
                //if user not provide a review
                if (already == null)
                {
                    SellerNotesReview review = new SellerNotesReview();
                    review.NoteID = noteid;
                    review.AgainstDownloadsID = downloadid;
                    review.ReviewedBYID = user.ID;
                    review.Rating = Convert.ToInt32(form["Rate1"]);
                    review.Comments = form["Comments"];
                    review.CreatedBy = user.ID;
                    review.CreatedDate = DateTime.Now;
                    review.IsActive = true;

                    _dbcontext.SellerNotesReviews.Add(review);
                    _dbcontext.SaveChanges();
                    return RedirectToAction("MyDownloads");
                }
                else
                {
                    already.Rating = Convert.ToInt32(form["Rate1"]);
                    already.Comments = form["Comments"];
                    already.ModifiedDate = DateTime.Now;
                    already.ModifiedBy = user.ID;
                    _dbcontext.Entry(already).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                    return RedirectToAction("MyDownloads");
                }
            }
            return RedirectToAction("MyDownloads");
        }

        
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        [Route("Note/SpamReports")]
        public ActionResult SpamReports(FormCollection form)
        {
            int DownloadID = Convert.ToInt32(form["downloadid"].ToString());
            int NoteID = Convert.ToInt32(form["noteid"].ToString());
            string remark = form["spamreports"].ToString();

            var downloader = _dbcontext.SellerNotesReportedIssues.Where(x => x.AgainstDownloadsID == DownloadID).FirstOrDefault();
            if (ModelState.IsValid) { 
                if (downloader == null)
                {
                    var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                    
                    SellerNotesReportedIssue sellerNotesReported = new SellerNotesReportedIssue();
                    sellerNotesReported.Remark = remark;
                    sellerNotesReported.CreatedBy = user.ID;
                    sellerNotesReported.AgainstDownloadsID = DownloadID;
                    sellerNotesReported.CreatedDate = DateTime.Now;
                    sellerNotesReported.ReportedBYID = user.ID;
                    sellerNotesReported.NoteID = NoteID;
                    _dbcontext.SellerNotesReportedIssues.Add(sellerNotesReported);
                    _dbcontext.SaveChanges();

                    string membername = user.FirstName + " " + user.LastName;
                    //Send mail for spam records
                    BuildMailSpamRecords(sellerNotesReported,membername);
                }
            }
            return RedirectToAction("MyDownloads");
        }

        private void BuildMailSpamRecords(SellerNotesReportedIssue sellerNotesReported,string membername)
        {
            //Find Name from seller id 
            var notes = _dbcontext.SellerNotes.Find(sellerNotesReported.ID);
            //Find Name of seller
            var seller = _dbcontext.Users.Where(x => x.ID == notes.SellerID).FirstOrDefault();

            //Read All Context from Email Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "spamRecords" + ".cshtml");

            //SellerName Replace in Template
            body = body.Replace("ViewBag.SellerName", seller.FirstName+" "+seller.LastName);
            //Buyer Name Replace in Template
            body = body.Replace("ViewBag.MemberName", membername);
            //Notes title Replace in Template
            body = body.Replace("ViewBag.NoteTitle", notes.Title);
            body = body.ToString();
            //Get Support Email
            var fromemail = _dbcontext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();
            var toemail = _dbcontext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();
            //Mail From 
            string from, subject, to;
            from = fromemail.Value.Trim();
            subject = membername + "Reported an issue for"+notes.Title;
            to = toemail.Value.Trim();

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

        [Authorize(Roles = "Member")]
        [HttpGet]
        [Route("SoldNotes")]
        public ActionResult MySoldNotes(string mysoldnotessearch, string sortby, string sortorder, int page = 1)
        {
            
            //Assigning A vaue of viewbag
            ViewBag.MySoldNotesSearch = mysoldnotessearch;
            ViewBag.PageNumber = page;
            ViewBag.SortOrder = sortorder;
            ViewBag.SortBy = sortby;

            //Find user Logged in or not
            var users = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Assignnig  a value of MySoldNotesViewModel
            IEnumerable<MySoldNotesViewModel> mysoldnotes = from download in _dbcontext.Downloads
                                                            join user in _dbcontext.Users on download.Downloader equals user.ID
                                                            where download.Seller == users.ID && download.IsAttachmentDownloaded == true && download.AttachmentPath != null
                                                            select new MySoldNotesViewModel
                                                            {
                                                                Buyer=user.EmailID,
                                                                Category=download.NoteCategory,
                                                                NoteTitle=download.NoteTitle,
                                                                DownloadedDate=download.AttachmentDownloadedDate,
                                                                DownloadID=download.ID,
                                                                Price=(int)download.PurchasedPrice,
                                                                SellType=download.IsPaid==true?"Paid":"Free",
                                                                NoteID=download.NoteID,
                                                            };

            //Search For in table
            if (!string.IsNullOrEmpty(mysoldnotessearch))
            {
                mysoldnotessearch = mysoldnotessearch.ToLower();
                mysoldnotes = mysoldnotes.Where(x => x.NoteTitle.ToLower().Contains(mysoldnotessearch) ||
                                                x.Category.ToLower().Contains(mysoldnotessearch) ||
                                                x.Buyer.ToLower().Contains(mysoldnotessearch) ||
                                                x.Price.ToString().ToLower().Contains(mysoldnotessearch)||
                                                x.SellType.ToLower().Contains(mysoldnotessearch)||
                                                x.DownloadedDate.Value.ToString("dd MMM yyyy,hh:mm:ss").Contains(mysoldnotessearch));
            }

            //Sorting and searching 
            //Going for sorting 
            mysoldnotes = SortTableMyDownload(sortorder, sortby, mysoldnotes);

            //Count a total Page Number
            ViewBag.TotalPages = Math.Ceiling((mysoldnotes.Count()) / 10.0);

            //Skip a next or Previous
            mysoldnotes = mysoldnotes.Skip((page - 1) * 10).Take(10);

            return View(mysoldnotes); 
        }

        private IEnumerable<MySoldNotesViewModel> SortTableMyDownload(string sortorder, string sortby, IEnumerable<MySoldNotesViewModel> table)
        {
            switch (sortby)
            {
                case "NoteTitle":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NoteTitle);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NoteTitle);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NoteTitle);
                                    return table;
                                }
                        }
                    }
                case "NoteCategory":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Category);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Category);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Category);
                                    return table;
                                }
                        }
                    }
                case "Buyer":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Buyer);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Buyer);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Buyer);
                                    return table;
                                }
                        }
                    }
                case "NotePrice":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Price);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Price);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Price);
                                    return table;
                                }
                        }
                    }
                case "DownloadDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.DownloadedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DownloadedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DownloadedDate);
                                    return table;
                                }
                        }
                    }
                case "SellType":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.SellType);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.SellType);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.SellType);
                                    return table;
                                }
                        }
                    }
                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NoteTitle);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NoteTitle);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NoteTitle);
                                    return table;
                                }
                        }
                    }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        [Route("RejectedNote")]
        public ActionResult MyRejectedNotes(string sortby,string sortorder,string rejectsearch,int page=1)
        {
            //Active Navigation
            ViewBag.RejectedNotes = "active";

            //ViewBag Value Assign
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.RejectedSearch = rejectsearch;

            //Check if user Logged in or not
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();


            IEnumerable<SellerNote> myrejected = _dbcontext.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == 10 && x.IsActive==true).ToList();

            if (!String.IsNullOrEmpty(rejectsearch))
            {
                rejectsearch = rejectsearch.ToLower();
                myrejected = myrejected.Where(x => x.Title.ToLower().Contains(rejectsearch)||
                                              x.NotesCategory.Name.ToLower().Contains(rejectsearch)||
                                              x.AdminRemarks.ToLower().Contains(rejectsearch)).ToList();
            }

            //Sorting and searching 
            //Going for sorting 
            myrejected = SortTableMyRejected(sortorder, sortby, myrejected);

            //Count a total Page Number
            ViewBag.TotalPages = Math.Ceiling((myrejected.Count()) / 10.0);

            //Skip a next or Previous
            myrejected = myrejected.Skip((page - 1) * 10).Take(10);

            return View(myrejected);
        }

        private IEnumerable<SellerNote> SortTableMyRejected(string sortorder, string sortby, IEnumerable<SellerNote> table)
        {
            switch (sortby)
            {
                case "NoteTitle":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Title);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                        }
                    }
                case "NoteCategory":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NotesCategory.Name);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NotesCategory.Name);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NotesCategory.Name);
                                    return table;
                                }
                        }
                    }

                case "Remark":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.AdminRemarks);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AdminRemarks);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AdminRemarks);
                                    return table;
                                }
                        }
                    }

                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Title);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Title);
                                    return table;
                                }
                        }
                    }
            }
        }
        [Authorize(Roles = "Member")]
        [Route("Clone/{noteid}")]
        public ActionResult Clone(int noteid)
        {
            //Find a noteid with all details in Database
            var notes = _dbcontext.SellerNotes.Find(noteid);

            //Check if user is logged in or not
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Derived Deaftid in the database
            int draftid = _dbcontext.ReferenceDatas.Where(x => x.Value == "Draft").Select(x => x.ID).FirstOrDefault();

            SellerNote sellernotes = new SellerNote();
            sellernotes.SellerID = user.ID;
            sellernotes.Status = draftid;
            sellernotes.SellingPrice = notes.SellingPrice;
            sellernotes.NotesCategory = notes.NotesCategory;
            sellernotes.Title = notes.Title;
            sellernotes.Country = notes.Country;
            sellernotes.Course = notes.Course;
            sellernotes.CourseCode = notes.CourseCode;
            sellernotes.CreatedBy = user.ID;
            sellernotes.CreatedDate = DateTime.Now;
            sellernotes.UniversityName = notes.UniversityName;
            sellernotes.NumberofPages = notes.NumberofPages;
            sellernotes.IsPaid = notes.IsPaid;
            sellernotes.IsActive = true;
            sellernotes.NotesType = notes.NotesType;
            sellernotes.Professor = notes.Professor;
            if (notes.DisplayPicture!=null)
            {
                var rejectedfilepath = Server.MapPath(notes.DisplayPicture);
                var clonenotefilepath= "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                var filepath = Path.Combine(Server.MapPath(clonenotefilepath));

                FileInfo file = new FileInfo(rejectedfilepath);
                Directory.CreateDirectory(filepath);
                if (file.Exists)
                {
                    System.IO.File.Copy(rejectedfilepath,Path.Combine(filepath,Path.GetFileName(rejectedfilepath)));
                }
                sellernotes.DisplayPicture = Path.Combine(clonenotefilepath, Path.GetFileName(rejectedfilepath));
                _dbcontext.SaveChanges();
                
            }
            if (notes.NotesPreview != null)
            {
                var rejectedfilepath = Server.MapPath(notes.DisplayPicture);
                var clonenotefilepath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                var filepath = Path.Combine(Server.MapPath(clonenotefilepath));

                FileInfo file = new FileInfo(rejectedfilepath);
                Directory.CreateDirectory(filepath);
                if (file.Exists)
                {
                    System.IO.File.Copy(rejectedfilepath, Path.Combine(filepath, Path.GetFileName(rejectedfilepath)));
                }
                sellernotes.NotesPreview = Path.Combine(clonenotefilepath, Path.GetFileName(rejectedfilepath));
                _dbcontext.SaveChanges();

            }

            var rejectednoteattachment = Server.MapPath("~/Members/" + user.ID + "/" + sellernotes.ID + "/Attachements/");
            var clonenoteattachment = "~/Members/" + user.ID + "/" + sellernotes.ID+ "/Attachements/";

            var attachmentfilepath = Path.Combine(Server.MapPath(clonenoteattachment));
            Directory.CreateDirectory(attachmentfilepath);
            foreach(var files in Directory.GetFiles(rejectednoteattachment))
            {
                FileInfo file = new FileInfo(files);
                if (file.Exists)
                {
                    System.IO.File.Copy(file.ToString(), Path.Combine(attachmentfilepath, Path.GetFileName(file.ToString())));
                }
                SellerNotesAttachement attachement = new SellerNotesAttachement();
                attachement.NoteID = sellernotes.ID;
                attachement.FileName = Path.GetFileName(file.ToString());
                attachement.FilePath = clonenoteattachment;
                attachement.CreatedDate = DateTime.Now;
                attachement.CreatedBy = user.ID;
                attachement.IsActive = true;
                _dbcontext.SellerNotesAttachements.Add(attachement);
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("Index","SellYourNotes");
        }

    }
}