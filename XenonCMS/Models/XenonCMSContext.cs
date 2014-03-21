using System.Data.Entity;

namespace XenonCMS.Models
{
    public partial class XenonCMSContext : DbContext
    {
        public XenonCMSContext()
            : base("XenonCMSContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<XenonCMSContext, XenonCMS.Migrations.Configuration>());
        }

        public virtual DbSet<GlobalAdminIP> GlobalAdminIPs { get; set; }
        public virtual DbSet<GlobalSidebar> GlobalSidebars { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteAdminIP> SiteAdminIPs { get; set; }
        public virtual DbSet<SiteBlogPost> SiteBlogPosts { get; set; }
        public virtual DbSet<SiteGlobalSidebar> SiteGlobalSidebars { get; set; }
        public virtual DbSet<SitePage> SitePages { get; set; }
    }
}
