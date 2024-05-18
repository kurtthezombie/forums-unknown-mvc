using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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
                post.ModifiedAt = DateTime.Now;
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
        [Route("EditPost")]
        public ActionResult EditPost(int id)
        {
            FORUM_POSTS post = db.FORUM_POSTS.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        [HttpPost]
        [Route("EditPost")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(FORUM_POSTS post)
        {
            if (ModelState.IsValid)
            {
                var data = db.FORUM_POSTS.Find(post.PostID);

                if (data == null)
                {
                    return HttpNotFound();
                }

                data.Title = post.Title;
                data.Content = post.Content;
                data.ModifiedAt = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("MyPosts", new { id = Session["UserId"] });

            }
            else
            { 
                return View(post);
            }
            
        }

        public ActionResult DeletePost(int id)
        {
            Delete(id);
            return RedirectToAction("MyPosts", new { id = Session["UserId"] });
        }

        public void Delete(int id)
        {
            var data = db.FORUM_POSTS.Find(id);

            //delete comments first before the post
            var comments = db.COMMENT.Where(c => c.PostID == id).ToList();

            if(comments.Any())
            {
                db.COMMENT.RemoveRange(comments);
            }

            //delete posts then
            db.FORUM_POSTS.Remove(data);
            db.SaveChanges();
        }

        [HttpGet]
        [Route("Post/{id}")]
        public ActionResult Post(int id)
        {
            //FORUM_POSTS post = db.FORUM_POSTS.Find(id);
            int postId = id;

            //query post
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
            //query comments
            //var comments = db.COMMENT.Where(c => c.PostID == id).ToList();
            var comments = (from c in db.COMMENT
                            join u in db.FORUM_USERS on c.AuthorID equals u.UserID
                            where c.PostID == postId
                            orderby c.CreatedAt descending
                            select new CommentViewModel
                            {
                                CommentID = c.CommentID,
                                Content = c.Content,
                                AuthorName = u.UserName, //get authorname instead of id
                                PostID = c.PostID,
                                CreatedAt = c.CreatedAt,
                                ModifiedAt = c.ModifiedAt
                            }).ToList();
            ;

            //create new PCVM instance
            var PostCommentsVM = new PostCommentsViewModel();

            //map post to forum post
            PostCommentsVM.ForumPost = post;

            PostCommentsVM.Comments = comments;

            ViewBag.CommentCount = db.COMMENT.Where(c => c.PostID == id).Count();


            return View(PostCommentsVM);
        }

        [Route("MyPosts")]
        public ActionResult MyPosts()
        {
            int id = (int)Session["UserId"];
            var posts = db.FORUM_POSTS.Where(post => post.AuthorID == id).ToList();
            return View(posts);
        }

        [HttpPost]
        [Route("CreateComment")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(COMMENT comment)
        {
            if (ModelState.IsValid)
            {
                //set data of each field
                comment.CreatedAt = DateTime.Now;
                comment.ModifiedAt = DateTime.Now;
                db.COMMENT.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Post","Post", new { id = comment.PostID });
            }
            else
            {
                return RedirectToAction("Post","Post", new { id = comment.PostID });
            }  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Search")]
        public ActionResult Search(string searchText)
        {
            //no toLower
            var posts = (from p in db.FORUM_POSTS
                         join u in db.FORUM_USERS on p.AuthorID equals u.UserID
                         where (p.Title.Contains(searchText) || p.Content.Contains(searchText))
                         orderby p.CreatedAt descending
                         select new ForumPostViewModel
                         {
                             PostID = p.PostID,
                             Title = p.Title,
                             Content = p.Content,
                             CreatedAt = (DateTime)p.CreatedAt,
                             ModifiedAt = (DateTime)p.ModifiedAt,
                             AuthorName = u.UserName
                         }).ToList();
            if (posts.Count == 0)
            {
                return View(new List<ForumPostViewModel>());
            }

            return View(posts);
        }
    }
}