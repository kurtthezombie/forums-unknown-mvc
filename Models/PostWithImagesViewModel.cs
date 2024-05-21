using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ForumsUnknown.Models
{
    public class PostWithImagesViewModel
    {
        public int PostID { get; set; }

        [Required]
        public int AuthorID { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Content")]
        public string Content {  get; set; }

        [DisplayName("Status")]
        public string PostStatus { get; set; }

        [DisplayName("Date Posted")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Image Files")]
        //public IFormFileCollection Images { get; set; }
        public List<HttpPostedFileBase> Images { get; set; }
    }
}