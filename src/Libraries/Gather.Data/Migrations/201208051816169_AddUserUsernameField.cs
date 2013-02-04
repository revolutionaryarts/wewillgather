namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserUsernameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "UserName", c => c.String(maxLength: 50));
            DropColumn("User", "SeoDisplayName");
        }
        
        public override void Down()
        {
            AddColumn("User", "SeoDisplayName", c => c.String());
            DropColumn("User", "UserName");
        }
    }
}
