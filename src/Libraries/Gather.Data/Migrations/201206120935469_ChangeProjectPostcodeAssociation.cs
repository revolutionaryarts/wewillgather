namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangeProjectPostcodeAssociation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Project", "Postcode_Id", "Postcode");
            DropIndex("Project", new[] { "Postcode_Id" });
            AddColumn("Project", "Location_Id", c => c.Int());
            AddForeignKey("Project", "Location_Id", "Location", "Id");
            CreateIndex("Project", "Location_Id");
            DropColumn("Project", "PostcodeStr");
            DropColumn("Project", "Postcode_Id");
        }
        
        public override void Down()
        {
            AddColumn("Project", "Postcode_Id", c => c.Int());
            AddColumn("Project", "PostcodeStr", c => c.String(maxLength: 15));
            DropIndex("Project", new[] { "Location_Id" });
            DropForeignKey("Project", "Location_Id", "Location");
            DropColumn("Project", "Location_Id");
            CreateIndex("Project", "Postcode_Id");
            AddForeignKey("Project", "Postcode_Id", "Postcode", "Id");
        }
    }
}
