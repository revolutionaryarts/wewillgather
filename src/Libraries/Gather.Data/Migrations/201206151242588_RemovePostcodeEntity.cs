namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemovePostcodeEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Postcode", "Location_Id", "Location");
            DropIndex("Postcode", new[] { "Location_Id" });
            DropTable("Postcode");
        }
        
        public override void Down()
        {
            CreateTable(
                "Postcode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 15),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.Int(),
                        LastModifiedDate = c.DateTime(),
                        Latitude = c.Decimal(nullable: false, precision: 25, scale: 18),
                        Longitude = c.Decimal(nullable: false, precision: 25, scale: 18),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Location_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("Postcode", "Location_Id");
            AddForeignKey("Postcode", "Location_Id", "Location", "Id", cascadeDelete: true);
        }
    }
}
