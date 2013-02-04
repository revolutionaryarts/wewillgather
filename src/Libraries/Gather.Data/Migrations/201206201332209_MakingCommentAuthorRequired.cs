namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakingCommentAuthorRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Comment", "Author_Id", "User");
            DropIndex("Comment", new[] { "Author_Id" });
            AlterColumn("Comment", "Author_Id", c => c.Int(nullable: false));
            AddForeignKey("Comment", "Author_Id", "User", "Id", cascadeDelete: true);
            CreateIndex("Comment", "Author_Id");
        }
        
        public override void Down()
        {
            DropIndex("Comment", new[] { "Author_Id" });
            DropForeignKey("Comment", "Author_Id", "User");
            AlterColumn("Comment", "Author_Id", c => c.Int());
            CreateIndex("Comment", "Author_Id");
            AddForeignKey("Comment", "Author_Id", "User", "Id");
        }
    }
}
