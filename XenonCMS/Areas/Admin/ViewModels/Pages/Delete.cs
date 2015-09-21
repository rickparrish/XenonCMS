using System;
using System.ComponentModel.DataAnnotations;

namespace XenonCMS.Areas.Admin.ViewModels.Pages
{
    public class Delete
    {
        [Display(Name = "Link text")]
        public string LinkText { get; set; }
        public string Slug { get; set; }
        [Display(Name = "Page heading")]
        public string Title { get; set; }
        [Display(Name = "Content")]
        public string Html { get; set; }
    }
}