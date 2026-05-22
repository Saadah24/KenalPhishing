namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSecurityAlert : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecurityAlerts", "ScammerInfo", c => c.String());
            AddColumn("dbo.SecurityAlerts", "ExampleMessage", c => c.String());
            AlterColumn("dbo.SecurityAlerts", "Title", c => c.String());
            AlterColumn("dbo.SecurityAlerts", "Content", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SecurityAlerts", "Content", c => c.String(nullable: false));
            AlterColumn("dbo.SecurityAlerts", "Title", c => c.String(nullable: false));
            DropColumn("dbo.SecurityAlerts", "ExampleMessage");
            DropColumn("dbo.SecurityAlerts", "ScammerInfo");
        }
    }
}
