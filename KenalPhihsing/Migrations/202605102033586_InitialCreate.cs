namespace KenalPhihsing.Migrations
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
                        SimulationText = c.String(),
                        QuizQuestion = c.String(),
                        QuizAnswerCorrect = c.String(),
                        QuizAnswerWrong = c.String(),
                        CorrectFeedback = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModulePages", "ModuleId", "dbo.Modules");
            DropIndex("dbo.ModulePages", new[] { "ModuleId" });
            DropTable("dbo.Modules");
            DropTable("dbo.ModulePages");
        }
    }
}
