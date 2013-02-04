namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemovePageTypeField : DbMigration
    {
        public override void Up()
        {
            DropColumn("Page", "PageTypeId");
        }
        
        public override void Down()
        {
            AddColumn("Page", "PageTypeId", c => c.Int(nullable: false));
        }
    }
}
