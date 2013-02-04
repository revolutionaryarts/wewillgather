namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProjectsNameLengthTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Project", "Name", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("Project", "Name", c => c.String(maxLength: 40));
        }
    }
}
