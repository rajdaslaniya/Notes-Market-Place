using NotesMarketPlaces.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("SellYourNotes")]
    public class SellYourNotesController : Controller
    {
        readonly private NotesMarketPlaceEntities _dbcontext = new NotesMarketPlaceEntities();

        // GET: SellYourNotes
        [HttpGet]
        [Authorize]
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
            User user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            //Check String is null or nor
            if (string.IsNullOrEmpty(inprogresssearch))
            {
                sellYourNotesView.InProressNotes = _dbcontext.SellerNotes.Where
                                                   ( 
                                                        x => x.SellerID == user.ID && 
                                                        (x.Status == 6 || x.Status == 7 || x.Status == 8)
                                                   );
            }
            else
            {
                inprogresssearch = inprogresssearch.ToLower();
                sellYourNotesView.InProressNotes = _dbcontext.SellerNotes.Where
                                                    (   
                                                        x => x.SellerID == user.ID && 
                                                        (x.Status == 6 || x.Status == 7 || x.Status == 8)
                                                        && (x.Title.ToLower().Contains(inprogresssearch)) 
                                                        || (x.NotesCategory.Name.ToLower().Contains(inprogresssearch)) 
                                                        || (x.ReferenceData.Value.ToLower().Contains(inprogresssearch))
                                                    );
            }
            //Check String is null or nor
            if (string.IsNullOrEmpty(publishsearch))
            {
                sellYourNotesView.PublishedNotes = _dbcontext.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == 9));
            }
            else

            {
                publishsearch = publishsearch.ToLower();
                sellYourNotesView.PublishedNotes = _dbcontext.SellerNotes.Where
                                                    (
                                                        x => x.SellerID == user.ID &&
                                                        (x.Status == 9) &&
                                                        (x.Title.ToLower().Contains(publishsearch) || 
                                                        x.NotesCategory.Name.ToLower().Contains(publishsearch) ||
                                                        x.SellingPrice.ToString().ToLower().Contains(publishsearch))
                                                    );
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

        private IEnumerable<SellerNote> SortTableDashboard(string sortorder, string sortby, IEnumerable<SellerNote> table)
        {
            switch (sortby)
            {
                case "CreatedDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.CreatedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.CreatedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.CreatedDate);
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
                case "Status":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.ReferenceData.Value);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.ReferenceData.Value);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.ReferenceData.Value);
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
                                    table = table.OrderBy(x => x.SellingPrice);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.SellingPrice);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.SellingPrice);
                                    return table;
                                }
                        }
                    }
                case "SeallType":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.IsPaid);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.IsPaid);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.IsPaid);
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
                                    table = table.OrderBy(x => x.CreatedDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.CreatedDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.CreatedDate);
                                    return table;
                                }
                        }
                    }
            }
        }


        [HttpGet]
        [Route("DeleteDraft")]
        public ActionResult DeleteDraft(int id)
        {
            //To find in sellernote with id
            SellerNote note = _dbcontext.SellerNotes.Find(id);
            //To find Sellernotesattachment with note id
            SellerNotesAttachement notesAttachement = _dbcontext.SellerNotesAttachements.FirstOrDefault(x => x.NoteID == id);
            string notefolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID);
            string notesattachementFolderpath = Server.MapPath("~/Members/" + note.SellerID + "/" + note.ID+"/Attachments");

            DirectoryInfo notefolder = new DirectoryInfo(notefolderpath);
            EmptyFolder(notefolder);
            //Delete Folder
            Directory.Delete(notefolderpath);

            _dbcontext.SellerNotes.Remove(note);
            _dbcontext.SellerNotesAttachements.Remove(notesAttachement);
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
        [Authorize]
        [Route("AddNotes")]
        public ActionResult AddNotes()
        {
            //To Add List in View Model
            AddNotesViewModel addNotesViewModel = new AddNotesViewModel
            {
                NoteCategoryList = _dbcontext.NotesCategories.ToList(),
                NoteTypeList = _dbcontext.NotesTypes.ToList(),
                CountryList = _dbcontext.Countries.ToList()
            };

            return View(addNotesViewModel);
        }

        // POST: Notes/AddNotes
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddNotes(AddNotesViewModel addnotesviewmodel)
        {
            //To check upload notes field is empty or not
            if (addnotesviewmodel.UploadNotes[0] == null)
            {
                ModelState.AddModelError("UploadNotes", "This Field is Required");
                return View(addnotesviewmodel);
            }
            //If Note type is paid and Note Preview is null or not
            if(addnotesviewmodel.IsPaid==true && addnotesviewmodel.NotesPreview == null)
            {
                ModelState.AddModelError("NotesPreview", "This is required field");
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
                sellernotes.Status = 6;
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

                sellernotes = _dbcontext.SellerNotes.Find(sellernotes.ID);

                if (addnotesviewmodel.DisplayPicture == null)
                {
                    string displaypicturefilename = "Book-Image.png";
                    string displaypicturepath = "~/Default Item/";
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    addnotesviewmodel.DisplayPicture.SaveAs(displaypicturefilepath);
                }
                else
                {
                    string displaypicturefilename = System.IO.Path.GetFileName(addnotesviewmodel.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    addnotesviewmodel.DisplayPicture.SaveAs(displaypicturefilepath);
                }

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

                if (addnotesviewmodel.UploadNotes != null)
                {
                    foreach (HttpPostedFileBase file in addnotesviewmodel.UploadNotes)
                    {
                        if (file != null)
                        {
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
                    NoteCategoryList = _dbcontext.NotesCategories.ToList(),
                    NoteTypeList = _dbcontext.NotesTypes.ToList(),
                    CountryList = _dbcontext.Countries.ToList()
                };

                return View(viewModel);
            }
        }
        // GET: Notes
        [HttpGet]
        [Authorize]
        [Route("EditNotes")]
        public ActionResult EditNotes(int id)
        {
            //To check user is logged in or not
            User user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            SellerNote note = _dbcontext.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            SellerNotesAttachement sellerNotesAttachement = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == note.ID).FirstOrDefault();
            //If User and note seller are Same
            if (user.ID == note.SellerID)
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
                    NoteCategoryList = _dbcontext.NotesCategories.ToList(),
                    NoteTypeList = _dbcontext.NotesTypes.ToList(),
                    CountryList = _dbcontext.Countries.ToList(),
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
        [Authorize]
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
                sellernotes.Status = 6;
                sellernotes.Category = editNotesViewModel.Category;
                sellernotes.NotesType = editNotesViewModel.NoteType;
                sellernotes.NumberofPages = editNotesViewModel.NumberofPages;
                sellernotes.Description = editNotesViewModel.Description;
                sellernotes.UniversityName = editNotesViewModel.UniversityName;
                sellernotes.Country = editNotesViewModel.Country;
                sellernotes.Course = editNotesViewModel.Course;
                sellernotes.CourseCode = editNotesViewModel.CourseCode;
                sellernotes.Professor = editNotesViewModel.Professor;
                sellernotes.IsPaid = editNotesViewModel.IsPaid;
                if (sellernotes.IsPaid)
                {
                    sellernotes.SellingPrice = editNotesViewModel.SellingPrice;
                }
                else
                {
                    sellernotes.SellingPrice = 0;
                }
                sellernotes.ModifiedDate = DateTime.Now;
                sellernotes.ModifiedBy = user.ID;
                _dbcontext.SaveChanges();

                if (editNotesViewModel.DisplayPicture != null)
                {
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

                if (editNotesViewModel.NotesPreview != null)
                {
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
                    foreach(HttpPostedFileBase file in editNotesViewModel.UploadNotes)
                    {
                        if (file != null)
                        {
                            string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                            string notesattachementpath = "~/Members/" + user.ID + "/" + sellernotes.ID + "/Attachements/";
                            CreateDirectoryIfMissing(notesattachementpath);
                            string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath),notesattachementfilename);
                            file.SaveAs(notesattachementfilepath);
                            SellerNotesAttachement sellerNotesAttachement = new SellerNotesAttachement
                            {
                                NoteID = sellernotes.ID,
                                FileName = notesattachementfilename,
                                FilePath = notesattachementfilepath,
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
                _dbcontext.Dispose();

                return RedirectToAction("Index", "SellYourNotes");
            }
            else
            {
                EditNotesViewModel viewModel = new EditNotesViewModel
                {
                    NoteCategoryList = _dbcontext.NotesCategories.ToList(),
                    NoteTypeList = _dbcontext.NotesTypes.ToList(),
                    CountryList = _dbcontext.Countries.ToList()
                };

                return View(viewModel);
            }
        }
        private void CreateDirectoryIfMissing(string folderpath)
        {
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }

        [Authorize]
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

            //Note Seller and user is Same
            if (note.SellerID == user.ID)
            {
                _dbcontext.SellerNotes.Attach(note);
                note.Status = 9;
                note.PublishedDate = DateTime.Now;
                note.ModifiedDate = DateTime.Now;
                note.ModifiedBy = user.ID;
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("Index", "SellYourNotes");
        }
    }
}