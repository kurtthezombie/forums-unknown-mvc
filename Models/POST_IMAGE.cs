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
    
    public partial class POST_IMAGE
    {
        public int ImageID { get; set; }
        public string AltText { get; set; }
        public string ImagePath { get; set; }
        public int PostID { get; set; }
    
        public virtual FORUM_POSTS FORUM_POSTS { get; set; }
    }
}
