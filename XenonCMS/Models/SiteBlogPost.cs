using System;

namespace XenonCMS.Models
{
    public partial class SiteBlogPost
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Title { get; set; }
        public string FullPostText { get; set; }
        public string PreviewText { get; set; }
        public string Slug { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public virtual Site Site { get; set; }

        public object ToRouteValues()
        {
            return new { id = Id, year = DatePosted.Year, month = DatePosted.Month.ToString("D2"), day = DatePosted.Day.ToString("D2"), slug = Slug };
        }
    }
}
