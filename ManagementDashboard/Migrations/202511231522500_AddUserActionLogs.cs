namespace ManagementDashboard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserActionLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserActionLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimeUtc = c.DateTime(nullable: false),
                        UserName = c.String(),
                        ControllerName = c.String(),
                        ActionName = c.String(),
                        HttpMethod = c.String(),
                        UrlAccessed = c.String(),
                        AccessedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserActionLogs");
        }
    }
}
