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

namespace ForumsUnknown.Controllers
{
    public class UserController : Controller
    {
        private FuDBContext db = new FuDBContext();

        // GET: User
        [HttpGet]
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
        [ValidateAntiForgeryToken]
        public ActionResult Login(FORUM_USERS user)
        {
            bool userExists = UserExists(user.UserName);
            if (userExists)
            {
                FORUM_USERS UserInfo = GetUserByUsername(user.UserName);

                Session["Username"] = UserInfo.UserName;
                Session["UserId"] = UserInfo.UserID;
                ViewBag.Notification = "Successfully logged in.";
                ViewBag.NotificationColor = "text-success";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
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

        private FORUM_USERS GetUserByUsername(string username)
        {
            using (var db = new FuDBContext())
            {
                return db.FORUM_USERS.FirstOrDefault(u => u.UserName == username);
            }
        }

        [HttpGet]
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
        [ValidateAntiForgeryToken]
        public ActionResult Register(FORUM_USERS user)
        {
            bool userExists = UserExists(user.UserName);

            if (userExists)
            {
                ViewBag.Notification = "Username already exists.";
                ViewBag.NotificaitonColor = "text-danger";
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
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    return View(user);
                }
            }
        }

        public ActionResult UserProfile(int id)
        {
            FORUM_USERS user = db.FORUM_USERS.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
    }
}