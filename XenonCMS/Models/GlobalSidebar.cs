using System.Collections.Generic;

namespace XenonCMS.Models
{
    public partial class GlobalSidebar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public int DisplayOrder { get; set; }
        public System.DateTime DateAdded { get; set; }
        public System.DateTime DateLastUpdated { get; set; }

        public virtual ICollection<SiteGlobalSidebar> SiteGlobalSidebars { get; set; }
    }
}
