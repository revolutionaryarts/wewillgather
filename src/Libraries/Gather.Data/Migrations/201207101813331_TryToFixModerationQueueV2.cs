namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TryToFixModerationQueueV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Moderation_ProjectComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(maxLength: 255),
                        ComplaintId = c.Int(nullable: false),
                        Comment_Id = c.Int(nullable: false),
                        ModerationQueue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Comment", t => t.Comment_Id, cascadeDelete: true)
                .ForeignKey("ModerationQueue", t => t.ModerationQueue_Id, cascadeDelete: true)
                .Index(t => t.Comment_Id)
                .Index(t => t.ModerationQueue_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Moderation_ProjectComment", new[] { "ModerationQueue_Id" });
            DropIndex("Moderation_ProjectComment", new[] { "Comment_Id" });
            DropForeignKey("Moderation_ProjectComment", "ModerationQueue_Id", "ModerationQueue");
            DropForeignKey("Moderation_ProjectComment", "Comment_Id", "Comment");
            DropTable("Moderation_ProjectComment");
        }
    }
}
