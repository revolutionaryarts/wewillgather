namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                        EmailAddress = c.String(maxLength: 1000),
                        EmailDisclosureId = c.Int(nullable: false),
                        EndDate = c.DateTime(),
                        Equipment = c.String(maxLength: 1000),
                        GettingThere = c.String(maxLength: 1000),
                        LastModeratorApprovalBy = c.Int(),
                        LastModeratorApprovalDate = c.DateTime(),
                        LastModifiedBy = c.Int(),
                        LastModifiedDate = c.DateTime(),
                        Name = c.String(maxLength: 40),
                        NumberOfVolunteers = c.Int(nullable: false),
                        Objective = c.String(),
                        PostcodeStr = c.String(maxLength: 15),
                        Recurrence = c.Int(nullable: false),
                        Skills = c.String(maxLength: 1000),
                        StartDate = c.DateTime(),
                        StatusId = c.Int(nullable: false),
                        Telephone = c.String(maxLength: 30),
                        TelephoneDisclosureId = c.Int(nullable: false),
                        VolunteerBenefits = c.String(maxLength: 1000),
                        Website = c.String(maxLength: 300),
                        WebsiteDisclosureId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Postcode_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Postcode", t => t.Postcode_Id)
                .Index(t => t.Postcode_Id);
            
            CreateTable(
                "Project_Category_Mapping",
                c => new
                    {
                        Project_Id = c.Int(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Category_Id })
                .ForeignKey("Project", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("Category", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "Project_Owner_Mapping",
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
            
        }
        
        public override void Down()
        {
            DropIndex("Project_Owner_Mapping", new[] { "User_Id" });
            DropIndex("Project_Owner_Mapping", new[] { "Project_Id" });
            DropIndex("Project_Category_Mapping", new[] { "Category_Id" });
            DropIndex("Project_Category_Mapping", new[] { "Project_Id" });
            DropIndex("Project", new[] { "Postcode_Id" });
            DropForeignKey("Project_Owner_Mapping", "User_Id", "User");
            DropForeignKey("Project_Owner_Mapping", "Project_Id", "Project");
            DropForeignKey("Project_Category_Mapping", "Category_Id", "Category");
            DropForeignKey("Project_Category_Mapping", "Project_Id", "Project");
            DropForeignKey("Project", "Postcode_Id", "Postcode");
            DropTable("Project_Owner_Mapping");
            DropTable("Project_Category_Mapping");
            DropTable("Project");
        }
    }
}
