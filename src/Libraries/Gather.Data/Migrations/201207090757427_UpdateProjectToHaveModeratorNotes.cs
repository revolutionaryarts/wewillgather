namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectToHaveModeratorNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "ModeratorNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Project", "ModeratorNotes");
        }
    }
}
