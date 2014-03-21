using System;
using System.ComponentModel.DataAnnotations;

namespace XenonCMS.ViewModels.Blog
{
    public class Details
    {
        public string Title { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime DatePosted { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime DateLastUpdated { get; set; }
        public string FullPostText { get; set; }
        public int Id { get; set; }
        public string Slug { get; set; }
        
        public object ToRouteValues()
        {
            return new { id = Id, year = DatePosted.Year, month = DatePosted.Month.ToString("D2"), day = DatePosted.Day.ToString("D2"), slug = Slug };
        }
    }
}