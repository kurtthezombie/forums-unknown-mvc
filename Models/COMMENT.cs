//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ForumsUnknown.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class COMMENT
    {
        public int CommentID { get; set; }

        [Required]
        public string Content { get; set; }

        public int AuthorID { get; set; }

        public int PostID { get; set; }

        public System.DateTime CreatedAt { get; set; }

        public Nullable<System.DateTime> ModifiedAt { get; set; }
    
        public virtual FORUM_USERS FORUM_USERS { get; set; }
        public virtual FORUM_POSTS FORUM_POSTS { get; set; }
    }
}
