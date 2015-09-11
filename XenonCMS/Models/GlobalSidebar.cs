using System;
using System.Collections.Generic;

namespace XenonCMS.Models
{
    public partial class GlobalSidebar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public virtual ICollection<Site> Sites { get; set; }
    }
}
