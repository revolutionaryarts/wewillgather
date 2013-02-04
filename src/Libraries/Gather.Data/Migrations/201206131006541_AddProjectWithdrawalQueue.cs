namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectWithdrawalQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Moderation_ProjectWithdrawal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Project_Id = c.Int(),
                        ModerationQueue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Project", t => t.Project_Id)
                .ForeignKey("ModerationQueue", t => t.ModerationQueue_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.ModerationQueue_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Moderation_ProjectWithdrawal", new[] { "ModerationQueue_Id" });
            DropIndex("Moderation_ProjectWithdrawal", new[] { "Project_Id" });
            DropForeignKey("Moderation_ProjectWithdrawal", "ModerationQueue_Id", "ModerationQueue");
            DropForeignKey("Moderation_ProjectWithdrawal", "Project_Id", "Project");
            DropTable("Moderation_ProjectWithdrawal");
        }
    }
}
