namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddingVolunteersToProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Project_Volunteer_Mapping",
                c => new
                    {
                        Project_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.User_Id })
                .ForeignKey("Project", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("User", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.User_Id);
            
            AddColumn("Project", "RemainingNumberOfVolunteers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropIndex("Project_Volunteer_Mapping", new[] { "User_Id" });
            DropIndex("Project_Volunteer_Mapping", new[] { "Project_Id" });
            DropForeignKey("Project_Volunteer_Mapping", "User_Id", "User");
            DropForeignKey("Project_Volunteer_Mapping", "Project_Id", "Project");
            DropColumn("Project", "RemainingNumberOfVolunteers");
            DropTable("Project_Volunteer_Mapping");
        }
    }
}
