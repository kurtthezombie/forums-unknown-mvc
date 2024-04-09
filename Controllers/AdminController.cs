using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumsUnknown.Controllers
{
    public class AdminController : Controller
    {
        private FuDBContext db = new FuDBContext();

        // GET: Admin
        public ActionResult AdminPage()
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                return username != "admin" ? RedirectToAction("Index", "Home") : (ActionResult)View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            
        }

        public ActionResult Users()
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                if (username != "admin")
                {
                    return RedirectToAction("Index","Home");
                } 
                else
                {
                    var users = db.FORUM_USERS.ToList();
                    return View(users);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            
        }

        public ActionResult Posts()
        {
            
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                if (username != "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var posts = db.FORUM_POSTS.ToList();
                    return View(posts);
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

    }

}