namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KemaskiniFieldKuizDanSimulasi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModulePages", "OptionA", c => c.String());
            AddColumn("dbo.ModulePages", "OptionB", c => c.String());
            AddColumn("dbo.ModulePages", "OptionC", c => c.String());
            AddColumn("dbo.ModulePages", "OptionD", c => c.String());
            AddColumn("dbo.ModulePages", "CorrectAnswer", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModulePages", "CorrectAnswer");
            DropColumn("dbo.ModulePages", "OptionD");
            DropColumn("dbo.ModulePages", "OptionC");
            DropColumn("dbo.ModulePages", "OptionB");
            DropColumn("dbo.ModulePages", "OptionA");
        }
    }
}
