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
    public class AdminSettingsController : Controller
    {
        readonly private NotesMarketPlaceEntities1 _dbContext = new NotesMarketPlaceEntities1();

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("Settings/ManageSystemConfiguration")]
        public ActionResult ManageSystem()
        {
            // create object of SystemConfigurationViewModel
            ManageSystemViewModel systemConfiguration = new ManageSystemViewModel();

            // get supportemail
            var supportemail = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "SupportEmail").FirstOrDefault();
            if (supportemail != null)
            {
                systemConfiguration.SupportEmail = supportemail.Value;
            }

            // get supportcontact
            var supportcontact = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "supportcontact").FirstOrDefault();
            if (supportcontact != null)
            {
                systemConfiguration.SupportContact = supportcontact.Value;
            }

            // get notifyemail
            var notifyemail = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "notifyemail").FirstOrDefault();
            if (notifyemail != null)
            {
                systemConfiguration.NotifyEmail = notifyemail.Value;
            }

            // get facebookurl
            var facebookurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "facebookurl").FirstOrDefault();
            if (facebookurl != null)
            {
                systemConfiguration.FacebookURL = facebookurl.Value;
            }

            // get twitterurl
            var twitterurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "twitterurl").FirstOrDefault();
            if (twitterurl != null)
            {
                systemConfiguration.TwitterURL = twitterurl.Value;
            }

            // get linkedinurl
            var linkedinurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "linkedinurl").FirstOrDefault();
            if (linkedinurl != null)
            {
                systemConfiguration.LinkedInURL = linkedinurl.Value;
            }

            // get defaultprofile
            var defaultprofile = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "defaultprofile").FirstOrDefault();
            if (defaultprofile != null)
            {
                systemConfiguration.DefaultProfileURL = defaultprofile.Value;
            }

            // get defaultnote
            var defaultnote = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "defaultnote").FirstOrDefault();
            if (defaultnote != null)
            {
                systemConfiguration.DefaultNoteURL = defaultnote.Value;
            }

            return View(systemConfiguration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        [Route("Settings/ManageSystem")]
        public ActionResult ManageSystem(ManageSystemViewModel obj)
        {
            // check if default note picture is available or not
            if (obj.DefaultNote == null && obj.DefaultNoteURL == null)
            {
                ModelState.AddModelError("DefaultNote", "Default note picture is required");
                return View(obj);
            }

            // check if default profile picture is available or not
            if (obj.DefaultProfile == null && obj.DefaultProfileURL == null)
            {
                ModelState.AddModelError("DefaultProfile", "Default profile picture is required");
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                // get logged in superadmin
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                // create object of SystemConfiguration
                SystemConfiguration systemConfiguration = new SystemConfiguration();

                // get supportemail
                var supportemail = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "supportemail").FirstOrDefault();
                // if supportemail is null then create
                if (supportemail == null)
                {
                    systemConfiguration.Key = "SupportEmail";
                    systemConfiguration.Value = obj.SupportEmail.Trim();
                    systemConfiguration.CreatedDate = DateTime.Now;
                    systemConfiguration.CreatedBy = superadmin.ID;
                    systemConfiguration.IsActive = true;

                    _dbContext.SystemConfigurations.Add(systemConfiguration);
                    _dbContext.SaveChanges();
                }
                // edit supportemail
                else
                {
                    if (!supportemail.Value.Equals(obj.SupportEmail))
                    {
                        supportemail.Value = obj.SupportEmail.Trim();
                        supportemail.ModifiedDate = DateTime.Now;
                        supportemail.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(supportemail).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get supportcontact
                var supportcontact = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "supportcontact").FirstOrDefault();
                // if supportcontact is null then create
                if (supportcontact == null)
                {
                    systemConfiguration.Key = "SupportContact";
                    systemConfiguration.Value = obj.SupportContact.Trim();
                    systemConfiguration.CreatedDate = DateTime.Now;
                    systemConfiguration.CreatedBy = superadmin.ID;
                    systemConfiguration.IsActive = true;

                    _dbContext.SystemConfigurations.Add(systemConfiguration);
                    _dbContext.SaveChanges();
                }
                // edit supportcontact
                else
                {
                    if (!supportcontact.Value.Equals(obj.SupportContact))
                    {
                        supportcontact.Value = obj.SupportContact.Trim();
                        supportcontact.ModifiedDate = DateTime.Now;
                        supportcontact.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(supportcontact).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get notifyemail
                var notifyemail = _dbContext.SystemConfigurations.Where(x => x.Value.ToLower() == "notifyemail").FirstOrDefault();
                // if notifyemail is null then create
                if (notifyemail == null)
                {
                    systemConfiguration.Key = "notifyemail";
                    systemConfiguration.Value = obj.NotifyEmail.Trim();
                    systemConfiguration.CreatedDate = DateTime.Now;
                    systemConfiguration.CreatedBy = superadmin.ID;
                    systemConfiguration.IsActive = true;

                    _dbContext.SystemConfigurations.Add(systemConfiguration);
                    _dbContext.SaveChanges();
                }
                // edit notifyemail
                else
                {
                    if (!notifyemail.Value.Equals(obj.NotifyEmail))
                    {
                        notifyemail.Value = obj.NotifyEmail.Trim();
                        notifyemail.ModifiedDate = DateTime.Now;
                        notifyemail.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(notifyemail).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get facebookurl
                var facebookurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "facebookurl").FirstOrDefault();
                // if facebookurl is null then create
                if (facebookurl == null)
                {
                    if (obj.FacebookURL != null)
                    {
                        systemConfiguration.Key = "facebookurl";
                        systemConfiguration.Value = obj.FacebookURL.Trim();
                        systemConfiguration.CreatedDate = DateTime.Now;
                        systemConfiguration.CreatedBy = superadmin.ID;
                        systemConfiguration.IsActive = true;

                        _dbContext.SystemConfigurations.Add(systemConfiguration);
                        _dbContext.SaveChanges();
                    }
                }
                // edit facebookurl
                else
                {
                    if (!facebookurl.Value.Equals(obj.FacebookURL))
                    {
                        facebookurl.Value = obj.FacebookURL.Trim();
                        facebookurl.ModifiedDate = DateTime.Now;
                        facebookurl.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(facebookurl).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get twitterurl
                var twitterurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "twitterurl").FirstOrDefault();
                // if twitterurl is null then create
                if (twitterurl == null)
                {
                    if (obj.TwitterURL != null)
                    {
                        systemConfiguration.Key = "twitterurl";
                        systemConfiguration.Value = obj.TwitterURL.Trim();
                        systemConfiguration.CreatedDate = DateTime.Now;
                        systemConfiguration.CreatedBy = superadmin.ID;
                        systemConfiguration.IsActive = true;

                        _dbContext.SystemConfigurations.Add(systemConfiguration);
                        _dbContext.SaveChanges();
                    }
                }
                // edit twitterurl
                else
                {
                    if (!twitterurl.Value.Equals(obj.TwitterURL))
                    {
                        twitterurl.Value = obj.TwitterURL.Trim();
                        twitterurl.ModifiedDate = DateTime.Now;
                        twitterurl.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(twitterurl).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get linkedinurl
                var linkedinurl = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "linkedinurl").FirstOrDefault();
                // if linkedinurl is null then create
                if (linkedinurl == null)
                {
                    if (obj.LinkedInURL != null)
                    {
                        systemConfiguration.Key = "linkedinurl";
                        systemConfiguration.Value = obj.LinkedInURL.Trim();
                        systemConfiguration.CreatedDate = DateTime.Now;
                        systemConfiguration.CreatedBy = superadmin.ID;
                        systemConfiguration.IsActive = true;

                        _dbContext.SystemConfigurations.Add(systemConfiguration);
                        _dbContext.SaveChanges();
                    }
                }
                // edit linkedinurl
                else
                {
                    if (!linkedinurl.Value.Equals(obj.LinkedInURL))
                    {
                        linkedinurl.Value = obj.LinkedInURL.Trim();
                        linkedinurl.ModifiedDate = DateTime.Now;
                        linkedinurl.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(linkedinurl).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get defaultprofile
                var defaultprofile = _dbContext.SystemConfigurations.Where(x => x.Value.ToLower() == "defaultprofile").FirstOrDefault();
                // if defaultprofile is null then create
                if (defaultprofile == null)
                {
                    systemConfiguration.Key = "defaultprofile";

                    if (obj.DefaultProfile != null)
                    {
                        string fileextension = System.IO.Path.GetExtension(obj.DefaultProfile.FileName);
                        string newfilename = "profile" + fileextension;
                        string profilepicturepath = "~/DefaultImage/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        obj.DefaultProfile.SaveAs(path);

                        systemConfiguration.Value = profilepicturepath + newfilename;
                    }

                    systemConfiguration.CreatedDate = DateTime.Now;
                    systemConfiguration.CreatedBy = superadmin.ID;
                    systemConfiguration.IsActive = true;

                    _dbContext.SystemConfigurations.Add(systemConfiguration);
                    _dbContext.SaveChanges();
                }
                // edit defaultprofile
                else
                {
                    // check if user uploaded defaultprofile
                    if (obj.DefaultProfile != null)
                    {
                        // check if there is already profile picture is available or not
                        // if available then we have to delete
                        if (defaultprofile.Value != null)
                        {
                            string previouspath = Server.MapPath(defaultprofile.Value);
                            FileInfo file = new FileInfo(previouspath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }

                        string fileextension = System.IO.Path.GetExtension(obj.DefaultProfile.FileName);
                        string newfilename = "profile" + fileextension;
                        string profilepicturepath = "~/DefaultImage/";
                        CreateDirectoryIfMissing(profilepicturepath);
                        string path = Path.Combine(Server.MapPath(profilepicturepath), newfilename);
                        obj.DefaultProfile.SaveAs(path);

                        defaultprofile.Value = profilepicturepath + newfilename;

                        defaultprofile.ModifiedDate = DateTime.Now;
                        defaultprofile.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(defaultprofile).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                // get defaultnote
                var defaultnote = _dbContext.SystemConfigurations.Where(x => x.Key.ToLower() == "defaultnote").FirstOrDefault();
                // if defaultnote is null then create
                if (defaultnote == null)
                {
                    systemConfiguration.Key = "defaultnote";

                    if (obj.DefaultNote != null)
                    {
                        string fileextension = System.IO.Path.GetExtension(obj.DefaultNote.FileName);
                        string newfilename = "note" + fileextension;
                        string notepicturepath = "~/DefaultImage/";
                        CreateDirectoryIfMissing(notepicturepath);
                        string path = Path.Combine(Server.MapPath(notepicturepath), newfilename);
                        obj.DefaultNote.SaveAs(path);

                        systemConfiguration.Value = notepicturepath + newfilename;
                    }

                    systemConfiguration.CreatedDate = DateTime.Now;
                    systemConfiguration.CreatedBy = superadmin.ID;
                    systemConfiguration.IsActive = true;

                    _dbContext.SystemConfigurations.Add(systemConfiguration);
                    _dbContext.SaveChanges();
                }
                // edit defaultnote
                else
                {
                    // check if user uploaded defaultnote
                    if (obj.DefaultNote != null)
                    {
                        // check if there is already note picture is available or not
                        // if available then we have to delete
                        if (defaultnote.Value != null)
                        {
                            string previouspath = Server.MapPath(defaultnote.Value);
                            FileInfo file = new FileInfo(previouspath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }

                        string fileextension = System.IO.Path.GetExtension(obj.DefaultNote.FileName);
                        string newfilename = "note" + fileextension;
                        string notepicturepath = "~/DefaultImage/";
                        CreateDirectoryIfMissing(notepicturepath);
                        string path = Path.Combine(Server.MapPath(notepicturepath), newfilename);
                        obj.DefaultNote.SaveAs(path);

                        defaultnote.Value = notepicturepath + newfilename;

                        defaultnote.ModifiedDate = DateTime.Now;
                        defaultnote.ModifiedBy = superadmin.ID;

                        _dbContext.Entry(defaultprofile).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                return RedirectToAction("ManageSystem");
            }
            else
            {
                return View(obj);
            }
        }

        // Create Directory
        private void CreateDirectoryIfMissing(string folderpath)
        {
            // check if directory is exists or not
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));

            // if directory is not exists then create directory
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }

       

        // GET: AdminSettings
        [Authorize(Roles = "SuperAdmin")]
        [Route("Settings/ManageAdministrator")]
        public ActionResult ManageAdministrator(string search, string sortby, string sortorder, int page = 1)
        {
            ViewBag.ManageAdmin = "active";
            ViewBag.ManageAdminSearch = search;
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;

            IEnumerable<ManageAdministratorViewModel> managelist;
            managelist = from user in _dbContext.Users
                         join userprofile in _dbContext.UserProfiles on user.ID equals userprofile.UserID
                         where user.RoleID == 2
                         select new ManageAdministratorViewModel
                         {
                             Active = user.IsActive == true ? "Yes" : "No",
                             DateAdded = user.CreatedDate,
                             Email = user.EmailID,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             UserID = user.ID,
                             PhoneNumber = userprofile.PhoneNumber
                         };

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                managelist = managelist.Where(x => x.Active.ToLower().Contains(search) ||
                                                   x.DateAdded.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                   x.Email.ToLower().Contains(search) ||
                                                   x.FirstName.ToLower().Contains(search) ||
                                                   x.LastName.ToLower().Contains(search) ||
                                                   x.PhoneNumber.ToLower().Contains(search)).ToList();
            }

            //Going for sorting
            managelist = SortTableManageAdministator(sortby, sortorder, managelist);

            //Count a total PageNumber
            ViewBag.TotalPages = Math.Ceiling(managelist.Count() / 5.0);
            //Skip a record
            managelist = managelist.Skip((page - 1) * 5).Take(5);
            return View(managelist);
        }

        private IEnumerable<ManageAdministratorViewModel> SortTableManageAdministator(string sortby, string sortorder, IEnumerable<ManageAdministratorViewModel> table)
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
                case "PhoneNumber":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.PhoneNumber);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.PhoneNumber);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.PhoneNumber);
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
                case "Active":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Active);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Active);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Active);
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        [Route("Settings/ManageAdministrator/Add")]
        public ActionResult AddAdmin()
        {
            AddAdministrativeViewModel add = new AddAdministrativeViewModel();
            add.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
            return View(add);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route("Settings/ManageAdministrator/Add")]
        public ActionResult AddAdmin(AddAdministrativeViewModel add)
        {
            if (ModelState.IsValid)
            {
                //Check if email is alredy exist or not
                var already = _dbContext.Users.Where(x => x.EmailID == add.Email).FirstOrDefault();
                if (already != null)
                {
                    ModelState.AddModelError("Email", "This email is already");
                    add.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
                    return View(add);
                }

                //Get Logged in user
                var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

                //Create object of User
                User usr = new User();
                usr.CreatedBy = user.ID;
                usr.FirstName = add.FirstName;
                usr.LastName = add.LastName;
                usr.EmailID = add.Email;
                usr.CreatedDate = DateTime.Now;
                usr.Password = "Admin@123";
                usr.IsEmailVerified = true;
                usr.IsActive = true;
                usr.RoleID = 2;
                _dbContext.Users.Add(usr);
                _dbContext.SaveChanges();

                //Create Object of Userprofile
                UserProfile userProfile = new UserProfile();
                userProfile.UserID = usr.ID;
                userProfile.Gender = 1;
                userProfile.CreatedBy = user.ID;
                userProfile.PhoneNumber = add.PhoneNumber;
                userProfile.PhoneNumberCountryCode = add.PhoneCode;
                userProfile.State = "NA";
                userProfile.Country = "NA";
                userProfile.City = "NA";
                userProfile.AddressLine1 = "NA";
                userProfile.AddressLine2 = "NA";
                userProfile.ZipCode = "NA";
                _dbContext.UserProfiles.Add(userProfile);
                _dbContext.SaveChanges();
                return RedirectToAction("ManageAdministrator");
            }
            else
            {
                add.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
                return View(add);
            }
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        [Route("Settings/ManageAdministrator/Edit/{id}")]
        public ActionResult EditAdmin(int id)
        {
            //Get Logged in user
            var userid = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //find in database in user
            var user = _dbContext.Users.Where(x => x.ID == id).FirstOrDefault();
            var userprofile = _dbContext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();

            //Assign Value
            AddAdministrativeViewModel edit = new AddAdministrativeViewModel();
            edit.Email = user.EmailID;
            edit.FirstName = user.FirstName;
            edit.LastName = user.LastName;
            edit.PhoneCode = userprofile.PhoneNumberCountryCode;
            edit.PhoneNumber = userprofile.PhoneNumber;
            edit.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
            edit.UserID = user.ID;

            return View(edit);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [Route("Settings/ManageAdministrator/Edit/{id}")]
        public ActionResult EditAdmin(AddAdministrativeViewModel edit)
        {
            if (ModelState.IsValid)
            {
                //Get logged in user
                var loginuser = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Find a Details in database
                var user = _dbContext.Users.Where(x => x.ID == edit.UserID).FirstOrDefault();
                var userprofile = _dbContext.UserProfiles.Where(x => x.UserID == user.ID).FirstOrDefault();

                //Check email is alredy use or not
                var already = _dbContext.Users.Where(x => x.EmailID == edit.Email).FirstOrDefault();
                if (already != null)
                {
                    ModelState.AddModelError("Email", "This email is already Used");
                    return View(edit);
                }

                user.EmailID = edit.Email;
                user.FirstName = edit.FirstName;
                user.LastName = edit.LastName;
                user.ModifiedBy = loginuser.ID;
                user.ModifiedDate = DateTime.Now;
                userprofile.PhoneNumber = edit.PhoneNumber;
                userprofile.PhoneNumberCountryCode = edit.PhoneCode;
                userprofile.ModifiedBy = loginuser.ID;
                userprofile.ModifiedDate = DateTime.Now;
                //Save data in database
                _dbContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                _dbContext.Entry(userprofile).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("ManageAdministrator");
            }
            else
            {
                edit.PhoneCodeList = _dbContext.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CountryCode).ToList();
                return View(edit);
            }
        }
        [Authorize(Roles ="SuperAdmin")]
        [Route("Settings/ManageAdministrator/Delete/{id}")]
        public ActionResult DeleteAdmin(int id)
        {
            //Get Logged in user
            var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
            var admin = _dbContext.Users.Where(x => x.ID == id).FirstOrDefault();

            if (admin == null)
            {
                return HttpNotFound();
            }
            admin.IsActive = false;
            admin.ModifiedBy = superadmin.ID;
            admin.ModifiedDate = DateTime.Now;
            _dbContext.Entry(admin).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("ManageAdministrator");
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Settings/ManageCountry")]
        public ActionResult ManageCountry(string search, string sortby, string sortorder, int page = 1)
        {
            ViewBag.ManageCountry = "active";
            ViewBag.ManageCountrySearch = search;
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;

            IEnumerable<ManageCountryViewModel> countrylist;
            countrylist = from country in _dbContext.Countries
                          join added in _dbContext.Users on country.CreatedBy equals added.ID
                          where added.IsActive == true
                          select new ManageCountryViewModel {
                              CreatedDate = country.CreatedDate,
                              CountryName=country.Name,
                              CountryCode = country.CountryCode,
                              AddedBy=added.FirstName+" "+added.LastName,
                              Active=country.IsActive==true?"Yes":"No",
                              ID=country.ID
                          };

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                countrylist = countrylist.Where(x => x.Active.ToLower().Contains(search) ||
                                                   x.CreatedDate.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                   x.CountryName.ToLower().Contains(search) ||
                                                   x.CountryCode.ToString().ToLower().Contains(search) ||
                                                   x.AddedBy.ToLower().Contains(search)
                                                   ).ToList();
            }

            countrylist = SortTableManageCountry(sortby, sortorder, countrylist);
            
            //Count a total PageNumber
            ViewBag.TotalPages = Math.Ceiling(countrylist.Count() / 5.0);
            //Skip a record
            countrylist = countrylist.Skip((page - 1) * 5).Take(5);
            return View(countrylist);
        }
        private IEnumerable<ManageCountryViewModel> SortTableManageCountry(string sortby, string sortorder, IEnumerable<ManageCountryViewModel> table)
        {
            switch (sortby)
            {
                case "CountryName":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.CountryName);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.CountryName);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.CountryName);
                                    return table;
                                }
                        }
                    }
                case "CountryCode":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x=>x.CountryCode);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.CountryCode);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.CountryCode);
                                    return table;
                                }
                        }
                    }
                case "AddedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.AddedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AddedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AddedBy);
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
                case "Active":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Active);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Active);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Active);
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
                                    table = table.OrderBy(x => x.CountryName);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.CountryName);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.CountryName);
                                    return table;
                                }
                        }
                    }
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageCountry/Add")]
        public ActionResult AddCountry()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Route("Settings/ManageCountry/Add")]
        [ValidateAntiForgeryToken]
        public ActionResult AddCountry(AddCountryViewModel add)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel
                
                var alredy = _dbContext.Countries.Where(x => x.Name == add.CountryName || x.CountryCode == add.Code).FirstOrDefault();
                if (alredy != null)
                {
                    ModelState.AddModelError("CountryCode", "Name or code is alredy entered");
                    return HttpNotFound();
                }
                Country country = new Country {
                    CountryCode = add.Code,
                    Name=add.CountryName,
                    CreatedBy=superadmin.ID,
                    IsActive=true,
                    CreatedDate=DateTime.Now
                };
                _dbContext.Countries.Add(country);
                _dbContext.SaveChanges();
                return RedirectToAction("ManageCountry");
            }
            else
            {
                return View(add);
            }
        }
        [HttpGet]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Route("Settings/ManageCountry/Edit/{id}")]
        public ActionResult EditCountry(int id)
        {
            //Get logged in user details
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //find a coutry with id
            var countryid = _dbContext.Countries.Where(x => x.ID == id).FirstOrDefault();
            if (countryid == null)
            {
                return HttpNotFound();
            }
            //Create a object of viewmodel
            AddCountryViewModel edit = new AddCountryViewModel();
            edit.ID = countryid.ID;
            edit.Code = countryid.CountryCode;
            edit.CountryName = countryid.Name;
            
            return View(edit);
            
        }

        [HttpPost]
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Route("Settings/ManageCountry/Edit/{id}")]
        public ActionResult EditCountry(AddCountryViewModel edit)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel

                
                //Find country with id
                var country = _dbContext.Countries.Where(x => x.ID == edit.ID).FirstOrDefault();
                country.CountryCode = edit.Code;
                country.Name = edit.CountryName;
                country.ModifiedBy = superadmin.ID;
                country.ModifiedDate = DateTime.Now;
                _dbContext.Entry(country).State = System.Data.Entity.EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("ManageCountry");
            }
            else
            {
                return View(edit);
            }
        }
        [Authorize(Roles ="SuperAdmin,Admin")]
        [Route("Settings/ManageCountry/Delete/{id}")]
        public ActionResult DeleteCountry(int id)
        {
            //Get details of logged in user
            var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Find country in database
            var country = _dbContext.Countries.Where(x => x.ID == id).FirstOrDefault();
            if (country == null)
            {
                return HttpNotFound();
            }
            country.IsActive = false;
            country.ModifiedBy = superadmin.ID;
            country.ModifiedDate = DateTime.Now;
            _dbContext.Entry(country).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("ManageCountry");
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Settings/ManageTypes")]
        public ActionResult ManageTypes(string search, string sortby, string sortorder, int page = 1)
        {
            ViewBag.ManageType = "active";
            ViewBag.ManageTypeSearch = search;
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;

            IEnumerable<ManageTypesViewModel> typelist;
            typelist = from type in _dbContext.NotesTypes
                          join added in _dbContext.Users on type.CreatedBy equals added.ID
                          where added.IsActive == true
                          select new ManageTypesViewModel
                          {
                              DateAdded = type.CreatedDate,
                              Type = type.Name,
                              Description = type.Description,
                              AddedBy = added.FirstName + " " + added.LastName,
                              Active = type.IsActive == true ? "Yes" : "No",
                              ID = type.ID
                          };

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                typelist = typelist.Where(x => x.Active.ToLower().Contains(search) ||
                                                   x.DateAdded.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                   x.Type.ToLower().Contains(search) ||
                                                   x.Description.ToLower().Contains(search) ||
                                                   x.AddedBy.ToLower().Contains(search)
                                                   ).ToList();
            }

            typelist = SortTableManageTypes(sortby, sortorder, typelist);

            //Count a total PageNumber
            ViewBag.TotalPages = Math.Ceiling(typelist.Count() / 5.0);
            //Skip a record
            typelist = typelist.Skip((page - 1) * 5).Take(5);
            return View(typelist);
        }
        private IEnumerable<ManageTypesViewModel> SortTableManageTypes(string sortby, string sortorder, IEnumerable<ManageTypesViewModel> table)
        {
            switch (sortby)
            {
                case "Type":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Type);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Type);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Type);
                                    return table;
                                }
                        }
                    }
                case "Description":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Description);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Description);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Description);
                                    return table;
                                }
                        }
                    }
                case "AddedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.AddedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AddedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AddedBy);
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
                case "Active":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Active);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Active);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Active);
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
                                    table = table.OrderBy(x => x.Type);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Type);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Type);
                                    return table;
                                }
                        }
                    }
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageTypes/Add")]
        public ActionResult AddTypes()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        [Route("Settings/ManageTypes/Add")]
        public ActionResult AddTypes(AddTypesViewModel add)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel

                var alredy = _dbContext.NotesTypes.Where(x => x.Name == add.Type).FirstOrDefault();
                if (alredy != null)
                {
                    ModelState.AddModelError("Type", "Type is alredy entered");
                    return HttpNotFound();
                }
                NotesType type = new NotesType
                {
                    Description = add.Description,
                    Name = add.Type,
                    CreatedBy = superadmin.ID,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _dbContext.NotesTypes.Add(type);
                _dbContext.SaveChanges();
                return RedirectToAction("ManageTypes");
            }
            else
            {
                return View(add);
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageTypes/Edit/{id}")]
        public ActionResult EditTypes(int id)
        {
            //Get logged in user details
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //find a coutry with id
            var typeid = _dbContext.NotesTypes.Where(x => x.ID == id).FirstOrDefault();
            if (typeid == null)
            {
                return HttpNotFound();
            }
            //Create a object of viewmodel
            AddTypesViewModel edit = new AddTypesViewModel();
            edit.ID = typeid.ID;
            edit.Type = typeid.Name;
            edit.Description = typeid.Description;

            return View(edit);

        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageTypes/Edit/{id}")]
        public ActionResult EditTypes(AddTypesViewModel edit)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel


                //Find country with id
                var type = _dbContext.NotesTypes.Where(x => x.ID == edit.ID).FirstOrDefault();
                type.Description = edit.Description;
                type.Name = edit.Type;
                type.ModifiedBy = superadmin.ID;
                type.ModifiedDate = DateTime.Now;
                _dbContext.Entry(type).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("ManageTypes");
            }
            else
            {
                return View(edit);
            }
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageTypes/Delete/{id}")]
        public ActionResult DeleteTypes(int id)
        {
            //Get details of logged in user
            var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Find country in database
            var type = _dbContext.NotesTypes.Where(x => x.ID == id).FirstOrDefault();
            if (type == null)
            {
                return HttpNotFound();
            }
            type.IsActive = false;
            type.ModifiedBy = superadmin.ID;
            type.ModifiedDate = DateTime.Now;
            _dbContext.Entry(type).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("ManageTypes");
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Route("Settings/ManageCategory")]
        public ActionResult ManageCategory(string search, string sortby, string sortorder, int page = 1)
        {
            ViewBag.ManageType = "active";
            ViewBag.ManageCategorySearch = search;
            ViewBag.PageNumber = page;
            ViewBag.SortBy = sortby;
            ViewBag.SortOrder = sortorder;

            IEnumerable<ManageCategoryViewModel> categorylist;
            categorylist = from category in _dbContext.NotesCategories
                       join added in _dbContext.Users on category.CreatedBy equals added.ID
                       where added.IsActive == true
                       select new ManageCategoryViewModel
                       {
                           DateAdded = category.CreatedDate,
                            Category= category.Name,
                           Description = category.Description,
                           AddedBy = added.FirstName + " " + added.LastName,
                           Active = category.IsActive == true ? "Yes" : "No",
                           ID = category.ID
                       };

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                categorylist = categorylist.Where(x => x.Active.ToLower().Contains(search) ||
                                                   x.DateAdded.Value.ToString("dd-MM-yyyy,hh:mm").Contains(search) ||
                                                   x.Category.ToLower().Contains(search) ||
                                                   x.Description.ToLower().Contains(search) ||
                                                   x.AddedBy.ToLower().Contains(search)
                                                   ).ToList();
            }

            categorylist = SortTableManageCategory(sortby, sortorder, categorylist);

            //Count a total PageNumber
            ViewBag.TotalPages = Math.Ceiling(categorylist.Count() / 5.0);
            //Skip a record
            categorylist = categorylist.Skip((page - 1) * 5).Take(5);
            return View(categorylist);
        }
        private IEnumerable<ManageCategoryViewModel> SortTableManageCategory(string sortby, string sortorder, IEnumerable<ManageCategoryViewModel> table)
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
                case "Description":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Description);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Description);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Description);
                                    return table;
                                }
                        }
                    }
                case "AddedBy":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.AddedBy);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.AddedBy);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.AddedBy);
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
                case "Active":
                    {
                        switch (sortorder)
                        {
                            case "Asc":
                                {
                                    table = table.OrderBy(x => x.Active);
                                    return table;
                                }
                            case "Desc":
                                {
                                    table = table.OrderByDescending(x => x.Active);
                                    return table;
                                }
                            default:
                                {
                                    table = table.OrderBy(x => x.Active);
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
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageCategory/Add")]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageCategory/Add")]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(AddCategoryViewModel add)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel

                var alredy = _dbContext.NotesCategories.Where(x => x.Name == add.Category).FirstOrDefault();
                if (alredy != null)
                {
                    ModelState.AddModelError("CountryCode", "Name or code is alredy entered");
                    return HttpNotFound();
                }
                NotesCategory category = new NotesCategory
                {
                    Description = add.Description,
                    Name = add.Category,
                    CreatedBy = superadmin.ID,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                _dbContext.NotesCategories.Add(category);
                _dbContext.SaveChanges();
                return RedirectToAction("ManageCategory");
            }
            else
            {
                return View(add);
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageCategory/Edit/{id}")]
        public ActionResult EditCategory(int id)
        {
            //Get logged in user details
            var user = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //find a coutry with id
            var categoryid = _dbContext.NotesCategories.Where(x => x.ID == id).FirstOrDefault();
            if (categoryid == null)
            {
                return HttpNotFound();
            }
            //Create a object of viewmodel
            AddCategoryViewModel edit = new AddCategoryViewModel();
            edit.ID = categoryid.ID;
            edit.Category = categoryid.Name;
            edit.Description = categoryid.Description;

            return View(edit);

        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [Route("Settings/ManageCategory/Edit/{id}")]
        public ActionResult EditCategory(AddCategoryViewModel edit)
        {
            if (ModelState.IsValid)
            {
                //Get Logged in user
                var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                //Create object of Viewmodel


                //Find country with id
                var type = _dbContext.NotesCategories.Where(x => x.ID == edit.ID).FirstOrDefault();
                type.Description = edit.Description;
                type.Name = edit.Category;
                type.ModifiedBy = superadmin.ID;
                type.ModifiedDate = DateTime.Now;
                _dbContext.Entry(type).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("ManageCategory");
            }
            else
            {
                return View(edit);
            }
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteCategory(int id)
        {
            //Get details of logged in user
            var superadmin = _dbContext.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            //Find country in database
            var category = _dbContext.NotesCategories.Where(x => x.ID == id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            category.IsActive = false;
            category.ModifiedBy = superadmin.ID;
            category.ModifiedDate = DateTime.Now;
            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("ManageTypes");
        }
    }
}