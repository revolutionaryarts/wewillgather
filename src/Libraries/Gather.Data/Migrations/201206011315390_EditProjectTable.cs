namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EditProjectTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("Project", "Active");
            DropColumn("Project", "Deleted");
        }
        
        public override void Down()
        {
            AddColumn("Project", "Deleted", c => c.Boolean(nullable: false));
            AddColumn("Project", "Active", c => c.Boolean(nullable: false));
        }
    }
}
