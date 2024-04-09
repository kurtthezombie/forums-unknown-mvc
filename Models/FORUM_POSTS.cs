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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class FORUM_POSTS
    {
        public int PostID { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Content")]
        public string Content { get; set; }

        [DisplayName("Date Posted")]
        public Nullable<System.DateTime> CreatedAt { get; set; }

        [Required]
        public int AuthorID { get; set; }
    

        public virtual FORUM_USERS FORUM_USERS { get; set; }
    }
}
