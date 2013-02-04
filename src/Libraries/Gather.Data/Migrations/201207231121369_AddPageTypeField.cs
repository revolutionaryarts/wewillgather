namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPageTypeField : DbMigration
    {
        public override void Up()
        {
            AddColumn("Page", "PageTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Page", "PageTypeId");
        }
    }
}
