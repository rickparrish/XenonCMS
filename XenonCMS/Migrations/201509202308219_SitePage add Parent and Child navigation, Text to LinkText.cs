namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SitePageaddParentandChildnavigationTexttoLinkText : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.SitePages", "Text", "LinkText");
            AlterColumn("dbo.SitePages", "ParentId", c => c.Int());
            CreateIndex("dbo.SitePages", "ParentId");
            AddForeignKey("dbo.SitePages", "ParentId", "dbo.SitePages", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SitePages", "ParentId", "dbo.SitePages");
            DropIndex("dbo.SitePages", new[] { "ParentId" });
            AlterColumn("dbo.SitePages", "ParentId", c => c.Int(nullable: false));
            RenameColumn("dbo.SitePages", "LinkText", "Text");
        }
    }
}
