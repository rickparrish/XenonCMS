using System;

namespace XenonCMS.Areas.Admin.ViewModels.Pages
{
    public class Index
    {
        public DateTime DateAdded { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int DisplayOrder { get; set; }
        public string Html { get; set; }
        public int Id { get; set; }
        public string LinkText { get; set; }
        public string Slug { get; set; }
        public int ParentId { get; set; }
        public bool RequireAdmin { get; set; }
        public bool RightAlign { get; set; }
        public bool ShowInMenu { get; set; }
        public bool ShowTitleOnPage { get; set; }
        public string Title { get; set; }
    }
}