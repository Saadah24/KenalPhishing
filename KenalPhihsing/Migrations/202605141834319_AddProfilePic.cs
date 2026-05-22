namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfilePic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ProfilePicture", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ProfilePicture");
        }
    }
}
