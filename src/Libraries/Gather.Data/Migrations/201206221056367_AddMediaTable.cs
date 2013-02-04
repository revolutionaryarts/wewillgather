namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMediaTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Media",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        EntityTypeId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 255),
                        UploadedDate = c.DateTime(nullable: false),
                        UploadedById = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Media");
        }
    }
}
