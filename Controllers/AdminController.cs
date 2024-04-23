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
    public class AdminController : Controller
    {
        private FuDBContext db = new FuDBContext();

        // GET: Admin
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
            if (ModelState.IsValid)
            {
                db.FORUM_USERS.Add(user);
                db.SaveChanges();

                return View();
            }
            else
            {
                return View(user);
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
            var data = db.FORUM_USERS.Find(id);

            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.FORUM_USERS.Remove(data);
                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        private bool IsUsernameUnique(int userId, string newUserName)
        {
            var existingUser = db.FORUM_USERS.FirstOrDefault(u => u.UserID == userId);

            return existingUser == null || existingUser.UserName == newUserName || 
                !db.FORUM_USERS.Any(u => u.UserID != userId && u.UserName == newUserName);
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

        #endregion

        #region Posts

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
        
        public ActionResult DetailsPost(int? id)
        {
            FORUM_POSTS post = db.FORUM_POSTS.Find(id);

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
        #endregion

    }

}