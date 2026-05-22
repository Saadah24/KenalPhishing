namespace KenalPhihsing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScamReportTable1 : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScamReports", "UserId", "dbo.Users");
            DropIndex("dbo.ScamReports", new[] { "UserId" });
            DropTable("dbo.ScamReports");
        }
    }
}
