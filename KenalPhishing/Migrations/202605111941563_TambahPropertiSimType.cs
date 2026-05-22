namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TambahPropertiSimType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModulePages", "SimType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModulePages", "SimType");
        }
    }
}
