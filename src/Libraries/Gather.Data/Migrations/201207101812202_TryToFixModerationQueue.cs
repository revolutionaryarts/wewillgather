namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TryToFixModerationQueue : DbMigration
    {
        public override void Up()
        {
            
            
            DropIndex("Moderation_ProjectComment", new[] { "Comment_Id" });
            DropIndex("Moderation_ProjectComment", new[] { "ModerationQueue_Id" });
            DropTable("Moderation_ProjectComment");
        }
        
        public override void Down()
        {
            CreateTable(
                "Moderation_ProjectComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(maxLength: 255),
                        ComplaintId = c.Int(nullable: false),
                        Comment_Id = c.Int(),
                        ModerationQueue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("Moderation_ProjectComment", "ModerationQueue_Id");
            CreateIndex("Moderation_ProjectComment", "Comment_Id");
            AddForeignKey("Moderation_ProjectComment", "ModerationQueue_Id", "ModerationQueue", "Id", cascadeDelete: true);
            AddForeignKey("Moderation_ProjectComment", "Comment_Id", "Comment", "Id");
        }
    }
}
