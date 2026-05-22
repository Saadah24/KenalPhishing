namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParentFeatures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ChildEmail", c => c.String());
            DropColumn("dbo.Users", "LinkedChildEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "LinkedChildEmail", c => c.String());
            DropColumn("dbo.Users", "ChildEmail");
        }
    }
}
