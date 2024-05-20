using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ForumsUnknown.Models
{
    public class CommentViewModel
    {
        public int CommentID { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorName { get; set; } // Changed from AuthorID to AuthorName
        public string ProfilePicPath { get; set; }

        public int? PostID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}