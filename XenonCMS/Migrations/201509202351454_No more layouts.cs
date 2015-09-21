namespace XenonCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nomorelayouts : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SitePages", "Layout");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SitePages", "Layout", c => c.String());
        }
    }
}
