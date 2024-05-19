using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumsUnknown.Models
{
    public class UserWithPosts
    {
        public FORUM_USERS User { get; set; }
        public List<FORUM_POSTS> Posts { get; set; }
    }
}