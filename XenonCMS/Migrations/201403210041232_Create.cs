namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GlobalAdminIPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            Sql("INSERT INTO GlobalAdminIPs (Address) VALUES ('::1')");
            Sql("INSERT INTO GlobalAdminIPs (Address) VALUES ('127.0.0.1')");

            CreateTable(
                "dbo.GlobalSidebars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Html = c.String(),
                        DisplayOrder = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        DateLastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteGlobalSidebars",
                c => new
                    {
                        SiteId = c.Int(nullable: false),
                        GlobalSidebarId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SiteId, t.GlobalSidebarId })
                .ForeignKey("dbo.GlobalSidebars", t => t.GlobalSidebarId, cascadeDelete: true)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.GlobalSidebarId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContactEmail = c.String(),
                        Domain = c.String(),
                        Title = c.String(),
                        Theme = c.String(),
                        NavBarInverted = c.Boolean(nullable: false),
                        Sidebar = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SiteAdminIPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.SiteBlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        Title = c.String(),
                        FullPostText = c.String(),
                        PreviewText = c.String(),
                        Slug = c.String(),
                        DatePosted = c.DateTime(nullable: false),
                        DateLastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.SitePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                        Title = c.String(),
                        Text = c.String(),
                        Slug = c.String(),
                        Html = c.String(),
                        Layout = c.String(),
                        ShowTitleOnPage = c.Boolean(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        RightAlign = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        RequireAdmin = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        DateLastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteGlobalSidebars", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SitePages", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteAdminIPs", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebarId", "dbo.GlobalSidebars");
            DropIndex("dbo.SiteGlobalSidebars", new[] { "SiteId" });
            DropIndex("dbo.SitePages", new[] { "SiteId" });
            DropIndex("dbo.SiteBlogPosts", new[] { "SiteId" });
            DropIndex("dbo.SiteAdminIPs", new[] { "SiteId" });
            DropIndex("dbo.SiteGlobalSidebars", new[] { "GlobalSidebarId" });
            DropTable("dbo.SitePages");
            DropTable("dbo.SiteBlogPosts");
            DropTable("dbo.SiteAdminIPs");
            DropTable("dbo.Sites");
            DropTable("dbo.SiteGlobalSidebars");
            DropTable("dbo.GlobalSidebars");
            DropTable("dbo.GlobalAdminIPs");
        }
    }
}
