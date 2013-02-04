namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectTable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "AlertMessageSent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Project", "AlertMessageSent");
        }
    }
}
