namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddScheduledTask : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ScheduleTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Seconds = c.Int(nullable: false),
                        Type = c.String(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        StopOnError = c.Boolean(nullable: false),
                        LastStartUtc = c.DateTime(),
                        LastEndUtc = c.DateTime(),
                        LastSuccessUtc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("ScheduleTask");
        }
    }
}
