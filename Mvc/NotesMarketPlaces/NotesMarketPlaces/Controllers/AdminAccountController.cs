using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using NotesMarketPlaces.ViewModels;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Admin")]
    [OutputCache(Duration = 0)]
    public class AdminAccountController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();

       [HttpGet]
       [Authorize(Roles ="Admin,SuperAdmin")]
       [Route("Admin/Profile")]
       public ActionResult UpdateProfile()
        {
            UpdateAdminProfileViewModel update = new UpdateAdminProfileViewModel();
            //Get Logged in user
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            update.Email = user.EmailID;
            update.FirstName = user.FirstName;
            update.LastName = user.LastName;
            //If user alredy fill this form
            var userprofile = _dbContext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();
            if (userprofile != null)
            {
                update.SecondryEmail = userprofile.SecondryEmailAddress;
                update.PhoneCode = userprofile.PhoneNumberCountryCode;
                update.PhoneNumber = userprofile.PhoneNumber;
                update.Picture = userprofile.ProfilePicture;
                update.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x=>x.CountryCode).ToList();
            }
            else
            {
                update.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x=>x.CountryCode).ToList();
            }

            return View(update);
        }
        [HttpPost]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Profile")]
        public ActionResult UpdateProfile(UpdateAdminProfileViewModel update)
        {
            //Check if Secondry Email is Alredy or not
            var alreadysec = _dbContext.UserProfiles.Where(x => x.SecondryEmailAddress == update.SecondryEmail).FirstOrDefault();
            var alredyuser = _dbContext.Users.Where(x => x.EmailID == update.SecondryEmail).FirstOrDefault();
            if (alreadysec != null&&alredyuser!=null)
            {
                ModelState.AddModelError("SecondryEmail","This email is already exist");
                update.PhoneCodeList= _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
                return View(update);
            }
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                //Get Data from Form
                user.FirstName = update.FirstName;
                user.LastName = update.LastName;
                user.ModifiedBy = user.ID;
                user.ModifiedDate = DateTime.Now;
                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();

                //check already data in profile table or not
                var userprofile = _dbContext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();
                if (userprofile != null)
                {
                    userprofile.PhoneNumber = update.PhoneNumber;
                    userprofile.PhoneNumberCountryCode = update.PhoneCode;
                    userprofile.SecondryEmailAddress = update.SecondryEmail;
                    userprofile.ModifiedBy = user.ID;
                    userprofile.ModifiedDate = DateTime.Now;

                    if (update.ProfilePicture != null)
                    {
                        if (userprofile.ProfilePicture != null)
                        {
                            string pathe = Server.MapPath(userprofile.ProfilePicture);
                            FileInfo file = new FileInfo(pathe);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }
                        string filename = System.IO.Path.GetFileName(update.ProfilePicture.FileName);
                        string fileextension = System.IO.Path.GetExtension(update.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + user.ID + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        userprofile.ProfilePicture = profilepicturepath + newfilename;
                        update.ProfilePicture.SaveAs(path);
                    }
                    
                    _dbContext.Entry(userprofile).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                else
                {
                    var users = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                    UserProfile userProfile = new UserProfile {
                        PhoneNumber = update.PhoneNumber,
                        PhoneNumberCountryCode = update.PhoneCode,
                        SecondryEmailAddress = update.SecondryEmail,
                        UserID=user.ID,
                        CreatedBy = users.ID,
                        CreatedDate=DateTime.Now,
                        AddressLine1 = "NA",
                        AddressLine2 = "NA",
                        City = "NA",
                        State = "NA",
                        Country = "NA",
                        ZipCode="NA"
                };
                   
                    if (update.ProfilePicture != null)
                    {
                        string filename = System.IO.Path.GetFileName(update.ProfilePicture.FileName);
                        string fileextension = System.IO.Path.GetExtension(update.ProfilePicture.FileName);
                        string newfilename = "DP_" + DateTime.Now.ToString("ddMMyyyy_hhmmss") + fileextension;
                        string profilepicturepath = "~/Members/" + user.ID + "/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        userProfile.ProfilePicture = profilepicturepath + newfilename;
                        update.ProfilePicture.SaveAs(path);
                    }
                    _dbContext.UserProfiles.Add(userProfile);
                    _dbContext.SaveChanges();
                }
                return RedirectToAction("Dashboard","Admin");
            }
            else
            {
                update.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).ToList();
                return View(update);
            }

        }
        private void CreateDirectoryIfMissing(string folderpath)
        {
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }
    }
}