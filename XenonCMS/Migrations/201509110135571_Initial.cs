namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Site_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Site_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
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
                .ForeignKey("dbo.Sites", t => t.SiteId)
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
                .ForeignKey("dbo.Sites", t => t.SiteId)
                .Index(t => t.SiteId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.SitePages", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.SiteGlobalSidebars", "GlobalSidebar_Id", "dbo.GlobalSidebars");
            DropForeignKey("dbo.SiteGlobalSidebars", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.SiteBlogPosts", "SiteId", "dbo.Sites");
            DropForeignKey("dbo.AspNetUsers", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SiteGlobalSidebars", new[] { "GlobalSidebar_Id" });
            DropIndex("dbo.SiteGlobalSidebars", new[] { "Site_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SitePages", new[] { "SiteId" });
            DropIndex("dbo.SiteBlogPosts", new[] { "SiteId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Site_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropTable("dbo.SiteGlobalSidebars");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.SitePages");
            DropTable("dbo.SiteBlogPosts");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Sites");
            DropTable("dbo.GlobalSidebars");
        }
    }
}
