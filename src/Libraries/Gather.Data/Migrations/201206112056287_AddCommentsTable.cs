namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Comment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.Int(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        UserComment = c.String(),
                        ModerationRequestCount = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Author_Id = c.Int(nullable: false),
                        Project_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("User", t => t.Author_Id, cascadeDelete: true)
                .ForeignKey("Project", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Author_Id)
                .Index(t => t.Project_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Comment", new[] { "Project_Id" });
            DropIndex("Comment", new[] { "Author_Id" });
            DropForeignKey("Comment", "Project_Id", "Project");
            DropForeignKey("Comment", "Author_Id", "User");
            DropTable("Comment");
        }
    }
}
