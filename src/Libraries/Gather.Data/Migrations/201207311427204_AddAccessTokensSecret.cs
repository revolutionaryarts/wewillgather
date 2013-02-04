namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessTokensSecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "TwitterAccessSecret", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("User", "TwitterAccessSecret");
        }
    }
}
