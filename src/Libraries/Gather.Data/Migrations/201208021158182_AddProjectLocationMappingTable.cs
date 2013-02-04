namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectLocationMappingTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Project", "Location_Id", "Location");
            DropIndex("Project", new[] { "Location_Id" });
            CreateTable(
                "Project_Location_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationId = c.Int(nullable: false),
                        Primary = c.Boolean(nullable: false),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Location", t => t.LocationId, cascadeDelete: true)
                .ForeignKey("Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.LocationId)
                .Index(t => t.ProjectId);
            
            DropColumn("Project", "Location_Id");
        }
        
        public override void Down()
        {
            AddColumn("Project", "Location_Id", c => c.Int());
            DropIndex("Project_Location_Mapping", new[] { "ProjectId" });
            DropIndex("Project_Location_Mapping", new[] { "LocationId" });
            DropForeignKey("Project_Location_Mapping", "ProjectId", "Project");
            DropForeignKey("Project_Location_Mapping", "LocationId", "Location");
            DropTable("Project_Location_Mapping");
            CreateIndex("Project", "Location_Id");
            AddForeignKey("Project", "Location_Id", "Location", "Id");
        }
    }
}
