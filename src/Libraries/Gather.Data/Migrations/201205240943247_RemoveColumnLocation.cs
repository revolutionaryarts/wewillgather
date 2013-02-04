namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveColumnLocation : DbMigration
    {
        public override void Up()
        {
            DropColumn("Location", "CreatedBy");
            DropColumn("Postcode", "CreatedBy");
        }
        
        public override void Down()
        {
            AddColumn("Postcode", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("Location", "CreatedBy", c => c.Int(nullable: false));
        }
    }
}
