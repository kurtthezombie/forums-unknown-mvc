using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;

namespace ForumsUnknown.Controllers
{
    public class UserController : Controller
    {
        private FuDBContext db = new FuDBContext();

        // GET: User
        [HttpGet]
        [Route("Login")]
        public ActionResult Login()
        {
            if (Session["Username"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [Route("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FORUM_USERS user)
        {
            //trim username 
            string username = user.UserName.Replace(" ","").Trim();
            bool userExists = UserExists(username);
            if (userExists)
            {
                FORUM_USERS UserInfo = GetUserByUsername(username);
                
                if (UserNamePassMatch(username, user.UserPassword))
                {
                    Session["Username"] = username;
                    Session["UserId"] = UserInfo.UserID;
                    ViewBag.Notification = "Successfully logged in.";
                    ViewBag.NotificationColor = "text-success";
                    //set authorization here
                    FormsAuthentication.SetAuthCookie(username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Notification = "Incorrect username or password.";
                    ViewBag.NotificationColor = "text-danger";
                    return View();
                }

            }
            else
            {
                ViewBag.Notification = "Account does not exist.";
                ViewBag.NotificationColor = "text-danger";
                return View();

            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        private bool UserExists(string username)
        {
            using (var db = new FuDBContext())
            {
                bool userExists = db.FORUM_USERS.Any(u => u.UserName == username);

                return userExists;
            }
        }

        private bool UserNamePassMatch(string username, string password)
        {
            using (var db = new FuDBContext())
            {
                FORUM_USERS user = db.FORUM_USERS.FirstOrDefault(u => u.UserName == username);
                
                //if both match return true, else return false
                return user.UserName.Equals(username) && user.UserPassword.Equals(password);
            }
        }

        //private bool passwordMatch(string username, string password)
        //{
        //    using (var db = new FuDBContext())
        //    {
        //        //get row matching password and username 
        //        //check if password written matches password in database
        //    }
        //}

        private FORUM_USERS GetUserByUsername(string username)
        {
            using (var db = new FuDBContext())
            {
                return db.FORUM_USERS.FirstOrDefault(u => u.UserName == username);
            }
        }

        [HttpGet]
        [Route("Register")]
        public ActionResult Register()
        {
            if (Session["Username"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string title = "Register";
                ViewBag.Title = title;
                return View();
            }
        }
        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FORUM_USERS user)
        {
            bool userExists = UserExists(user.UserName);

            if (userExists)
            {
                ViewBag.Notification = "Username already exists.";
                ViewBag.NotificationColor = "text-danger";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.FORUM_USERS.Add(user);
                    db.SaveChanges();

                    ViewBag.Notification = "Successfully registered.";
                    ViewBag.NotificationColor = "text-success";
                    //clear textfields
                    ModelState.Clear();

                    return View(new FORUM_USERS());
                }
                else
                {
                    return View(user);
                }
            }
        }
        [Authorize]
        [Route("MyProfile")]
        public ActionResult UserProfile()
        {
            int id = (int)Session["UserId"];
            //if (id != SessionId)
            //{
            //    return RedirectToAction("Index", "Home", null);
            //}
            if (Session["Username"] != null )
            {
                FORUM_USERS user = db.FORUM_USERS.Find(id);

                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Login","User");
            }

        }

        public ActionResult DeleteUser (int id)
        {
            Session.Clear();
            Delete(id);

            return RedirectToAction("Index", "Home");
        }

        public void Delete(int? id)
        {
            var data = db.FORUM_USERS.Find(id);

            db.FORUM_USERS.Remove(data);
            db.SaveChanges();
        }

        [Authorize]
        [HttpGet]
        [Route("EditProfile")]
        public ActionResult EditUser()
        {
            int id = (int)Session["UserId"];
            FORUM_USERS user = db.FORUM_USERS.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [Authorize]
        [HttpPost]
        [Route("EditProfile")]
        public ActionResult EditUser(FORUM_USERS user)
        {
            if (ModelState.IsValid)
            {
                if (IsUsernameUnique(user.UserID, user.UserName))
                {
                    //get the row
                    var data = db.FORUM_USERS.Where(x => x.UserID == user.UserID).FirstOrDefault();
                    //edit data
                    if (data != null)
                    {
                        data.UserName = user.UserName;
                        data.EmailAddress = user.EmailAddress;
                        data.UserPassword = user.UserPassword;
                        data.ConfirmPassword = user.ConfirmPassword;
                    }

                    db.SaveChanges();

                    return RedirectToAction("UserProfile", new { id = Session["UserId"] });
                }
                else
                {
                    // If the new username already exists, add a model error and return the Edit view
                    ModelState.AddModelError("UserName", "Username already exists.");
                    return View(user);
                }

            }
            return View(user);
        }

        private bool IsUsernameUnique(int userId, string newUserName)
        {
            var existingUser = db.FORUM_USERS.FirstOrDefault(u => u.UserID == userId);

            return existingUser == null || existingUser.UserName == newUserName ||
                !db.FORUM_USERS.Any(u => u.UserID != userId && u.UserName == newUserName);
        }
    }
}