namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCoordinateFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("Postcode", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Postcode", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Project", "Latitude", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Project", "Longitude", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Project", "Longitude");
            DropColumn("Project", "Latitude");
            DropColumn("Postcode", "Longitude");
            DropColumn("Postcode", "Latitude");
        }
    }
}
