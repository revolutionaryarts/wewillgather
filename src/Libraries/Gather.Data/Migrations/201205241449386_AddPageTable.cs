namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPageTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.Int(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        MetaDescription = c.String(maxLength: 150),
                        MetaKeywords = c.String(maxLength: 500),
                        MetaTitle = c.String(maxLength: 100),
                        Title = c.String(maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Page");
        }
    }
}
