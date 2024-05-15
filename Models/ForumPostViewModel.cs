using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumsUnknown.Models
{
    public class ForumPostViewModel
    {
        public int PostID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string AuthorName { get; set; }
    }
}