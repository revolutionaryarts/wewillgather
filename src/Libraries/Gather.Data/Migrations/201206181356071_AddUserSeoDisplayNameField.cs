namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSeoDisplayNameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "SeoDisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("User", "SeoDisplayName");
        }
    }
}
