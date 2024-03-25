using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumsUnknown.Controllers
{
    public class HomeController : Controller
    {
        private FuDBContext db = new FuDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

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
                Session["Username"] = user.UserName;
                ViewBag.Notification = "Successfully logged in.";
                ViewBag.NotificationColor = "text-success";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
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
                if(ModelState.IsValid)
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

        private bool UserExists(string username)
        {
            using (var db = new FuDBContext())
            {
                bool userExists = db.FORUM_USERS.Any(u => u.UserName == username);

                return userExists;
            }
        }
    }
}