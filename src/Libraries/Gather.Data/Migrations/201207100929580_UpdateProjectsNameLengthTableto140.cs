namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectsNameLengthTableto140 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Project", "Name", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            AlterColumn("Project", "Name", c => c.String(maxLength: 100));
        }
    }
}
