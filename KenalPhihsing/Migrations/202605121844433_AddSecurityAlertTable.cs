namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSecurityAlertTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecurityAlerts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Severity = c.String(),
                        DatePublished = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SecurityAlerts");
        }
    }
}
