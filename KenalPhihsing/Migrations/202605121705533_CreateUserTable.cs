namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsParent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "LinkedChildEmail", c => c.String());
            AddColumn("dbo.Users", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Role");
            DropColumn("dbo.Users", "LinkedChildEmail");
            DropColumn("dbo.Users", "IsParent");
        }
    }
}
