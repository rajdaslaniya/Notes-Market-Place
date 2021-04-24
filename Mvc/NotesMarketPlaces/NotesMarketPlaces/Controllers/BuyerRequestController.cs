using NotesMarketPlaces.Models;
using NotesMarketPlaces.Send_Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    public class BuyerRequestController : Controller
    {
        readonly NotesMarketPlaceEntities1 _dbcontext = new NotesMarketPlaceEntities1();



        [Authorize(Roles = "Member")]
        [Route("BuyerRequest")]
        public ActionResult BuyerRequest(string search, string sortorder, string sortby,int page=1)
        {
            //Navigation Active 
            ViewBag.BuyerRequest = "active";
            //Use a White Navigation
            ViewBag.Navigation = "white-navbar";
            //Sorting ,searching and pagination
            ViewBag.PageNumber = page;
            ViewBag.BuyerSearch = search;
            ViewBag.SortOrder = sortorder;
            ViewBag.SortBy = sortby;

            //Get User is Logged in 
            User user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Get Buyer  Request
            IEnumerable<BuyerRequestViewModel> buyerRequest = from download in _dbcontext.Downloads
                                                               join users in _dbcontext.Users on download.Downloader equals users.ID
                                                               join userprofile in _dbcontext.UserProfiles on download.Downloader equals userprofile.UserID
                                                               where download.Seller == user.ID && download.IsSellerHasAllowedDownload == false && download.AttachmentPath==null
                                                               select new BuyerRequestViewModel { 
                                                                   Price=(int)download.PurchasedPrice,
                                                                   SellType = download.IsPaid == true ? "Paid" : "False",
                                                                   DownloadDate=download.CreatedDate,
                                                                   NoteID=download.NoteID,
                                                                   DownloadID=download.ID,
                                                                   NoteTitle=download.NoteTitle,
                                                                   Category=download.NoteCategory,
                                                                   Buyer=user.EmailID,
                                                                   PhoneNumber= "+"+userprofile.PhoneNumberCountryCode+userprofile.PhoneNumber,
                                                                   UserID=user.ID,
                                                               };

            //To Check Search string is null or not
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                buyerRequest = buyerRequest.Where(
                                                    x=>x.NoteTitle.ToLower().Contains(search)||
                                                    x.Category.ToLower().Contains(search)||
                                                    x.Buyer.ToLower().Contains(search)||
                                                    x.Price.ToString().Contains(search)||
                                                    x.DownloadDate.ToString().Contains(search)||
                                                    x.PhoneNumber.ToString().Contains(search)||
                                                    x.SellType.ToLower().Contains(search)
                                                 ).ToList();
            }
            //Sorting and searching 
            //Going for sorting
            buyerRequest = TableSortInBuyerRequest(sortorder, sortby, buyerRequest);

            //To find Number of Pages
            ViewBag.TotalPages = Math.Ceiling(buyerRequest.Count()/10.0);

            //SKip A number
            buyerRequest = buyerRequest.Skip((page - 1) * 10).Take(10);
            return View(buyerRequest);
        }

        private IEnumerable<BuyerRequestViewModel> TableSortInBuyerRequest(string sortorder, string sortby, IEnumerable<BuyerRequestViewModel> table)
        {
            switch (sortby)
            {
                case "NoteTitle":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.NoteTitle);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.NoteTitle);
                                return table;
                            default:
                                table = table.OrderBy(x => x.NoteTitle);
                                return table;
                        }
                    }
                case "Category":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.Category);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.Category);
                                return table;
                            default:
                                table = table.OrderBy(x => x.Category);
                                return table;
                        }
                    }
                case "Buyer":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.Buyer);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.Buyer);
                                return table;
                            default:
                                table = table.OrderBy(x => x.Buyer);
                                return table;
                        }
                    }
                case "Price":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.Price);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.Price);
                                return table;
                            default:
                                table = table.OrderBy(x => x.Price);
                                return table;
                        }
                    }
                case "DownloadDate":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.DownloadDate);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.DownloadDate);
                                return table;
                            default:
                                table = table.OrderBy(x => x.DownloadDate);
                                return table;
                        }
                    }
                case "PhoneNumber":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.PhoneNumber);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.PhoneNumber);
                                return table;
                            default:
                                table = table.OrderBy(x => x.PhoneNumber);
                                return table;
                        }
                    }
                case "SellType":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.SellType);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.SellType);
                                return table;
                            default:
                                table = table.OrderBy(x => x.SellType);
                                return table;
                        }
                    }
                default:
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                table = table.OrderBy(x => x.NoteTitle);
                                return table;
                            case "Desc":
                                table = table.OrderByDescending(x => x.NoteTitle);
                                return table;
                            default:
                                table = table.OrderBy(x => x.NoteTitle);
                                return table;
                        }
                    }
            }
        }

        [Authorize(Roles = "Member")]
        [Route("BuyerRequest/AllowDownload/{id}")]
        public ActionResult AllowDownload(int id)
        {
            User user = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            Download download = _dbcontext.Downloads.Find(id);
            if (user.ID == download.Seller)
            {
                //Get Sellernotes attachment object
                SellerNotesAttachement attachement = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == download.NoteID && x.IsActive == true).FirstOrDefault();
                
                //Update data in download table
                _dbcontext.Downloads.Attach(download);
                download.IsSellerHasAllowedDownload = true;
                download.AttachmentPath = attachement.FilePath;
                download.ModifiedBy = user.ID;
                download.ModifiedDate = DateTime.Now;
                download.IsAttachmentDownloaded = true;
                _dbcontext.SaveChanges();
                BuildEmailTemplate(download,user);
                return RedirectToAction("BuyerRequest");
            }
            else
            {
                return RedirectToAction("BuyerRequest");
            }
        }

        //To send Email for Allow Download
        private void BuildEmailTemplate(Download download, User user)
        {
            //Find Name from download User
            var seller = _dbcontext.Users.Where(x => x.ID == download.Seller).FirstOrDefault();
            //Read All Context from Email Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "AllowDownload" + ".cshtml");
            //Seller Name Replace in EmailTemplate
            body = body.Replace("ViewBag.SellerName", seller.FirstName);
            //Buyer Name Replace in Email Template
            body = body.Replace("ViewBag.BuyerName",user.FirstName);
            body = body.ToString();
            //Get Support Email
            var fromemail = _dbcontext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();
            //Mail From 
            string from, subject, to;
            from = fromemail.Value.Trim();

            subject = user.FirstName + " Allows you to download a note";
            to = seller.EmailID.Trim();

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

            //Send Mail
            SendingMail.SendEmail(mail);
        }
    }
}