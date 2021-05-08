using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    
    [RoutePrefix("Admin")]
    [OutputCache(Duration = 0)]
    public class AdminReportsController : Controller
    {

        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();

        // GET: AdminReports
        [HttpGet]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Route("SpamReports")]
        public ActionResult SpamReports(string search,string sortby,string sortorder,int page=1)
        {
            ViewBag.SpamRepotsSearch = search;
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;

            IEnumerable<AdminSpamRecordsViewModel> spamlist;
            spamlist = from reports in _dbContext.SellerNotesReportedIssues
                       join creatby in _dbContext.Users on reports.ReportedBYID equals creatby.ID
                       join note in _dbContext.SellerNotes on reports.NoteID equals note.ID
                       where note.IsActive == true
                       select new AdminSpamRecordsViewModel { 
                           Category=note.NotesCategory.Name,
                           NoteID=note.ID,
                           NoteTitle=note.Title,
                           Remark=reports.Remark,
                           ReportedBy=creatby.FirstName+" "+creatby.LastName,
                           SpamID=reports.ID,
                           DateEdited=reports.CreatedDate
                       };
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                spamlist = spamlist.Where(x => x.NoteTitle.ToLower().Contains(search) ||
                                             x.Remark.ToLower().Contains(search) ||
                                             x.ReportedBy.ToLower().Contains(search) ||
                                             x.Category.ToLower().Contains(search) ||
                                             x.NoteTitle.ToLower().Contains(search)||
                                             x.DateEdited.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search)).ToList();
            }
            //Count total page
            ViewBag.TotalPages = Math.Ceiling(spamlist.Count() / 5.0);
            spamlist = spamlist.Skip((page - 1) * 5).Take(5);
            //Going for sorting
            spamlist = SortTableSpamReports(sortby, sortorder, spamlist);

            return View(spamlist);
        }

        private IEnumerable<AdminSpamRecordsViewModel> SortTableSpamReports(string sortby, string sortorder, IEnumerable<AdminSpamRecordsViewModel> table)
        {
            switch (sortby)
            {
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
                case "ReportedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.ReportedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.ReportedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.ReportedBy);
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
            }
        }
        [Authorize(Roles ="SuperAdmin,Admin")]
        [HttpGet]
        [Route("SpamReports/Delete/{id}")]
        public ActionResult DeleteSpam(int id)
        {
            //Find spam note in database
            var spam = _dbContext.SellerNotesReportedIssues.Where(x => x.ID == id).FirstOrDefault();
            if (spam == null)
            {
                return HttpNotFound();
            }
            _dbContext.SellerNotesReportedIssues.Remove(spam);
            _dbContext.SaveChanges();
            return RedirectToAction("SpamReports");
        }
    }
}