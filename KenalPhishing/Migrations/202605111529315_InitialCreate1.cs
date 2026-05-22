namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModulePages", "SimSender", c => c.String());
            AddColumn("dbo.ModulePages", "SimSubject", c => c.String());
            AddColumn("dbo.ModulePages", "SimBody", c => c.String());
            AddColumn("dbo.ModulePages", "SimPhishLink", c => c.String());
            AddColumn("dbo.ModulePages", "SimClues", c => c.String());
            DropColumn("dbo.ModulePages", "SimulationText");
            DropColumn("dbo.ModulePages", "QuizAnswerCorrect");
            DropColumn("dbo.ModulePages", "QuizAnswerWrong");
            DropColumn("dbo.ModulePages", "CorrectFeedback");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModulePages", "CorrectFeedback", c => c.String());
            AddColumn("dbo.ModulePages", "QuizAnswerWrong", c => c.String());
            AddColumn("dbo.ModulePages", "QuizAnswerCorrect", c => c.String());
            AddColumn("dbo.ModulePages", "SimulationText", c => c.String());
            DropColumn("dbo.ModulePages", "SimClues");
            DropColumn("dbo.ModulePages", "SimPhishLink");
            DropColumn("dbo.ModulePages", "SimBody");
            DropColumn("dbo.ModulePages", "SimSubject");
            DropColumn("dbo.ModulePages", "SimSender");
        }
    }
}
