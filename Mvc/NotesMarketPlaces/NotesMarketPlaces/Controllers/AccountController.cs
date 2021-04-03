using System;
using System.Linq;
using System.Web.Mvc;
using NotesMarketPlaces.Models;
using System.Net.Mail;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;
using NotesMarketPlaces.Send_Mail;

namespace NotesMarketPlaces.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        readonly private NotesMarketPlaceEntities _dbContext = new NotesMarketPlaceEntities();


        //Get : Account/SignUp
        [HttpGet]
        [Route("SignUp")]
        public ActionResult SignUp()
        {
            return View();
        }

        //Post : Account/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Signup")]
        public ActionResult SignUp(SignUpViewModal suvm)
        {
            if (ModelState.IsValid)
            {
                //Check if Email Is Exists or Not
                var isEmailAlreadyExists = _dbContext.Users.Where(a => a.EmailID.Equals(suvm.EmailID)).FirstOrDefault();
                if (isEmailAlreadyExists != null)
                {
                    ModelState.AddModelError("EmailID", "User with this email already exists");
                    return View(suvm);
                }
                else
                {
                    //Create user and Save data in Database
                    User user = new User {
                        FirstName = suvm.FirstName.ToString(),
                        RoleID = 3,
                        LastName = suvm.LastName.ToString(),
                        Password = suvm.ConfirmPassword,
                        EmailID = suvm.EmailID,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        CreatedBy = 1
                    };

                    //Add Object in database
                    _dbContext.Users.Add(user);
                    //Save Data in Database
                    _dbContext.SaveChanges();

                    //Send  Message to User Account is created
                    ViewBag.result = true;

                    string date = user.CreatedDate.Value.ToString("ddMMyyyyHHmmss");
                    //Send Mail For Cofirmation
                    BuildEmailVerifyTemplate(user,date);
                    //To Give a message to user for Sign up is Success
                    ViewBag.result = "Success";
                    //Clear a Model State
                    ModelState.Clear();
                    return View();
                }
            }
            else
            {
                return View(suvm);
            }

        }

        //For Varifying Email
        [Route("VerifyEmail")]
        public ActionResult VerifyEmail(int key,string value)
        {
            ViewBag.Key = key;
            ViewBag.Value = value;
            return View();
        }

        //To Set is emailverified true in database after verification
        [Route("RegisterConfirm")]
        public ActionResult RegisterConfirm(int key)
        {
            User user = _dbContext.Users.Where(x => x.ID == key).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            user.IsEmailVerified = true;
            user.ModifiedBy = user.ID;
            user.ModifiedDate = DateTime.Now;
            _dbContext.SaveChanges();
            
            return RedirectToAction("Login", "Account");
        }

        //To Get Email  Varification Template
        public void BuildEmailVerifyTemplate(User user,string date)
        {
            //Get Text From email verification template
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "EmailVarification" + ".cshtml");
            var url = "https://localhost:44379/" + "Account/VerifyEmail?key=" + user.ID+"&value"+date;

            //Replace url and first Name
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.Replace("@ViewBag.FirstName", user.FirstName);
            body = body.ToString();
            
            //Get Support Email
            var fromemail = _dbContext.SystemConfigurations.Where(x => x.Key == "SupportEmail").FirstOrDefault();

            //From , To,Subject
            string from, to, subject;
            from = fromemail.Value.Trim();
            to = user.EmailID.Trim();
            subject = "Note Marketplace - Email Verification";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            //To create a Mail
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from, "NotesMarketPlace");
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            //Send Mail
            SendingMail.SendEmail(mail);
        }

        
        // GET: Account/Login
        [HttpGet]
        [Route("Login")]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                return View();
            }
        }

        //Post : Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public ActionResult Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                //Check if user  for given email is present or not
                var user = _dbContext.Users.FirstOrDefault(a => a.EmailID.Equals(lvm.Email));
                if (user != null)
                {
                    //Check user is active or not
                    if (user.IsActive)
                    {
                        //Check if email varified or not
                        if (user.IsEmailVerified)
                        {
                            //Check if password match
                            if (user.Password == lvm.Password)
                            {
                                //Check if user is member
                                if (user.RoleID == 3)
                                {
                                    //Set authentication cookie
                                    FormsAuthentication.SetAuthCookie(user.EmailID, lvm.RememberMe);
                                    return RedirectToAction("Index", "Home");
                                }
                                //For user admin or super admin
                                else
                                {
                                    //Set authentication cookie
                                    FormsAuthentication.SetAuthCookie(user.EmailID, lvm.RememberMe);
                                    return RedirectToAction("AdminDashboard");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("Password", "Incorrect password");
                                ViewBag.result = "Success";
                                return View(lvm);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("EmailID", "Verify your email address");
                            return View(lvm);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailID", "Your account with this email is not active");
                        return View(lvm);
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailID", "Incorrect Email Id");
                    return View(lvm);
                }
            }
            else
            {
                return View(lvm);
            }
        }
        
        //Logout
        [Authorize]
        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword(ForgotViewModel forgotViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(a => a.EmailID.Equals(forgotViewModel.Email));
                if (user != null && user.IsActive==true)
                {
                    string password;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(RandomNumber(10, 299));
                    sb.Append(RandomString(8));
                    password = sb.ToString();
                    User usr = _dbContext.Users.FirstOrDefault(a => a.EmailID.Equals(forgotViewModel.Email));
                    usr.Password = password;

                    //Send a mail for Random password
                    BuildPasswordTemplate(user);
                    //Save Changes in Database
                    _dbContext.SaveChanges();
                    return RedirectToAction("Login");
                    
                }
                else
                {
                    //Generate Error for email is not exists
                    ModelState.AddModelError("Email", "This email is not exists");
                    return View(forgotViewModel);
                }
            }
            else
            {
                return View(forgotViewModel);
            }
            
        }
        //To Generate a Random Number for Password
        private int RandomNumber(int min,int max)
        {
            Random randomNumber = new Random();
            return randomNumber.Next(min,max);
        }
        //To Generate a Random String for Password
        private string RandomString(int lengths)
        {
            StringBuilder sb = new StringBuilder();
            Random randomString = new Random();
            char value; 
            for(int i = 0; i < lengths; i++)
            {
                value=Convert.ToChar(Convert.ToInt32(Math.Floor(26*randomString.NextDouble()+65)));
                sb.Append(value);
            }
            return randomString.ToString();
        }
        //To Get Email  Varification Template
        public void BuildPasswordTemplate(User user)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplates/") + "TemporaryPassword" + ".cshtml");
            
            body = body.Replace("@ViewBag.FirstName", user.FirstName);
            body = body.Replace("@ViewBag.Password",user.Password);
            body = body.ToString();

            //Get Support Email ID
            var fromemail = _dbContext.SystemConfigurations.Where(x => x.Value == "SupportEmail").FirstOrDefault();

            //Set from,to,subject,body
            string from, to, subject;
            from = fromemail.Key.Trim();
            to = user.EmailID.Trim();
            subject = "New Temporary Password has been created for you";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            //Create mailmessage object
            MailMessage mail = new MailMessage();
            mail.From=new MailAddress(from, "NoteMarket Place");
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            //Send A Mail 
            SendingMail.SendEmail(mail);
        }
    }
}