namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddScheduledTaskRunOnLoad : DbMigration
    {
        public override void Up()
        {
            AddColumn("ScheduleTask", "RunOnLoad", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ScheduleTask", "RunOnLoad");
        }
    }
}
