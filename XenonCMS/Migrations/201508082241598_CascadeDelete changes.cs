namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeletechanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars");
            DropForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteAdminIPs", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SitePages", "SiteId", "dbo.Sites");
            AddForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars", "Id");
            AddForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites", "Id");
            AddForeignKey("dbo.SiteAdminIPs", "SiteId", "dbo.Sites", "Id");
            AddForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites", "Id");
            AddForeignKey("dbo.SitePages", "SiteId", "dbo.Sites", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SitePages", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteAdminIPs", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars");
            AddForeignKey("dbo.SitePages", "SiteId", "dbo.Sites", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SiteAdminIPs", "SiteId", "dbo.Sites", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars", "Id", cascadeDelete: true);
        }
    }
}
