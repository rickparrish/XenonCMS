using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using XenonCMS.Classes;
using XenonCMS.Models;

namespace XenonCMS.Areas.Admin.ViewModels.Pages
{
    public class Edit
    {
        [Required]
        public int Id { get; set; }
        [Required, Display(Name = "Page heading")]
        public string Title { get; set; }
        [Required, Display(Name = "Link text")]
        public string LinkText { get; set; }
        [Required]
        public string Slug { get; set; }
        [Display(Name = "Parent page")]
        public SelectList Parents { get; set; }
        [Required]
        public int ParentId { get; set; }
        [Required, Display(Name = "Only admin can view?")]
        public bool? RequireAdmin { get; set; }
        [Required, AllowHtml, Display(Name = "Content")]
        public string Html { get; set; }
        [Required, Display(Name = "Show page heading?")]
        public bool? ShowTitleOnPage { get; set; }
        [Required, Display(Name = "Show page in menu?")]
        public bool? ShowInMenu { get; set; }
        [Required, Display(Name = "Right align in menu?")]
        public bool? RightAlign { get; set; }
        [Required, Display(Name = "Display order in menu")]
        public int DisplayOrder { get; set; }

        internal void GetParents(ApplicationDbContext db)
        {
            var ParentItems = db.SitePages.Where(x => (x.ParentId == null) && (x.Id != Id)).OrderBy(x => x.LinkText).ToList();

            SitePage NoParent = new SitePage();
            NoParent.Id = 0;
            NoParent.LinkText = "No Parent";
            ParentItems.Insert(0, NoParent);

            Parents = new SelectList(ParentItems, "Id", "LinkText");
        }
    }
}