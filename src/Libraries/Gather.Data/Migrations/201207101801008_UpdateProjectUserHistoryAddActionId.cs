namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectUserHistoryAddActionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProjectUserHistory", "ProjectUserActionId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProjectUserHistory", "ProjectUserActionId");
        }
    }
}
