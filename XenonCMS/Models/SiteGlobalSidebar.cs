using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XenonCMS.Models
{
    public partial class SiteGlobalSidebar
    {
        [Key, Column(Order = 0)]
        public int SiteId { get; set; }
        [Key, Column(Order = 1)]
        public int GlobalSidebarId { get; set; }

        public virtual Site Site { get; set; }
        public virtual GlobalSidebar GlobalSidebar { get; set; }
    }
}
