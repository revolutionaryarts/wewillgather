namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddLocationRegionFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("Location", "IsRegion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Location", "IsRegion");
        }
    }
}
