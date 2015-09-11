using System;

namespace XenonCMS.Models
{
    public partial class SitePage
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Slug { get; set; }
        public string Html { get; set; }
        public string Layout { get; set; }
        public bool ShowTitleOnPage { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RightAlign { get; set; }
        public int DisplayOrder { get; set; }
        public bool RequireAdmin { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public virtual Site Site { get; set; }
    }
}
