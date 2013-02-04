namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChangeRequest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ProjectChangeRequest", "OriginalProject_Id", "Project");
            DropIndex("ProjectChangeRequest", new[] { "OriginalProject_Id" });
            AddColumn("ProjectChangeRequest", "ParentProject_Id", c => c.Int());
            AddForeignKey("ProjectChangeRequest", "ParentProject_Id", "Project", "Id");
            CreateIndex("ProjectChangeRequest", "ParentProject_Id");
            DropColumn("ProjectChangeRequest", "OriginalProject_Id");
        }
        
        public override void Down()
        {
            AddColumn("ProjectChangeRequest", "OriginalProject_Id", c => c.Int());
            DropIndex("ProjectChangeRequest", new[] { "ParentProject_Id" });
            DropForeignKey("ProjectChangeRequest", "ParentProject_Id", "Project");
            DropColumn("ProjectChangeRequest", "ParentProject_Id");
            CreateIndex("ProjectChangeRequest", "OriginalProject_Id");
            AddForeignKey("ProjectChangeRequest", "OriginalProject_Id", "Project", "Id");
        }
    }
}
