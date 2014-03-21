using System.Collections.Generic;

namespace XenonCMS.Models
{
    public partial class Site
    {
        public int Id { get; set; }
        public string ContactEmail { get; set; }
        public string Domain { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public bool NavBarInverted { get; set; }
        public string Sidebar { get; set; }

        public virtual List<SiteAdminIP> AdminIPs { get; set; }
        public virtual List<SiteBlogPost> BlogPosts { get; set; }
        public virtual List<SitePage> Pages { get; set; }
        public virtual List<SiteGlobalSidebar> SiteGlobalSidebars { get; set; }
    }
}
