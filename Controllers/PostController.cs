using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumsUnknown.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        FuDBContext db = new FuDBContext();

        [HttpGet]
        [Route("CreatePost")]
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
        [Route("CreatePost")]
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
                //clear textfields
                ModelState.Clear();

                return View(new FORUM_POSTS());
            } 
            else
            {
                return View(post);
            }
        }

        [HttpGet]
        public ActionResult EditPost(int id)
        {
            FORUM_POSTS post = db.FORUM_POSTS.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        public void Delete(int? id)
        {
            var data = db.FORUM_POSTS.Find(id);

            db.FORUM_POSTS.Remove(data);
            db.SaveChanges();
        }

        [Route("Post/{id}")]
        public ActionResult Post(int id)
        {
            //FORUM_POSTS post = db.FORUM_POSTS.Find(id);
            int postId = id;

            var post = (from p in db.FORUM_POSTS
                        join u in db.FORUM_USERS on p.AuthorID equals u.UserID
                        where p.PostID == postId // Filter by post ID
                        select new ForumPostViewModel
                        {
                            PostID = p.PostID,
                            Title = p.Title,
                            Content = p.Content,
                            CreatedAt = (DateTime)p.CreatedAt,
                            AuthorName = u.UserName
                        }).FirstOrDefault();

            return View(post);
        }

        [Route("MyPosts")]
        public ActionResult MyPosts(int id)
        {
            var posts = db.FORUM_POSTS.Where(post => post.AuthorID == id).ToList();
            return View(posts);
        }
    }
}