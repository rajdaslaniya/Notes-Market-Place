using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    
    [RoutePrefix("Admin")]
    [OutputCache(Duration = 0)]
    public class AdminNoteController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContecxt = new NotesMarketPlaceEntities1();

        // GET: AdminNote
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Note/UnderReviewNotes")]
        public ActionResult NotesUnderReview(int? member,int? sellername, string search, string sortby, string sortorder, int page = 1)
        {
            //Active Navigation
            ViewBag.NotesUnderReview = "active";

            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;
            ViewBag.SellerName = sellername;
            ViewBag.PageNumber = page;
            ViewBag.UnderReviewSearch = search;

            //Check if user is get logged in or not
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name && x.RoleID == 2 || x.RoleID == 3).FirstOrDefault();

            IEnumerable<UnderReviewViewModel> notelist;

            if (member != null)
            {
                notelist = from note in _dbContecxt.SellerNotes
                           join users in _dbContecxt.Users on note.SellerID equals users.ID
                           where (note.Status == 7 || note.Status == 8) && note.IsActive == true && users.ID == member
                           select new UnderReviewViewModel
                           {
                               NoteID = note.ID,
                               AddedDate = note.CreatedDate,
                               NoteCategory = note.NotesCategory.Name,
                               SellerID = note.SellerID,
                               NoteTitle = note.Title,
                               Seller = user.FirstName + " " + user.LastName,
                               Status = note.Status == 7 ? "Submited For Review" : "InReview",
                               UserID = user.ID
                         };
                ViewBag.SellerList = notelist.Select(x => new { Value = x.SellerID, Text = x.Seller }).Distinct().ToList();
            }
            else
            {
                notelist = from note in _dbContecxt.SellerNotes
                           join users in _dbContecxt.Users on note.SellerID equals users.ID
                           where note.Status == 7 || note.Status == 8 && note.IsActive == true
                           select new UnderReviewViewModel
                           {
                               NoteID = note.ID,
                               AddedDate = note.CreatedDate,
                               NoteCategory = note.NotesCategory.Name,
                               SellerID = note.SellerID,
                               NoteTitle = note.Title,
                               Seller = user.FirstName + " " + user.LastName,
                               Status = note.Status == 7 ? "Submited For Review" : "InReview",
                               UserID = user.ID
                           };
                ViewBag.SellerList = notelist.Select(x => new { Value = x.SellerID, Text = x.Seller }).Distinct().ToList();
            }

            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notelist = notelist.Where(x => x.Seller.ToLower().Contains(search) ||
                                               x.NoteTitle.ToLower().Contains(search) ||
                                               x.NoteCategory.ToLower().Contains(search) ||
                                               x.AddedDate.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search)
                                               ).ToList();
            }
            if (sellername != null)
            {
                notelist = notelist.Where(x => x.SellerID == sellername).ToList(); ;
            }
            //Sending for sorting
            notelist = SortTableNotesUnderReview(sortby, sortorder, notelist);
            //Count a total page
            ViewBag.TotalPages = Math.Ceiling(notelist.Count() / 5.0);
            //Skip  Page Number
            notelist = notelist.Skip((page - 1) * 5).Take(5);
            return View(notelist);
        }

        private IEnumerable<UnderReviewViewModel> SortTableNotesUnderReview(string sortby, string sortorder, IEnumerable<UnderReviewViewModel> table)
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
                case "Category":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NoteCategory);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NoteCategory);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NoteCategory);
                                    return table;
                                }
                        }
                    }
                case "Seller":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Seller);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                        }
                    }

                case "DateAdded":
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

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Note/RejectNote")]
        public ActionResult RejectNote(FormCollection form)
        {
            int noteid = Convert.ToInt32(form["noteid"]);
            string remark = form["Remark"];
            if (remark == null)
            {
                return RedirectToAction("NotesUnderReview");
            }
            //Find a note in  seller notes
            var note = _dbContecxt.SellerNotes.Where(x => x.ID == noteid && x.Status == 7 || x.Status == 8).FirstOrDefault();

            //Get user logged in user
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name && (x.RoleID == 2 || x.RoleID == 1)).FirstOrDefault();
            if (note == null)
            {
                return HttpNotFound();
            }
            note.Status = 10;
            note.ActionedBy = user.ID;
            note.AdminRemarks = remark;
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            _dbContecxt.Entry(note).State = EntityState.Modified;
            _dbContecxt.SaveChanges();

            return RedirectToAction("NotesUnderReview");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [Route("Note/ApproveNote")]
        public ActionResult Approve(FormCollection form)
        {
            int noteid = Convert.ToInt32(form["noteid"]);
            //Get Logged in user
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            //Find note in database
            var note = _dbContecxt.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
            if (note == null)
            {
                return HttpNotFound();
            }
            note.Status = 9;
            note.AdminRemarks = "This Note is published";
            note.ActionedBy = user.ID;
            note.PublishedDate = DateTime.Now;
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            _dbContecxt.Entry(note).State = EntityState.Modified;
            _dbContecxt.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        [Route("Note/InReview")]
        public ActionResult InReview(FormCollection form)
        {
            int noteid = Convert.ToInt32(form["noteid"]);
            //Get Logged in user
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            //Find note in database
            var note = _dbContecxt.SellerNotes.Where(x => x.ID == noteid && x.Status == 7 || x.Status == 8).FirstOrDefault();
            if (note == null)
            {
                return HttpNotFound();
            }
            note.Status = 8;
            note.AdminRemarks = "This is inreview";
            note.ActionedBy = user.ID;
            note.ModifiedBy = user.ID;
            note.ModifiedDate = DateTime.Now;
            _dbContecxt.Entry(note).State = EntityState.Modified;
            _dbContecxt.SaveChanges();
            return RedirectToAction("NotesUnderReview");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Note/PublishedNotes")]
        public ActionResult PublishedNote(int? member,int? sellername, string search, string sortby, string sortorder, int page = 1)
        {
            //Active Navigation
            ViewBag.PublishedNotes = "active";

            ViewBag.PublishSearch = search;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;
            ViewBag.SellerName = sellername;
            ViewBag.PageNumber = page;
            //Check if user is get logged in or not
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name && x.RoleID == 2 || x.RoleID == 3).FirstOrDefault();


            IEnumerable<PublishedNotesViewModel> notelist;
            if (member != null)
            {
                notelist = from seller in _dbContecxt.SellerNotes
                           join usr in _dbContecxt.Users on seller.SellerID equals usr.ID
                           join usrs in _dbContecxt.Users on seller.ActionedBy equals usrs.ID
                           join number in _dbContecxt.Downloads on seller.ID equals number.NoteID into downloadlist
                           where seller.Status == 9 && seller.IsActive == true && usr.ID==member
                           select new PublishedNotesViewModel
                           {
                               NoteId = seller.ID,
                               ApprovedBy = usrs.FirstName + " " + usrs.LastName,
                               Category = seller.NotesCategory.Name,
                               Price = seller.SellingPrice,
                               PublishedDate = seller.PublishedDate,
                               Seller = usr.FirstName + " " + usr.LastName,
                               SellerID = seller.SellerID,
                               SellType = seller.IsPaid == true ? "Paid" : "False",
                               Title = seller.Title,
                               NumberOfDownloads = downloadlist.Where(x => x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Count()
                           };
                ViewBag.SellerList = notelist.Select(x => new { Value = x.SellerID, Text = x.Seller }).Distinct().ToList();

            }
            else
            {
                notelist = from seller in _dbContecxt.SellerNotes
                           join usr in _dbContecxt.Users on seller.SellerID equals usr.ID
                           join usrs in _dbContecxt.Users on seller.ActionedBy equals usrs.ID
                           join number in _dbContecxt.Downloads on seller.ID equals number.NoteID into downloadlist
                           where seller.Status == 9 && seller.IsActive == true
                           select new PublishedNotesViewModel
                           {
                               NoteId = seller.ID,
                               ApprovedBy = usrs.FirstName + " " + usrs.LastName,
                               Category = seller.NotesCategory.Name,
                               Price = seller.SellingPrice,
                               PublishedDate = seller.PublishedDate,
                               Seller = usr.FirstName + " " + usr.LastName,
                               SellerID = seller.SellerID,
                               SellType = seller.IsPaid == true ? "Paid" : "False",
                               Title = seller.Title,
                               NumberOfDownloads = downloadlist.Where(x => x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Count()
                           };
                ViewBag.SellerList = notelist.Select(x => new { Value = x.SellerID, Text = x.Seller }).Distinct().ToList();

            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notelist = notelist.Where(x => x.Category.ToLower().Contains(search) ||
                                                x.Title.ToLower().Contains(search) ||
                                                x.SellType.ToLower().Contains(search) ||
                                                x.Seller.ToLower().Contains(search) ||
                                                x.PublishedDate.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                x.Price.ToString().Contains(search) ||
                                                x.ApprovedBy.ToLower().Contains(search)
                                                ).ToList();
            }
            if (sellername != null)
            {
                notelist = notelist.Where(x => x.SellerID == sellername).ToList();
            }
            //Sending for sorting
            notelist = SortTablePublish(sortby, sortorder, notelist);
            //Count a total page
            ViewBag.TotalPages = Math.Ceiling(notelist.Count() / 5.0);
            //Skip  Page Number
            notelist = notelist.Skip((page - 1) * 5).Take(5);
            return View(notelist);
        }

        private IEnumerable<PublishedNotesViewModel> SortTablePublish(string sortby, string sortorder, IEnumerable<PublishedNotesViewModel> table)
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
                case "Seller":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Seller);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                        }
                    }

                case "DateAdded":
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
                case "ApprovedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.ApprovedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.ApprovedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.ApprovedBy);
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
                case "Downloads":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NumberOfDownloads);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NumberOfDownloads);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NumberOfDownloads);
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


        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Note/DownloadedNotes")]
        public ActionResult AdminDownload(int? selectednote, int? member, int? note, int? seller, int? buyer, string search, string sortorder,string sortby, int page = 1)
        {
            ViewBag.DownloadNotes = "active";
            // viewbag for searching, sorting and pagination
            ViewBag.Search = search;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrdet = sortorder;
            ViewBag.PageNumber = page;
            ViewBag.Seller = seller;
            ViewBag.Buyer = buyer;
            ViewBag.Note = note;

            IEnumerable<AdminDownloadViewModel> notelist;

            // get downloaded notes for specific notes only
            if (selectednote != null)
            {
                ViewBag.SelectedNote = selectednote;
                ViewBag.Note = selectednote;

                notelist = from downloads in _dbContecxt.Downloads
                           join sellers in _dbContecxt.Users on downloads.Seller equals sellers.ID
                           join buyers in _dbContecxt.Users on downloads.Downloader equals buyers.ID
                           where downloads.IsSellerHasAllowedDownload == true && downloads.AttachmentPath != null && downloads.NoteID == selectednote
                           select new AdminDownloadViewModel
                           {
                               NoteID = downloads.NoteID,
                               SellerID = sellers.ID,
                               BuyerID = buyers.ID,
                               NoteTitle = downloads.NoteTitle,
                               Category = downloads.NoteCategory,
                               Buyer = buyers.FirstName + " " + buyers.LastName,
                               Seller = sellers.FirstName + " " + sellers.LastName,
                               SellType = downloads.IsPaid == true ? "Paid" : "Free",
                               Price = downloads.PurchasedPrice,
                               DownloadDate = downloads.CreatedDate.Value
                           };
            }
            // get downloaded notes for specific member only
            else if (member != null)
            {
                ViewBag.Member = member;
                ViewBag.Buyer = member;

                notelist = from downloads in _dbContecxt.Downloads
                           join sellers in _dbContecxt.Users on downloads.Seller equals sellers.ID
                           join buyers in _dbContecxt.Users on downloads.Downloader equals buyers.ID
                           where downloads.IsSellerHasAllowedDownload == true && downloads.AttachmentPath != null && downloads.Downloader == member
                           select new AdminDownloadViewModel
                           {
                               NoteID = downloads.NoteID,
                               SellerID = sellers.ID,
                               BuyerID = buyers.ID,
                               NoteTitle = downloads.NoteTitle,
                               Category = downloads.NoteCategory,
                               Buyer = buyers.FirstName + " " + buyers.LastName,
                               Seller = sellers.FirstName + " " + sellers.LastName,
                               SellType = downloads.IsPaid == true ? "Paid" : "Free",
                               Price = downloads.PurchasedPrice,
                               DownloadDate = downloads.CreatedDate.Value
                           };
            }
            // get all downloaded notes
            else
            {
                notelist = from downloads in _dbContecxt.Downloads
                           join sellers in _dbContecxt.Users on downloads.Seller equals sellers.ID
                           join buyers in _dbContecxt.Users on downloads.Downloader equals buyers.ID
                           where downloads.IsSellerHasAllowedDownload == true && downloads.AttachmentPath != null
                           select new AdminDownloadViewModel
                           {
                               NoteID = downloads.NoteID,
                               SellerID = sellers.ID,
                               BuyerID = buyers.ID,
                               NoteTitle = downloads.NoteTitle,
                               Category = downloads.NoteCategory,
                               Buyer = buyers.FirstName + " " + buyers.LastName,
                               Seller = sellers.FirstName + " " + sellers.LastName,
                               SellType = downloads.IsPaid == true ? "Paid" : "Free",
                               Price = downloads.PurchasedPrice,
                               DownloadDate = downloads.CreatedDate.Value
                           };
            }

            // get sellerlist
            ViewBag.SellerList = notelist.Select(x => new {
                Value = x.SellerID,
                Text = x.Seller
            }).Distinct().ToList();

            // get buyerlist
            ViewBag.BuyerList = notelist.Select(x => new {
                Value = x.BuyerID,
                Text = x.Buyer
            }).Distinct().ToList();

            // get notelist
            ViewBag.NoteList = notelist.Select(x => new {
                Value = x.NoteID,
                Text = x.NoteTitle
            }).Distinct().ToList();

            // filter result based on seller
            if (seller != null)
            {
                notelist = notelist.Where(x => x.SellerID == seller).ToList();
            }
            // filter result based on buyer
            if (buyer != null)
            {
                notelist = notelist.Where(x => x.BuyerID == buyer).ToList();
            }
            // filter result based on note
            if (note != null)
            {
                notelist = notelist.Where(x => x.NoteID == note).ToList();
            }

            // if search is not empty then get result based on search
            if (!String.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notelist = notelist.Where(x => x.NoteTitle.ToLower().Contains(search) ||
                                               x.Category.ToLower().Contains(search) ||
                                               x.Buyer.ToLower().Contains(search) ||
                                               x.Seller.ToLower().Contains(search) ||
                                               x.Price.ToString().Contains(search) ||
                                               x.SellType.ToString().Contains(search) ||
                                               x.DownloadDate.Value.ToString("dd-MM-yyyy hh:mm").Contains(search)
                                         ).ToList();
            }

            // sort result
            notelist = SortTableAdminDownload(sortby, sortorder, notelist);

            // gte totalpages
            ViewBag.TotalPages = Math.Ceiling(notelist.Count() / 5.0);

            // show result according to pagination
            notelist = notelist.Skip((page - 1) * 5).Take(5);

            return View(notelist);
        }

        private IEnumerable<AdminDownloadViewModel> SortTableAdminDownload(string sortby, string sortorder, IEnumerable<AdminDownloadViewModel> table)
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
                case "Seller":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Seller);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Seller);
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

        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Note/RejectedNotes")]
        public ActionResult AdminRejected(int? sellerid,string search,string sortby,string sortorder, int page = 1)
        {
            ViewBag.RejectedNotes = "active";
            ViewBag.RejectSearch = search;
            ViewBag.SortOrder = sortorder;
            ViewBag.SortBy = sortby;
            ViewBag.PageNumber = page;
            ViewBag.Seller = sellerid;

            //Get logged in user
            var user = _dbContecxt.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Create Object 
            IEnumerable<AdminRejectedNotesViewModel> notelist;
            notelist = from note in _dbContecxt.SellerNotes
                       join seller in _dbContecxt.Users on note.SellerID equals seller.ID
                       join rejected in _dbContecxt.Users on note.ActionedBy equals rejected.ID
                       where note.Status == 10 && note.IsActive == true 
                       select new AdminRejectedNotesViewModel {
                           Category = note.NotesCategory.Name,
                           DateEdited = note.ModifiedDate,
                           NoteID = note.ID,
                           NoteTitle = note.Title,
                           RejectedBy = rejected.FirstName + " " + rejected.LastName,
                           Remark=note.AdminRemarks,
                           Seller=seller.FirstName+" "+seller.LastName,
                           SellerID=seller.ID
                       };
            //Seller List
            ViewBag.SellerList = notelist.Select(x => new { Value=x.SellerID,Text=x.Seller}).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                notelist = notelist.Where(x => x.NoteTitle.ToLower().Contains(search)||
                                               x.RejectedBy.ToLower().Contains(search)||
                                               x.Remark.ToLower().Contains(search)||
                                               x.Seller.ToLower().Contains(search)||
                                               x.Category.ToLower().Contains(search)||
                                               x.DateEdited.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search)
                                               ).ToList();
            }

            if (sellerid != null)
            {
                notelist = notelist.Where(x => x.SellerID == sellerid).ToList();
            }
            //Going for sorting 
            notelist = SortTableAdminRejec(sortby, sortorder, notelist);

            //Count a total page
            ViewBag.TotalPages = Math.Ceiling(notelist.Count() / 5.0);

            //Skip Number
            notelist = notelist.Skip((page - 1) * 5).Take(5);
            return View(notelist);
        }

        private IEnumerable<AdminRejectedNotesViewModel> SortTableAdminRejec(string sortby, string sortorder, IEnumerable<AdminRejectedNotesViewModel> table)
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
                
                case "Seller":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Seller);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Seller);
                                    return table;
                                }
                        }
                    }

                case "DateEdited":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.DateEdited);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DateEdited);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DateEdited);
                                    return table;
                                }
                        }
                    }

               
                case "RejectedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.RejectedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.RejectedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.RejectedBy);
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
                                    table = table.OrderBy(x => x.Remark);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Remark);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Remark);
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
    }
}