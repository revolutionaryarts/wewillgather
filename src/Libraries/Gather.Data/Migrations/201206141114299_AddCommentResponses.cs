namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentResponses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Comment", "Project_Id", "Project");
            DropIndex("Comment", new[] { "Project_Id" });
            AddColumn("Comment", "InResponseTo_Id", c => c.Int());
            AlterColumn("Comment", "Project_Id", c => c.Int());
            AddForeignKey("Comment", "InResponseTo_Id", "Comment", "Id");
            AddForeignKey("Comment", "Project_Id", "Project", "Id");
            CreateIndex("Comment", "InResponseTo_Id");
            CreateIndex("Comment", "Project_Id");
        }
        
        public override void Down()
        {
            DropIndex("Comment", new[] { "Project_Id" });
            DropIndex("Comment", new[] { "InResponseTo_Id" });
            DropForeignKey("Comment", "Project_Id", "Project");
            DropForeignKey("Comment", "InResponseTo_Id", "Comment");
            AlterColumn("Comment", "Project_Id", c => c.Int(nullable: false));
            DropColumn("Comment", "InResponseTo_Id");
            CreateIndex("Comment", "Project_Id");
            AddForeignKey("Comment", "Project_Id", "Project", "Id", cascadeDelete: true);
        }
    }
}
