namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectUserHistoryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProjectUserHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
                        Project_Id = c.Int(nullable: false),
                        CommittingUser_Id = c.Int(),
                        AffectedUser_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Project", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("User", t => t.CommittingUser_Id)
                .ForeignKey("User", t => t.AffectedUser_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.CommittingUser_Id)
                .Index(t => t.AffectedUser_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("ProjectUserHistory", new[] { "AffectedUser_Id" });
            DropIndex("ProjectUserHistory", new[] { "CommittingUser_Id" });
            DropIndex("ProjectUserHistory", new[] { "Project_Id" });
            DropForeignKey("ProjectUserHistory", "AffectedUser_Id", "User");
            DropForeignKey("ProjectUserHistory", "CommittingUser_Id", "User");
            DropForeignKey("ProjectUserHistory", "Project_Id", "Project");
            DropTable("ProjectUserHistory");
        }
    }
}
