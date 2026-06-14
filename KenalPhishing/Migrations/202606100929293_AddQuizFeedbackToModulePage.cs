namespace KenalPhishing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizFeedbackToModulePage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModulePages", "QuizFeedback", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModulePages", "QuizFeedback");
        }
    }
}
