namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectApproval : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProjectApprovalRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Project_Id = c.Int(),
                        Queue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Project", t => t.Project_Id)
                .ForeignKey("ModerationQueue", t => t.Queue_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Queue_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("ProjectApprovalRequest", new[] { "Queue_Id" });
            DropIndex("ProjectApprovalRequest", new[] { "Project_Id" });
            DropForeignKey("ProjectApprovalRequest", "Queue_Id", "ModerationQueue");
            DropForeignKey("ProjectApprovalRequest", "Project_Id", "Project");
            DropTable("ProjectApprovalRequest");
        }
    }
}
