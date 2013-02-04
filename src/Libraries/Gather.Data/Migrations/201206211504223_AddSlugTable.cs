namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddSlugTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Slug",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlugUrl = c.String(),
                        SuccessStoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Slug");
        }
    }
}
