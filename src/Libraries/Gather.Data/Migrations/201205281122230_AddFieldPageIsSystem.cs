namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldPageIsSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "IsSystemPage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Page", "IsSystemPage");
        }
    }
}
