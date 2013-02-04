namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectRecurrenceFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "IsRecurring", c => c.Boolean(nullable: false));
            AddColumn("Project", "RecurrenceIntervalId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Project", "RecurrenceIntervalId");
            DropColumn("Project", "IsRecurring");
        }
    }
}
