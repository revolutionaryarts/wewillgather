namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectChangeRequestTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProjectChangeRequest",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalProject_Id = c.Int(),
                        ChangeProject_Id = c.Int(),
                        ModerationQueue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Project", t => t.OriginalProject_Id)
                .ForeignKey("Project", t => t.ChangeProject_Id)
                .ForeignKey("ModerationQueue", t => t.ModerationQueue_Id, cascadeDelete: true)
                .Index(t => t.OriginalProject_Id)
                .Index(t => t.ChangeProject_Id)
                .Index(t => t.ModerationQueue_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("ProjectChangeRequest", new[] { "ModerationQueue_Id" });
            DropIndex("ProjectChangeRequest", new[] { "ChangeProject_Id" });
            DropIndex("ProjectChangeRequest", new[] { "OriginalProject_Id" });
            DropForeignKey("ProjectChangeRequest", "ModerationQueue_Id", "ModerationQueue");
            DropForeignKey("ProjectChangeRequest", "ChangeProject_Id", "Project");
            DropForeignKey("ProjectChangeRequest", "OriginalProject_Id", "Project");
            DropTable("ProjectChangeRequest");
        }
    }
}
