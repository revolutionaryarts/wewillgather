namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModerationQueue : DbMigration
    {
        public override void Up()
        {
            DropColumn("ModerationQueue", "Active");
            DropColumn("ModerationQueue", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("ModerationQueue", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("ModerationQueue", "Active", c => c.Boolean(nullable: false));
        }
    }
}
