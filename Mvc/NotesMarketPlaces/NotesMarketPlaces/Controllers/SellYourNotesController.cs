using NotesMarketPlaces.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlaces.ViewModels;
using System.Web.Hosting;
using System.Text;
using System.Net.Mail;
using NotesMarketPlaces.Send_Mail;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("SellYourNotes")]
    public class SellYourNotesController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbcontext = new NotesMarketPlaceEntities1();

        // GET: SellYourNotes
        [HttpGet]
        [Authorize(Roles = "Member")]
        public ActionResult Index(string inprogresssearch, string publishsearch, string sortorder, string sortby, string sortorderpublish, string sortbypublish, int inprogresspage = 1, int publishpage = 1)
        {
            //To Active a navigation bar
            ViewBag.SellYourNotes = "active";

            //Sorting ,Searching and Pagination
            ViewBag.SortOrder = sortorder;
            ViewBag.SortOrderForPublishedNotes = sortorderpublish;
            ViewBag.SortBy = sortby;
            ViewBag.SortByForPublishedNotes = sortbypublish;
            ViewBag.PageNumber = inprogresspage;
            ViewBag.PageNumberPublished = publishpage;
            ViewBag.InProgress = inprogresssearch;
            ViewBag.Published = publishsearch;

            //To Create a object of SellYour Notes View Model
            SellYourNotesViewModel sellYourNotesView = new SellYourNotesViewModel();

            //To check user is Logged in or not
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Get ID for note status submitted for review,draft,rejected,published 
            var submittedreviewid = _dbcontext.ReferenceDatas.Where(x => x.Value.ToLower()== "SubmittedForReview").Select(x=>x.ID).FirstOrDefault();
            var inreviewid = _dbcontext.ReferenceDatas.Where(x => x.Value.ToLower() == "InReview").Select(x => x.ID).FirstOrDefault();
            var draftid = _dbcontext.ReferenceDatas.Where(x => x.Value.ToLower() == "Draft").Select(x => x.ID).FirstOrDefault();
            var rejectedid = _dbcontext.ReferenceDatas.Where(x => x.Value.ToLower() == "Rejected").Select(x => x.ID).FirstOrDefault();
            var publishedid = _dbcontext.ReferenceDatas.Where(x => x.Value.ToLower() == "Published").Select(x => x.ID).FirstOrDefault();

            //Count Total no. of Downloads a note
            int download = _dbcontext.Downloads.Where(x => x.CreatedBy == user.ID&&x.IsSellerHasAllowedDownload==true&&x.AttachmentPath!=null).Count();

            //Count a Total no. of Logged in user  rejected note
            int rejected = _dbcontext.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == rejectedid).Count();

            //Count a Total no. of Logged in user paid note for request
            int request = _dbcontext.Downloads.Where(x => x.CreatedBy == user.ID && x.IsSellerHasAllowedDownload == false&&x.AttachmentPath==null).Count();

            //Count a Total no. of Sold Notes
            int soldnotes = _dbcontext.Downloads.Where(x => x.Seller==user.ID && x.AttachmentPath!=null).Count();

            //Total earning of logged in user
            var totalearning = _dbcontext.Downloads.Where(x => x.Seller == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Select(x => x.PurchasedPrice).Sum();

            int earned;
            if (totalearning != null)
            {
                earned = (int)totalearning;
            }
            else
            {
                earned = 0;
            }


            //Assign a Value of ViewModel
            sellYourNotesView.MyDownload = download;
            sellYourNotesView.BuyerRequestNotes = request;
            sellYourNotesView.MyRejectedNotes = rejected;
            sellYourNotesView.NumberOfSoldNotes = soldnotes;
            sellYourNotesView.MoneyEarned = earned;

            //Get InProgressNotes
            sellYourNotesView.InProressNotes = from note in _dbcontext.SellerNotes
                                               where ((note.Status ==draftid || note.Status == submittedreviewid || note.Status ==inreviewid )&& note.SellerID == user.ID)
                                               select new InProgressNote
                                               {
                                                   NoteID = note.ID,
                                                   Title = note.Title,
                                                   Category = note.NotesCategory.Name,
                                                   Status=note.ReferenceData.Value,
                                                   AddedDate=note.CreatedDate.Value
                                               };

            //Check If String is null or not
            if (!string.IsNullOrEmpty(inprogresssearch))
            {
                inprogresssearch = inprogresssearch.ToLower();
                sellYourNotesView.InProressNotes = sellYourNotesView.InProressNotes.Where(x => x.AddedDate.ToString().ToLower().Contains(inprogresssearch)||
                                                                                          x.Category.ToLower().ToLower().Contains(inprogresssearch)||
                                                                                          x.Title.ToLower().ToLower().Contains(inprogresssearch)||
                                                                                          x.Status.ToLower().Contains(inprogresssearch)
                                                                                          ).ToList();
            }

            sellYourNotesView.PublishedNotes = from note in _dbcontext.SellerNotes
                                               where note.Status == publishedid && note.SellerID == user.ID
                                               select new PublishedNote
                                               {
                                                   NoteID=note.ID,
                                                   Title=note.Title,
                                                   Category=note.NotesCategory.Name,
                                                   SellType=note.IsPaid==true?"Paid":"False",
                                                   Price=note.SellingPrice,
                                                   PublishedDate=note.PublishedDate.Value
                                               };
           
            //Check String is null or nor
            if (!string.IsNullOrEmpty(publishsearch))
            {
                sellYourNotesView.PublishedNotes = sellYourNotesView.PublishedNotes.Where
                                                        (
                                                            x => x.Title.ToLower().Contains(publishsearch) ||
                                                            x.Category.ToLower().Contains(publishsearch) ||
                                                            x.Price.ToString().ToLower().Contains(publishsearch)||
                                                            x.SellType.ToString().Contains(publishsearch)
                                                        ).ToList();
            }
            
            //Going for sorting 
            sellYourNotesView.InProressNotes = SortTableDashboard(sortorder, sortby, sellYourNotesView.InProressNotes);
            sellYourNotesView.PublishedNotes = SortTableDashboard(sortorder, sortby, sellYourNotesView.PublishedNotes);

            //Total Page Count
            ViewBag.TotalPageInProgress = Math.Ceiling(sellYourNotesView.InProressNotes.Count() / 5.0);
            ViewBag.TotalPagePublish = Math.Ceiling(sellYourNotesView.PublishedNotes.Count() / 5.0);

            //Going to next number
            sellYourNotesView.InProressNotes = sellYourNotesView.InProressNotes.Skip((inprogresspage - 1) * 5).Take(5);
            sellYourNotesView.PublishedNotes = sellYourNotesView.PublishedNotes.Skip((publishpage - 1) * 5).Take(5);
            return View(sellYourNotesView);
        }

        private IEnumerable<PublishedNote> SortTableDashboard(string sortorder, string sortby, IEnumerable<PublishedNote> table)
        {
            switch (sortby)
            {
                case "PublishedDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.PublishedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.PublishedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.PublishedDate);
                                    return table;
                                }
                        }
                    }
                case "Title":
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
                case "Category":
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
                case "Price":
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
                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.PublishedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.PublishedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.PublishedDate);
                                    return table;
                                }
                        }
                    }
            }
        }

        private IEnumerable<InProgressNote> SortTableDashboard(string sortorder, string sortby, IEnumerable<InProgressNote> table)
        {
            switch (sortby)
            {
                case "CreatedDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.AddedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AddedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AddedDate);
                                    return table;
                                }
                        }
                    }
                case "Title":
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
                case "Category":
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
                case "Status":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Status);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Status);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Status);
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
                                    table = table.OrderBy(x => x.AddedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AddedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AddedDate);
                                    return table;
                                }
                        }
                    }
            }
        }

        [Authorize(Roles = "Member")]
        [HttpGet]
        [Route("DeleteDraft/{id}")]
        public ActionResult DeleteDraft(int id)
        {
            //To find in sellernote with id
            SellerNote note = _dbcontext.SellerNotes.Find(id);

            if (note == null)
            {
                return HttpNotFound();
            }
            //To find Sellernotesattachment with note id
            IEnumerable<SellerNotesAttachement> notesAttachement = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == id&& x.IsActive==true).ToList();

            //if note attachment count is 0
            if (notesAttachement.Count() == 0)
            {
                return HttpNotFound();
            }


            string notefolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID);
            string notesattachementFolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID+"/Attachments");

            //Get directoty info
            DirectoryInfo notefolder = new DirectoryInfo(notefolderpath);
            DirectoryInfo attachmentfolder = new DirectoryInfo(notesattachementFolderpath);

            //Empty Directory
            EmptyFolder(notefolder);
            EmptyFolder(attachmentfolder);

            //Delete Folder
            Directory.Delete(notefolderpath);
            //Remove a note detail in Sellernotes
            _dbcontext.SellerNotes.Remove(note);

            foreach(var item in notesAttachement)
            {
                SellerNotesAttachement attachement = _dbcontext.SellerNotesAttachements.Where(x => x.ID == item.ID).FirstOrDefault();
                _dbcontext.SellerNotesAttachements.Remove(attachement);
            }
            //Save Changes in Database
            _dbcontext.SaveChanges();

            return RedirectToAction("Index"); 
        }

        //To Empty Folder
        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach(FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo subdirectory in directoryInfo.GetDirectories())
            {
                EmptyFolder(subdirectory);
                subdirectory.Delete();
            }
            
        }
        // GET: Notes
        [HttpGet]
        [Authorize(Roles = "Member")]
        [Route("AddNotes")]
        public ActionResult AddNotes()
        {
            //To Add List in View Model
            AddNotesViewModel addNotesViewModel = new AddNotesViewModel
            {
                NoteCategoryList = _dbcontext.NotesCategories.Where(x=>x.IsActive==true).ToList(),
                NoteTypeList = _dbcontext.NotesTypes.Where(x=>x.IsActive==true).ToList(),
                CountryList = _dbcontext.Countries.Where(x=>x.IsActive==true).ToList()
            };

            return View(addNotesViewModel);
        }

        // POST: Notes/AddNotes
        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        [Route("AddNotes")]
        public ActionResult AddNotes(AddNotesViewModel addnotesviewmodel)
        {
            //To check upload notes field is empty or not
            if (addnotesviewmodel.UploadNotes[0] == null)
            {
                ModelState.AddModelError("UploadNotes", "This Field is Required");
                addnotesviewmodel.NoteCategoryList = _dbcontext.NotesCategories.Where(x => x.IsActive == true).ToList();
                addnotesviewmodel.NoteTypeList = _dbcontext.NotesTypes.Where(x => x.IsActive == true).ToList();
                addnotesviewmodel.CountryList = _dbcontext.Countries.Where(x => x.IsActive == true).ToList();
                return View(addnotesviewmodel);
            }
            //If Note type is paid and Note Preview is null or not
            if(addnotesviewmodel.IsPaid==true && addnotesviewmodel.NotesPreview == null)
            {
                ModelState.AddModelError("NotesPreview", "This is required field");
                addnotesviewmodel.NoteCategoryList = _dbcontext.NotesCategories.Where(x => x.IsActive == true).ToList();
                addnotesviewmodel.NoteTypeList = _dbcontext.NotesTypes.Where(x => x.IsActive == true).ToList();
                addnotesviewmodel.CountryList = _dbcontext.Countries.Where(x => x.IsActive == true).ToList();
                return View(addnotesviewmodel);
            }
            if (ModelState.IsValid)
            {
                SellerNote sellernotes = new SellerNote();
                //If User is Logged in or not
                User user = _dbcontext.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                //Add Data in Database
                sellernotes.SellerID = user.ID;
                sellernotes.Title = addnotesviewmodel.Title;
                sellernotes.Status = _dbcontext.ReferenceDatas.Where(x=>x.Value.ToLower()== "Draft").Select(x=>x.ID).FirstOrDefault();
                sellernotes.Category = addnotesviewmodel.Category;
                sellernotes.NotesType = addnotesviewmodel.NoteType;
                sellernotes.NumberofPages = addnotesviewmodel.NumberofPages;
                sellernotes.Description = addnotesviewmodel.Description;
                sellernotes.UniversityName = addnotesviewmodel.UniversityName;
                sellernotes.Country = addnotesviewmodel.Country;
                sellernotes.Course = addnotesviewmodel.Course;
                sellernotes.CourseCode = addnotesviewmodel.CourseCode;
                sellernotes.Professor = addnotesviewmodel.Professor;
                sellernotes.IsPaid = addnotesviewmodel.IsPaid;
                if (sellernotes.IsPaid)
                {
                    sellernotes.SellingPrice = addnotesviewmodel.SellingPrice;
                }
                else
                {
                    sellernotes.SellingPrice = 0;
                }
                sellernotes.CreatedDate = DateTime.Now;
                sellernotes.CreatedBy = user.ID;
                sellernotes.IsActive = true;

                _dbcontext.SellerNotes.Add(sellernotes);
                //Save Changes in Database
                _dbcontext.SaveChanges();

                //Get Seller note
                sellernotes = _dbcontext.SellerNotes.Find(sellernotes.ID);

                //If Display Picture is not null then save  picture into directory and directory path into database
                if (addnotesviewmodel.DisplayPicture != null)
                {
                    string displaypicturefilename = System.IO.Path.GetFileName(addnotesviewmodel.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    addnotesviewmodel.DisplayPicture.SaveAs(displaypicturefilepath);
                }
                //If NotesPreview is not null then save  picture into directory and directory path into database
                if (addnotesviewmodel.NotesPreview != null)
                {
                    string notespreviewfilename = System.IO.Path.GetFileName(addnotesviewmodel.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    sellernotes.NotesPreview = notespreviewpath + notespreviewfilename;
                    addnotesviewmodel.NotesPreview.SaveAs(notespreviewfilepath);
                }

                _dbcontext.SellerNotes.Attach(sellernotes);
                _dbcontext.Entry(sellernotes).Property(x => x.DisplayPicture).IsModified = true;
                _dbcontext.Entry(sellernotes).Property(x => x.NotesPreview).IsModified = true;
                _dbcontext.SaveChanges();


                //If Uploads Notes is not null then save  picture into directory and directory path into database
                if (addnotesviewmodel.UploadNotes != null)
                {
                    //Attachment if file null or not
                    foreach (HttpPostedFileBase file in addnotesviewmodel.UploadNotes)
                    {
                        //checked if file is null or not
                        if (file != null)
                        {   
                            //Save file in directory
                            string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                            string notesattachementpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/Attachements/";
                            CreateDirectoryIfMissing(notesattachementpath);
                            string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath),notesattachementfilename);
                            file.SaveAs(notesattachementfilepath);

                            //Enter Data in sellernotesattachments
                            SellerNotesAttachement notesattachements = new SellerNotesAttachement
                            {
                                NoteID = sellernotes.ID,
                                FileName = notesattachementfilename,
                                FilePath = notesattachementpath,
                                CreatedDate = DateTime.Now,
                                CreatedBy = user.ID,
                                IsActive = true
                            };

                            _dbcontext.SellerNotesAttachements.Add(notesattachements);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
                
                return RedirectToAction("Index", "SellYourNotes");
            }
            else
            {
                AddNotesViewModel viewModel = new AddNotesViewModel
                {
                    NoteCategoryList = _dbcontext.NotesCategories.Where(x=>x.IsActive==true).ToList(),
                    NoteTypeList = _dbcontext.NotesTypes.Where(x => x.IsActive == true).ToList(),
                    CountryList = _dbcontext.Countries.Where(x => x.IsActive == true).ToList()
                };

                return View(viewModel);
            }
        }
        // GET: Notes
        [HttpGet]
        [Authorize(Roles = "Member")]
        [Route("EditNotes")]
        public ActionResult EditNotes(int id)
        {
            //To check user is logged in or not
            User user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            SellerNote note = _dbcontext.SellerNotes.Where(x => x.ID == id &&x.IsActive==true&&x.SellerID==user.ID).FirstOrDefault();
            SellerNotesAttachement sellerNotesAttachement = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == note.ID).FirstOrDefault();
            //If User and note seller are Same
            if (note!=null)
            {
                //Create a object to View Model
                EditNotesViewModel editNotesViewModel = new EditNotesViewModel
                {
                    ID = note.ID,
                    NoteID = note.ID,
                    Title = note.Title,
                    Category = note.Category,
                    Picture = note.DisplayPicture,
                    Note = sellerNotesAttachement.FilePath+sellerNotesAttachement.FileName,
                    NumberofPages = note.NumberofPages,
                    Description = note.Description,
                    NoteType = note.NotesType,
                    UniversityName = note.UniversityName,
                    Course = note.Course,
                    CourseCode = note.CourseCode,
                    Country = note.Country,
                    Professor = note.Professor,
                    IsPaid = note.IsPaid,
                    SellingPrice = note.SellingPrice,
                    Preview = note.NotesPreview,
                    NoteCategoryList = _dbcontext.NotesCategories.Where(x=>x.IsActive==true).ToList(),
                    NoteTypeList = _dbcontext.NotesTypes.Where(x => x.IsActive == true).ToList(),
                    CountryList = _dbcontext.Countries.Where(x => x.IsActive == true).ToList(),
                };

                return View(editNotesViewModel);
            }
            else
            {

                return RedirectToAction("Index", "SellYourNotes");
            }
        }

        //Post:/EditNotes
        [HttpPost]
        [Authorize(Roles = "Member")]
        [Route("EditNotes")]
        public ActionResult EditNotes(int id,EditNotesViewModel editNotesViewModel)
        {
            if (ModelState.IsValid)
            {

                var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                var sellernotes = _dbcontext.SellerNotes.Where(x => x.ID == id && x.IsActive==true && x.SellerID==user.ID).FirstOrDefault();
                var notesattachment = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == editNotesViewModel.NoteID && x.IsActive == true).ToList();  ;

                if (sellernotes == null)
                {
                    return HttpNotFound();
                }
                //Check if note is paid or preview is not null
                if (editNotesViewModel.IsPaid == true && editNotesViewModel.NotesPreview == null && sellernotes.NotesPreview == null)
                {
                    return HttpNotFound();
                }
                _dbcontext.SellerNotes.Attach(sellernotes);
                sellernotes.Title = editNotesViewModel.Title;
                sellernotes.Category = editNotesViewModel.Category;
                sellernotes.NotesType = editNotesViewModel.NoteType;
                sellernotes.NumberofPages = editNotesViewModel.NumberofPages;
                sellernotes.Description = editNotesViewModel.Description;
                sellernotes.UniversityName = editNotesViewModel.UniversityName;
                sellernotes.Country = editNotesViewModel.Country;
                sellernotes.Course = editNotesViewModel.Course;
                sellernotes.CourseCode = editNotesViewModel.CourseCode;
                sellernotes.Professor = editNotesViewModel.Professor;
                if (sellernotes.IsPaid==true)
                {
                    sellernotes.IsPaid =true;
                    sellernotes.SellingPrice = editNotesViewModel.SellingPrice;
                }
                else
                {
                    sellernotes.IsPaid = false;
                    sellernotes.SellingPrice = 0;
                }
                sellernotes.ModifiedDate = DateTime.Now;
                sellernotes.ModifiedBy = user.ID;
                _dbcontext.SaveChanges();

                //if display picture is not null
                if (editNotesViewModel.DisplayPicture != null)
                {
                    //if note object has already ppreviously uploaded picture then delete it
                    if (sellernotes.DisplayPicture != null)
                    {
                        string path = Server.MapPath(sellernotes.DisplayPicture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    //To Update Display Picture in root directory
                    string displaypicturefilename = System.IO.Path.GetFileName(editNotesViewModel.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    editNotesViewModel.DisplayPicture.SaveAs(displaypicturefilepath);
                }
                //if note preview is not null
                if (editNotesViewModel.NotesPreview != null)
                {
                    //if note object has already ppreviously uploaded picture then delete it
                    if (sellernotes.NotesPreview != null)
                    {
                        string path = Server.MapPath(sellernotes.NotesPreview);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
                    //To Update Notes Preview in root directory
                    string notespreviewfilename = System.IO.Path.GetFileName(editNotesViewModel.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    sellernotes.NotesPreview = notespreviewpath + notespreviewfilename;
                    editNotesViewModel.NotesPreview.SaveAs(notespreviewfilepath);
                }


                if (editNotesViewModel.UploadNotes[0] != null)
                {
                    //if user upload notes then delete directory that have previousley uploaded
                    string path = Server.MapPath(notesattachment[0].FilePath);
                    DirectoryInfo dir = new DirectoryInfo(path);
                    EmptyFolder(dir);

                    //Remove previously uploaded attachment from database
                    foreach (var item in notesattachment)
                    {
                        SellerNotesAttachement attachement = _dbcontext.SellerNotesAttachements.Where(x => x.ID == item.ID).FirstOrDefault();
                        _dbcontext.SellerNotesAttachements.Remove(attachement);
                    }
                    //Add Newly upload file is null or not
                    foreach(HttpPostedFileBase file in editNotesViewModel.UploadNotes)
                    {
                        if (file != null)
                        {
                            //Save file in directory
                            string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                            string notesattachementpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/Attachements/";
                            CreateDirectoryIfMissing(notesattachementpath);
                            string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath), notesattachementfilename);
                            file.SaveAs(notesattachementfilepath);

                            SellerNotesAttachement sellerNotesAttachement = new SellerNotesAttachement
                            {
                                NoteID = sellernotes.ID,
                                FileName = notesattachementfilename,
                                FilePath = notesattachementpath,
                                CreatedDate = DateTime.Now,
                                CreatedBy = user.ID,
                                IsActive = true
                            };

                            //Save seller notes attachment
                            _dbcontext.SellerNotesAttachements.Add(sellerNotesAttachement);
                            _dbcontext.SaveChanges();
                        }
                    }
                }
                _dbcontext.SaveChanges();
                return RedirectToAction("Index", "SellYourNotes");
            }
            else
            {

                return RedirectToAction("EditNotes", new { id = editNotesViewModel.ID }); ;
            }
        }
        private void CreateDirectoryIfMissing(string folderpath)
        {
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }

        [Authorize(Roles = "Member")]
        [Route("SellYourNotes/Publish")]
        public ActionResult Publish(int id)
        {
            //Find  A Seller Note with Id
            var note = _dbcontext.SellerNotes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }

            //User Id find in database
            var user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Seller full name
            string sellrname = user.FirstName + " " + user.LastName;
            //Send Email address
            string email = user.EmailID;
            
            //Note Seller and user is Same
            if (note.SellerID == user.ID)
            {
                _dbcontext.SellerNotes.Attach(note);
                note.Status = 7;
                note.PublishedDate = DateTime.Now;
                note.ModifiedDate = DateTime.Now;
                note.ModifiedBy = user.ID;
                _dbcontext.SaveChanges();
                PublishedNote(note.Title,email, sellrname);
            }
            return RedirectToAction("Index", "SellYourNotes");
        }

        public void PublishedNote(string note,string email, string sellername)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "PublishNote" + ".cshtml");
            body = body.Replace("ViewBag.SellerName", sellername);
            body = body.Replace("ViewBag.NoteTitle", note);
            body = body.ToString();
            //get support email
            var fromemail = _dbcontext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();

            //Set from to subject ,body
            string from, to, subject;
            from = fromemail.Value.Trim();
            to = email.Trim();
            subject = sellername + " sent this note for review";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            //create a mailmessage object
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from, "NotesMarketPlace");
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendingMail.SendEmail(mail);
        }
    }
}