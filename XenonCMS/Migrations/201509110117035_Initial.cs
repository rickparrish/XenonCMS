namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars");
            DropForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites");
            DropIndex("dbo.SiteGlobalSidebars", new[] { "SiteId" });
            DropIndex("dbo.SiteGlobalSidebars", new[] { "GlobalSidebarId" });
            CreateTable(
                "dbo.SiteGlobalSidebars",
                c => new
                    {
                        Site_Id = c.Int(nullable: false),
                        GlobalSidebar_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Site_Id, t.GlobalSidebar_Id })
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .ForeignKey("dbo.GlobalSidebars", t => t.GlobalSidebar_Id)
                .Index(t => t.Site_Id)
                .Index(t => t.GlobalSidebar_Id);
            
            DropTable("dbo.SiteGlobalSidebars");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SiteGlobalSidebars",
                c => new
                    {
                        SiteId = c.Int(nullable: false),
                        GlobalSidebarId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SiteId, t.GlobalSidebarId });
            
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebar_Id", "dbo.GlobalSidebars");
            DropForeignKey("dbo.SiteGlobalSidebars", "Site_Id", "dbo.Sites");
            DropIndex("dbo.SiteGlobalSidebars", new[] { "GlobalSidebar_Id" });
            DropIndex("dbo.SiteGlobalSidebars", new[] { "Site_Id" });
            DropTable("dbo.SiteGlobalSidebars");
            CreateIndex("dbo.SiteGlobalSidebars", "GlobalSidebarId");
            CreateIndex("dbo.SiteGlobalSidebars", "SiteId");
            AddForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites", "Id");
            AddForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars", "Id");
        }
    }
}
