namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectApproval : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ProjectApprovalRequest", "Queue_Id", "ModerationQueue");
            DropIndex("ProjectApprovalRequest", new[] { "Queue_Id" });
            AddColumn("ProjectApprovalRequest", "ModerationQueue_Id", c => c.Int(nullable: false));
            AddForeignKey("ProjectApprovalRequest", "ModerationQueue_Id", "ModerationQueue", "Id", cascadeDelete: true);
            CreateIndex("ProjectApprovalRequest", "ModerationQueue_Id");
            DropColumn("ProjectApprovalRequest", "Queue_Id");
        }
        
        public override void Down()
        {
            AddColumn("ProjectApprovalRequest", "Queue_Id", c => c.Int(nullable: false));
            DropIndex("ProjectApprovalRequest", new[] { "ModerationQueue_Id" });
            DropForeignKey("ProjectApprovalRequest", "ModerationQueue_Id", "ModerationQueue");
            DropColumn("ProjectApprovalRequest", "ModerationQueue_Id");
            CreateIndex("ProjectApprovalRequest", "Queue_Id");
            AddForeignKey("ProjectApprovalRequest", "Queue_Id", "ModerationQueue", "Id", cascadeDelete: true);
        }
    }
}
