using System;
using System.Collections.Generic;

namespace XenonCMS.Models
{
    public partial class SitePage
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string LinkText { get; set; }
        public string Slug { get; set; }
        public string Html { get; set; }
        public bool ShowTitleOnPage { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RightAlign { get; set; }
        public int DisplayOrder { get; set; }
        public bool RequireAdmin { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public virtual List<SitePage> Children { get; set; }
        public virtual SitePage Parent { get; set; }
        public virtual Site Site { get; set; }
    }
}
