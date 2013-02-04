namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangeProjectLatLngPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Project", "Latitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
            AlterColumn("Project", "Longitude", c => c.Decimal(nullable: false, precision: 25, scale: 18));
        }
        
        public override void Down()
        {
            AlterColumn("Project", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Project", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
