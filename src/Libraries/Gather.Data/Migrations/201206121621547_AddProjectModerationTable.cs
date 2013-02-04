namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectModerationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProjectModeration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(maxLength: 255),
                        ComplaintId = c.Int(nullable: false),
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
            DropIndex("ProjectModeration", new[] { "ModerationQueue_Id" });
            DropIndex("ProjectModeration", new[] { "Project_Id" });
            DropForeignKey("ProjectModeration", "ModerationQueue_Id", "ModerationQueue");
            DropForeignKey("ProjectModeration", "Project_Id", "Project");
            DropTable("ProjectModeration");
        }
    }
}
