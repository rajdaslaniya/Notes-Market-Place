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


namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Notes")]
    public class NotesController : Controller
    {
        NotesMarketPlaceEntities _dbcontext = new NotesMarketPlaceEntities();
        
       [Route("Details")]
        public ActionResult Details(int id)
        {
            var NoteDetail = _dbcontext.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            var NoteAttachment = _dbcontext.SellerNotesAttachements.Where(x => x.NoteID == NoteDetail.ID).FirstOrDefault();
            var users = _dbcontext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            DetailsViewModel detailsView = new DetailsViewModel {
               sellernotes=NoteDetail,
               sellernotesattachment=NoteAttachment
            };

            return View(detailsView);
        }

        [Authorize]
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
                var downloadfreenote = _dbcontext.Downloads.Where(x=>x.NoteID==noteid && x.Downloader==user.ID && x.IsSellerHasAllowedDownload==true && x.AttachmentPath!=null).FirstOrDefault();
                if (downloadfreenote == null) {
                    //Create download Object
                    Download download = new Download
                    {
                        NoteID = note.ID,
                        Seller = note.SellerID,
                        Downloader = user.ID,
                        IsSellerHasAllowedDownload = true,
                        AttachmentPath = attachment.FilePath,
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
                var downloadpaidnote = _dbcontext.Downloads.Where(x => x.NoteID == noteid && x.Downloader == user.ID && x.IsSellerHasAllowedDownload == true && x.AttachmentPath != null).FirstOrDefault();

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

        [Authorize]
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


            return RedirectToAction("Notes",new { id=note.ID});
        }

        private void BuildEmailTemplateForBuyerRequest(Download download,User user)
        {
            //Find Name from seller id 
            var seller = _dbcontext.Users.Where(x => x.ID == download.Seller).FirstOrDefault();
            //Read All Context from Email Template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "BuyerRequest" + ".cshtml");

            //SellerName Replace in Template
            body = body.Replace("@ViewBag.SellerName",seller.FirstName);
            //Buyer Name Replace in Template
            body = body.Replace("@ViewBag.BuyerName", user.FirstName);
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