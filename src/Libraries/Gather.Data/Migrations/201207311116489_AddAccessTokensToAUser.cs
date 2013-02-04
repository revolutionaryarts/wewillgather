namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessTokensToAUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "FacebookAccessToken", c => c.String(maxLength: 40));
            AddColumn("User", "TwitterAccessToken", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("User", "TwitterAccessToken");
            DropColumn("User", "FacebookAccessToken");
        }
    }
}
