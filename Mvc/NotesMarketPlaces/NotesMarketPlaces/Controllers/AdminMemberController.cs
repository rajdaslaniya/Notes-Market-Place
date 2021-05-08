using NotesMarketPlaces.Models;
using NotesMarketPlaces.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlaces.Controllers
{
    
    [RoutePrefix("Admin")]
    [OutputCache(Duration = 0)]
    public class AdminMemberController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        [Route("Member")]
        // GET: Member
        public ActionResult Member(string search, string sortby, string sortorder, int page = 1)
        {
            ViewBag.MemberSearch = search;
            ViewBag.Members = "active";
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;
            IEnumerable<MemberViewModel> memberlist;

            memberlist = from user in _dbContext.Users
                         join note in _dbContext.SellerNotes on user.ID equals note.SellerID into notelist
                         join download in _dbContext.Downloads on user.ID equals download.Downloader into downloadlist
                         join seller in _dbContext.Downloads on user.ID equals seller.Seller into sellerlist
                         where user.IsActive == true && user.RoleID == 3
                         orderby user.CreatedDate descending
                         select new MemberViewModel
                         {
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             Email = user.EmailID,
                             JoiningDate = user.CreatedDate,
                             RegisterID = user.ID,
                             PublishedNotes = notelist.Where(x => x.Status == 9 && x.IsActive == true).Count(),
                             NotesUnderReview = notelist.Where(x => x.IsActive == true && (x.Status == 7 || x.Status == 8)).Count(),
                             DownloadedNote = downloadlist.Where(x => x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).Count(),
                             TotalEarning = sellerlist.Where(x => x.IsSellerHasAllowedDownload == true).Select(x => x.PurchasedPrice).Sum(),
                             TotalExpensis = sellerlist.Where(x => x.IsSellerHasAllowedDownload == true).Select(x => x.PurchasedPrice).Sum()
                         };
            //Search Data in list
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                memberlist = memberlist.Where(x => x.Email.ToLower().Contains(search) ||
                                                 x.DownloadedNote.ToString().Contains(search) ||
                                                 x.JoiningDate.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                 x.PublishedNotes.ToString().Contains(search) ||
                                                 x.NotesUnderReview.ToString().Contains(search) ||
                                                 x.TotalEarning.ToString().Contains(search) ||
                                                 x.TotalExpensis.ToString().Contains(search) ||
                                                 x.FirstName.ToLower().ToString().Contains(search) ||
                                                 x.LastName.ToLower().ToString().Contains(search)).ToList();
            }
            //Going for sorting
            memberlist = SortTableNotesUnderReview(sortby, sortorder, memberlist);
            //Total Pages
            ViewBag.TotalPages = Math.Ceiling(memberlist.Count() / 5.0);
            //Skip Page Number
            memberlist = memberlist.Skip((page - 1) * 5).Take(5);
            return View(memberlist);
        }
        private IEnumerable<MemberViewModel> SortTableNotesUnderReview(string sortby, string sortorder, IEnumerable<MemberViewModel> table)
        {
            switch (sortby)
            {
                case "FirstName":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.FirstName);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.FirstName);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.FirstName);
                                    return table;
                                }
                        }
                    }
                case "LastName":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.LastName);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.LastName);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.LastName);
                                    return table;
                                }
                        }
                    }
                case "Email":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Email);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Email);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Email);
                                    return table;
                                }
                        }
                    }

                case "JoiningDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.JoiningDate);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.JoiningDate);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.JoiningDate);
                                    return table;
                                }
                        }
                    }
                case "UnderReviewNotes":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.NotesUnderReview);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.NotesUnderReview);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.NotesUnderReview);
                                    return table;
                                }
                        }
                    }
                case "PublishedNotes":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.PublishedNotes);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.PublishedNotes);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.PublishedNotes);
                                    return table;
                                }
                        }
                    }
                case "DownloadedNotes":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.DownloadedNote);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DownloadedNote);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DownloadedNote);
                                    return table;
                                }
                        }
                    }
                case "TotalExpenses":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.TotalExpensis);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.TotalExpensis);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.TotalExpensis);
                                    return table;
                                }
                        }
                    }
                case "TotalEarning":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.TotalEarning);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.TotalEarning);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.TotalEarning);
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
                                    table = table.OrderBy(x => x.FirstName);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.FirstName);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.FirstName);
                                    return table;
                                }
                        }
                    }
            }
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("Member/Deactive/{memberid}")]
        public ActionResult DeactiveMember(int memberid)
        {
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            //find a Member of depends on id
            var register = _dbContext.Users.Where(x => x.ID == memberid).FirstOrDefault();

            //Change Status isActive true to false
            register.IsActive = false;
            register.ModifiedBy = user.ID;
            register.ModifiedDate = DateTime.Now;
            _dbContext.Entry(register).State = EntityState.Modified;
            _dbContext.SaveChanges();

            //Find a Register Note in Seller
            var note = _dbContext.SellerNotes.Where(x => x.SellerID == register.ID && x.Status == 9).ToList();
            foreach (var item in note)
            {
                item.Status = 11;
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = user.ID;
                _dbContext.Entry(item).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Member", "Member");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [Route("Member/{memberid}")]
        public ActionResult MemberDetails(int memberid,string sortby,string sortorder,int page=1)
        {
            ViewBag.PageNumber = page;
            ViewBag.SortOrder = sortorder;
            ViewBag.SortBy = sortby;

            //Find a detail of user
            var user = _dbContext.Users.Where(x => x.ID == memberid).FirstOrDefault();
            //Get more details of user
            var userprofile = _dbContext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();

            MemberDetailsViewModel member = new MemberDetailsViewModel();
            member.FirstName = user.FirstName;
            member.LastName = user.LastName;
            member.Email = user.EmailID;
            member.PhoneNumber = userprofile.PhoneNumber;
            member.RegisterID = user.ID;
            member.DOB = userprofile.DOB;
            member.DisplayImage = userprofile.ProfilePicture;
            member.Address1 = userprofile.AddressLine1;
            member.Address2 = userprofile.AddressLine2;
            member.City = userprofile.City;
            member.College = userprofile.University;
            member.Country = userprofile.Country;
            member.ZipCode = userprofile.ZipCode;
            member.State = userprofile.State;

            member.Note = from note in _dbContext.SellerNotes
                          join download in _dbContext.Downloads on note.ID equals download.NoteID into down
                           where (note.Status == 8 || note.Status == 9) && note.IsActive == true && note.SellerID == memberid
                           select new Notes {
                               NoteID = note.ID,
                               Category = note.NotesCategory.Name,
                               DateAdded = note.CreatedDate,
                               NoteTitle = note.Title,
                               PublishedDate = note.PublishedDate,
                               Status = note.Status == 8 ? "InReview" : "Published",
                               TotalEarning = (int)down.Where(x=>x.Seller==memberid).Select(x => x.PurchasedPrice).Sum(),
                               DownloadedNotes = down.Where(x=>x.Seller==memberid).Select(x=>x.ID).Count()
                           };

            //Count Total Page
            ViewBag.TotalPages = Math.Ceiling(member.Note.Count()/5.0);
            //Skip a Number
            member.Note = member.Note.Skip((page - 1) * 5).Take(5);


            //Going for sorting 
            member.Note = SortTableMemberDetails(sortby, sortorder, member.Note);

            return View(member);
        }

        private IEnumerable<Notes> SortTableMemberDetails(string sortby, string sortorder, IEnumerable<Notes> table)
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
                
                case "TotalEarning":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.TotalEarning);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.TotalEarning);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.TotalEarning);
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
                case "DownloadedNotes":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.DownloadedNotes);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DownloadedNotes);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DownloadedNotes);
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
                                    table = table.OrderBy(x => x.DateAdded);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.DateAdded);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.DateAdded);
                                    return table;
                                }
                        }
                    }
                case "PublishDate":
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