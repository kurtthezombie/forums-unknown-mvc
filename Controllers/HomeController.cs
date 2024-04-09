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
    public class HomeController : Controller
    {
        private FuDBContext db = new FuDBContext();

        public ActionResult Index()
        {
            //var posts = db.FORUM_POSTS.ToList();
            /*if { Session["Username"] != null}
            {
                using (var db = new FuDBContext())
                {
                    var postsWithAuthors = db.FORUM_POSTS
                        .Include(p => p.FORUM_USERS)
                        .FirstOrDefault(p => p.AuthorID == Session["UserId"])
                        .ToList();

                    return View(postsWithAuthors);
                }
            }
            */
            var posts = (from p in db.FORUM_POSTS
                         join u in db.FORUM_USERS on p.AuthorID equals u.UserID
                         orderby p.CreatedAt descending
                         select new ForumPostViewModel
                         {
                             PostID = p.PostID,
                             Title = p.Title,
                             Content = p.Content,
                             CreatedAt = (DateTime)p.CreatedAt,
                             AuthorName = u.UserName
                         }).ToList();

            return View(posts);
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
    }
}