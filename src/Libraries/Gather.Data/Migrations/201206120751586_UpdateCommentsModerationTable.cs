namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentsModerationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProjectCommentModeration", "Reason", c => c.String(maxLength: 255));
            AddColumn("ProjectCommentModeration", "ComplaintId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProjectCommentModeration", "ComplaintId");
            DropColumn("ProjectCommentModeration", "Reason");
        }
    }
}
