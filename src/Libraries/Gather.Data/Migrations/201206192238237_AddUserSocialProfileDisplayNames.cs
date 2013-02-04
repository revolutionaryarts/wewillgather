namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSocialProfileDisplayNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "FacebookDisplayName", c => c.String());
            AddColumn("User", "TwitterDisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("User", "TwitterDisplayName");
            DropColumn("User", "FacebookDisplayName");
        }
    }
}
