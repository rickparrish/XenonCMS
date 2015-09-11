using System.Collections.Generic;

namespace XenonCMS.Models
{
    // TODO Allow a single site to be served via multiple domains
    //      Maybe have option for specifying "redirect to primary" for secondary domains (ie redirect a .com to .ca)
    // TODO Sites should be able to have any number of global and site sidebars (right now limited to just 1 site sidebar)
    //      Allow them to be mixed, ie global 1 then site 2 then global 2 then site 1 or whatever
    // TODO Drop ContactEmail and just email each of the Admins?
    public partial class Site
    {
        public int Id { get; set; }
        public string ContactEmail { get; set; }
        public string Domain { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public bool NavBarInverted { get; set; }
        public string Sidebar { get; set; }

        public virtual List<ApplicationUser> Admins { get; set; }
        public virtual List<SiteBlogPost> BlogPosts { get; set; }
        public virtual List<SitePage> Pages { get; set; }
        public virtual List<SiteGlobalSidebar> SiteGlobalSidebars { get; set; }
    }
}
