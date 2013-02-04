namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakingCommentAuthorOptional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Comment", "Author_Id", "User");
            DropIndex("Comment", new[] { "Author_Id" });
            AlterColumn("Comment", "Author_Id", c => c.Int());
            AddForeignKey("Comment", "Author_Id", "User", "Id");
            CreateIndex("Comment", "Author_Id");
        }
        
        public override void Down()
        {
            DropIndex("Comment", new[] { "Author_Id" });
            DropForeignKey("Comment", "Author_Id", "User");
            AlterColumn("Comment", "Author_Id", c => c.Int(nullable: false));
            CreateIndex("Comment", "Author_Id");
            AddForeignKey("Comment", "Author_Id", "User", "Id", cascadeDelete: true);
        }
    }
}
