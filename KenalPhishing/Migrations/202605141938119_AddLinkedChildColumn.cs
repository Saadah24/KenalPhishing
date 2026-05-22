namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinkedChildColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LinkedChildId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LinkedChildId");
        }
    }
}
