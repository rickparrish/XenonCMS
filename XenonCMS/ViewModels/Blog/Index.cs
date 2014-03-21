using System;

namespace XenonCMS.ViewModels.Blog
{
    public class Index
    {
        public string Title { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public string PreviewText { get; set; }
        public string FullPostText { get; set; }
        public int Id { get; set; }
        public string Slug { get; set; }

        public object ToRouteValues()
        {
            return new { id = Id, year = DatePosted.Year, month = DatePosted.Month.ToString("D2"), day = DatePosted.Day.ToString("D2"), slug = Slug };
        }
    }
}