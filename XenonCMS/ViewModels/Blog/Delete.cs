using System;
using System.ComponentModel.DataAnnotations;

namespace XenonCMS.ViewModels.Blog
{
    public class Delete
    {
        public string Title { get; set; }
        [Display(Name = "Content")]
        public string FullPostText { get; set; }
        [DataType(DataType.Date), Display(Name = "Date Posted"), DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime DatePosted { get; set; }
    }
}