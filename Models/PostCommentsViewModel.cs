using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumsUnknown.Models
{
    public class PostCommentsViewModel
    {
        public ForumPostViewModel ForumPost { get; set; }
        public List<COMMENT> Comments { get; set; }

        public COMMENT Comment { get; set; }
    }
}