namespace KenalPhishing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TambahSystemSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemSettings", "LastUpdated", c => c.DateTime());
            AddColumn("dbo.SystemSettings", "UpdatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemSettings", "UpdatedBy");
            DropColumn("dbo.SystemSettings", "LastUpdated");
        }
    }
}
