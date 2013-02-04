namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ExtendAccessTokensSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("User", "FacebookAccessToken", c => c.String(maxLength: 200));
            AlterColumn("User", "TwitterAccessToken", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("User", "TwitterAccessToken", c => c.String(maxLength: 40));
            AlterColumn("User", "FacebookAccessToken", c => c.String(maxLength: 40));
        }
    }
}
