using NotesMarketPlaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Users")]
    public class UsersController : Controller
    {
        readonly NotesMarketPlaceEntities _dbcontext = new NotesMarketPlaceEntities();

        // GET: Users
        [Authorize]
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
                                                                      from notepreview in rev.DefaultIfEmpty()
                                                                      where download.Downloader == user.ID && download.IsSellerHasAllowedDownload == true && download.AttachmentPath!=null
                                                                      select new MyDownloadsViewModel { tblDownload=download,tblUser=user,tblReview= notepreview };

            //Search For in table
            if (!string.IsNullOrEmpty(mydownloadsearch))
            {
                mydownloadsearch = mydownloadsearch.ToLower();
                myDownloads = myDownloads.Where(x => x.tblDownload.NoteTitle.ToLower().Contains(mydownloadsearch)||
                                                x.tblDownload.NoteCategory.ToLower().Contains(mydownloadsearch)||
                                                x.tblUser.EmailID.ToLower().Contains(mydownloadsearch)||
                                                x.tblDownload.PurchasedPrice.ToString().ToLower().Contains(mydownloadsearch));
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
                                    table = table.OrderBy(x => x.tblDownload.NoteTitle);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.tblDownload.NoteTitle);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.tblDownload.NoteTitle);
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
                                    table = table.OrderBy(x => x.tblDownload.NoteTitle);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.tblDownload.NoteTitle);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.tblDownload.NoteTitle);
                                    return table;
                                }
                        }
                    }
            }
        }
    }
}