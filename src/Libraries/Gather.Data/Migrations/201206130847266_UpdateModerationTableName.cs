namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModerationTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "ProjectCommentModeration", newName: "Moderation_ProjectComment");
            RenameTable(name: "ProjectApprovalRequest", newName: "Moderation_ProjectApproval");
            RenameTable(name: "ProjectChangeRequest", newName: "Moderation_ProjectChange");
            RenameTable(name: "ProjectModeration", newName: "Moderation_ProjectContent");
        }
        
        public override void Down()
        {
            RenameTable(name: "Moderation_ProjectContent", newName: "ProjectModeration");
            RenameTable(name: "Moderation_ProjectChange", newName: "ProjectChangeRequest");
            RenameTable(name: "Moderation_ProjectApproval", newName: "ProjectApprovalRequest");
            RenameTable(name: "Moderation_ProjectComment", newName: "ProjectCommentModeration");
        }
    }
}
