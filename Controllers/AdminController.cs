using ForumsUnknown.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace ForumsUnknown.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private FuDBContext db = new FuDBContext();

        // GET: Admin
        [Route("Admin")]
        public ActionResult AdminPage()
        {
            if (Session["Username"] != null)
            {
                string username = Session["Username"].ToString();
                if (username == "admin")
                {
                    TableCountsViewModel tableCounts = new TableCountsViewModel();

                    tableCounts.UserCount = db.FORUM_USERS.Count();
                    tableCounts.PostCount = db.FORUM_POSTS.Count();
                    tableCounts.FeedbackCount = db.FEEDBACK.Count();

                    return View(tableCounts);
                } 
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            
        }

        #region Users

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(FORUM_USERS user)
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
                    //set dates
                    user.CreatedAt = DateTime.Now;
                    user.ModifiedAt = DateTime.Now;
                    //add
                    db.FORUM_USERS.Add(user);
                    //save
                    db.SaveChanges();

                    //clear fields
                    ModelState.Clear();

                    //mesage
                    ViewBag.Notification = "User created.";
                    ViewBag.NotificationColor = "text-success";

                    return View();
                }
                else
                {
                    return View(user);
                }
            }
                
        }

        public ActionResult DetailsUser(int? id)
        {
            FORUM_USERS user = db.FORUM_USERS.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
    
        }

        [HttpGet]
        public ActionResult EditUser(int id)
        {
            FORUM_USERS user = db.FORUM_USERS.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
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
                        data.ModifiedAt = DateTime.Now;
                    }

                    db.SaveChanges();

                    return RedirectToAction("DetailsUser", new { id = user.UserID });
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

        public ActionResult DeleteUser(int id)
        {
            //var data = db.FORUM_USERS.Find(id);

            //if (data == null)
            //{
            //    return HttpNotFound();
            //}
            //else
            //{
            //    //delete comments first
            //    var comments = db.COMMENT.Where(x => x.AuthorID == id);
            //    db.COMMENT.RemoveRange(comments);

            //    //delete posts of user before deleting user
            //    var posts = db.FORUM_POSTS.Where(x => x.AuthorID == id);
            //    db.FORUM_POSTS.RemoveRange(posts);


            //    //remove the user
            //    db.FORUM_USERS.Remove(data);
            //    db.SaveChanges();
            //}
            var userController = new UserController();
            userController.Delete(id);

            return RedirectToAction("Users");
        }

        private bool IsUsernameUnique(int userId, string newUserName)
        {
            var existingUser = db.FORUM_USERS.FirstOrDefault(u => u.UserID == userId);

            return existingUser == null || existingUser.UserName == newUserName || 
                !db.FORUM_USERS.Any(u => u.UserID != userId && u.UserName == newUserName);
        }
        private bool UserExists(string username)
        {
            using (var db = new FuDBContext())
            {
                bool userExists = db.FORUM_USERS.Any(u => u.UserName == username);

                return userExists;
            }
        }

        public ActionResult Users(string searchString)
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
                    var users = db.FORUM_USERS.AsQueryable();

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        searchString = searchString.Replace(" ", "");
                        users = users.Where(u => u.UserName.Contains(searchString) || u.EmailAddress.Contains(searchString));
                    }

                    ViewBag.CurrentFilter = searchString;
                    ViewBag.ResultsCount = users.Count();
                    return View(users.ToList());
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        #endregion

        #region Posts
        [HttpGet]
        public ActionResult CreatePost()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(FORUM_POSTS post) {
            if (ModelState.IsValid)
            {
                //set values to these necessary data
                post.CreatedAt = DateTime.Now;
                post.ModifiedAt = DateTime.Now;
                post.PostStatus = "approved";

                db.FORUM_POSTS.Add(post);
                db.SaveChanges();

                ViewBag.Notification = "Successfully posted.";
                ViewBag.NotificationColor = "text-success";

                ModelState.Clear();
                return View(new FORUM_POSTS());
            }
            else
            {
                return View(post);
            }
        }
        public ActionResult Posts(string statusFilter, string searchString)
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
                    var posts = db.FORUM_POSTS.AsQueryable();

                    if (!string.IsNullOrEmpty(statusFilter))
                    {
                        posts = posts.Where(p => p.PostStatus == statusFilter);
                    }

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        posts = posts.Where(p => p.Title.Contains(searchString) || p.Content.Contains(searchString));
                    }
                    
                    ViewBag.CurrentFilter = searchString;
                    ViewBag.CurrentStatusFilter = statusFilter;
                    ViewBag.ResultsCount = posts.Count();
                    return View(posts.ToList());
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        
        public ActionResult DetailsPost(int id)
        {
            //FORUM_POSTS post = db.FORUM_POSTS.Find(id);
            var post = (from p in db.FORUM_POSTS
                        join u in db.FORUM_USERS on p.AuthorID equals u.UserID
                        where p.PostID == id // Filter by post ID
                        select new ForumPostViewModel
                        {
                            PostID = p.PostID,
                            Title = p.Title,
                            Content = p.Content,
                            CreatedAt = (DateTime)p.CreatedAt,
                            AuthorName = u.UserName,
                            Images = p.POST_IMAGE.Select(i => new ImageViewModel
                            {
                                ImagePath = i.ImagePath,
                                AltText = i.AltText
                            }).ToList()
                        }).FirstOrDefault();

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        public ActionResult DeletePost(int id)
        {
            var postController = new PostController();
            postController.Delete(id);

            return RedirectToAction("Posts");
        }

        public ActionResult Approve(int id)
        {
            var post = db.FORUM_POSTS.FirstOrDefault(p => p.PostID == id);

            if (post != null)
            {
                post.PostStatus = "approved";
                db.SaveChanges();
            }

            return RedirectToAction("Posts", "Admin");
        }
        #endregion

        #region Feedback

        [Authorize]
        public ActionResult Feedbacks(string searchString)
        {
            var feedbacks = from f in db.FEEDBACK
                            select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                feedbacks = feedbacks.Where(s => s.Content.Contains(searchString));
            }

            // Order by CreatedAt in descending order
            feedbacks = feedbacks.OrderByDescending(s => s.CreatedAt);

            ViewBag.CurrentFilter = searchString;
            return View(feedbacks.ToList());
        }

        public ActionResult DeleteFeedback(int id)
        {
            var feedback = db.FEEDBACK.Find(id);
            
            if (feedback != null)
            {
                //remove 
                db.FEEDBACK.Remove(feedback);
                //save db changes
                db.SaveChanges();
                //go back to the view / reload the page
                return RedirectToAction("Feedbacks", "Admin");
            }
            else
            {
                ViewBag.FBError = "There was an error with deleting the feedback (feedback missing..)";
                return RedirectToAction("Feedbacks");
            }
        }

        #endregion
    }

}