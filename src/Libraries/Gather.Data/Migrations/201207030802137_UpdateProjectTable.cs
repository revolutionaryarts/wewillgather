namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "ReminderMessageSent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Project", "ReminderMessageSent");
        }
    }
}
