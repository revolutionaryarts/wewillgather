namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddTwitterIdToProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("Project", "TwitterProfile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Project", "TwitterProfile");
        }
    }
}
