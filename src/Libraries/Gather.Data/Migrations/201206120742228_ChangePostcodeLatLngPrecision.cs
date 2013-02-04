namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangePostcodeLatLngPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Postcode", "Latitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
            AlterColumn("Postcode", "Longitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
        }
        
        public override void Down()
        {
            AlterColumn("Postcode", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Postcode", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
