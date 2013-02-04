namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectChildFriendlyField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "ChildFriendly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Project", "ChildFriendly");
        }
    }
}
