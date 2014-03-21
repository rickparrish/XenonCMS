using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XenonCMS.ViewModels.Contact
{
    public class Index
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        public string Url { get; set; }
        [Required, AllowHtml]
        public string Body { get; set; }
    }
}