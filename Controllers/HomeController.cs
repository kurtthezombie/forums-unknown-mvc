﻿using ForumsUnknown.Models;
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

        //GET 
        public ActionResult Index()
        {
            var posts = (from p in db.FORUM_POSTS
                         join u in db.FORUM_USERS on p.AuthorID equals u.UserID
                         where p.PostStatus == "approved"
                         orderby p.CreatedAt descending
                         select new ForumPostViewModel
                         {
                             PostID = p.PostID,
                             Title = p.Title,
                             Content = p.Content,
                             CreatedAt = (DateTime)p.CreatedAt,
                             ModifiedAt = (DateTime)p.ModifiedAt,
                             AuthorName = u.UserName,
                             ProfilePicPath = u.ProfilePicPath,
                             Images = db.POST_IMAGE
                                        .Where(img => img.PostID == p.PostID)
                                        .Select(img => new ImageViewModel
                                        {
                                            ImagePath = img.ImagePath,
                                            AltText = img.AltText
                                        }).ToList()
                         }).ToList();
            return View(posts);
        }

        [Route("About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}