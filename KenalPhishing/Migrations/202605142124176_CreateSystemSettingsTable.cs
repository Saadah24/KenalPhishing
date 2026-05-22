namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSystemSettingsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsMaintenanceMode = c.Boolean(nullable: false),
                        GlobalAnnouncement = c.String(),
                        ScoreChild = c.Int(nullable: false),
                        ScoreAdult = c.Int(nullable: false),
                        ScoreElder = c.Int(nullable: false),
                        QuizAttempts = c.Int(nullable: false),
                        SessionTimeout = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemSettings");
        }
    }
}
