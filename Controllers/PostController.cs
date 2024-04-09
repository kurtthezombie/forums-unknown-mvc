using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumsUnknown.Controllers
{
    public class PostController : Controller
    {
        FuDBContext db = new FuDBContext();
        [HttpGet]
        public ActionResult CreatePost()
        {
            if (Session["Username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(FORUM_POSTS post)
        {
            if (ModelState.IsValid)
            {
                //set getdate
                post.CreatedAt = DateTime.Now;
                //query the insertion
                db.FORUM_POSTS.Add(post);
                db.SaveChanges();
                //notification messages
                ViewBag.Notification = "Successfully posted.";
                ViewBag.NotificationColor = "text-success";

                return View();
            } 
            else
            {
                return View(post);
            }
        }

    }
}