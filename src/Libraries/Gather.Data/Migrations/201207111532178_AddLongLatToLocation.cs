namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddLongLatToLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Location", "Latitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
            AddColumn("Location", "Longitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
        }
        
        public override void Down()
        {
            DropColumn("Location", "Longitude");
            DropColumn("Location", "Latitude");
        }
    }
}
