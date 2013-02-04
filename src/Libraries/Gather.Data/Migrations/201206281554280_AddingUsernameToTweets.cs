namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddingUsernameToTweets : DbMigration
    {
        public override void Up()
        {
            AddColumn("Tweet", "UserName", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("Tweet", "UserName");
        }
    }
}
