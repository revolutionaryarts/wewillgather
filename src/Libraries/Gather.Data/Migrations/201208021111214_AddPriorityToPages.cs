namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPriorityToPages : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Page", "Priority");
        }
    }
}
