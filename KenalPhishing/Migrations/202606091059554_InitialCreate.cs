namespace KenalPhishing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModulePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        PageNumber = c.Int(nullable: false),
                        PageType = c.String(),
                        PageTitle = c.String(),
                        BodyContent = c.String(),
                        SimSender = c.String(),
                        SimSubject = c.String(),
                        SimBody = c.String(),
                        SimPhishLink = c.String(),
                        SimClues = c.String(),
                        SimType = c.String(),
                        QuizQuestion = c.String(),
                        OptionA = c.String(),
                        OptionB = c.String(),
                        OptionC = c.String(),
                        OptionD = c.String(),
                        CorrectAnswer = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        ModuleCode = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        Icon = c.String(),
                        ThemeColor = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScamReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Category = c.String(),
                        ScammerInfo = c.String(),
                        Description = c.String(),
                        DateReported = c.DateTime(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Category = c.String(),
                        Role = c.String(),
                        IsParent = c.Boolean(nullable: false),
                        ChildEmail = c.String(),
                        ProfilePicture = c.String(),
                        LinkedChildId = c.Int(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SecurityAlerts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Severity = c.String(),
                        DatePublished = c.DateTime(nullable: false),
                        ScammerInfo = c.String(),
                        ExampleMessage = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateTable(
                "dbo.UserActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ActivityType = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ActionUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        LoginDate = c.DateTime(nullable: false, storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProgresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        ProgressPercent = c.Int(nullable: false),
                        CertificateIssued = c.Boolean(nullable: false),
                        CertificateUrl = c.String(),
                        SimResult = c.String(),
                        QuizScore = c.Int(nullable: false),
                        QuizTotal = c.Int(nullable: false),
                        CompletedAt = c.DateTime(),
                        LastAccessedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProgresses", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserActivities", "UserId", "dbo.Users");
            DropForeignKey("dbo.ScamReports", "UserId", "dbo.Users");
            DropForeignKey("dbo.ModulePages", "ModuleId", "dbo.Modules");
            DropIndex("dbo.UserProgresses", new[] { "ModuleId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.UserActivities", new[] { "UserId" });
            DropIndex("dbo.ScamReports", new[] { "UserId" });
            DropIndex("dbo.ModulePages", new[] { "ModuleId" });
            DropTable("dbo.UserProgresses");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserActivities");
            DropTable("dbo.SystemSettings");
            DropTable("dbo.SecurityAlerts");
            DropTable("dbo.Users");
            DropTable("dbo.ScamReports");
            DropTable("dbo.Modules");
            DropTable("dbo.ModulePages");
        }
    }
}
