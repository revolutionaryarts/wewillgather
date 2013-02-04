namespace Gather.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPrimaryAuthMethod : DbMigration
    {
        public override void Up()
        {
            AddColumn("User", "PrimaryAuthMethodId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("User", "PrimaryAuthMethodId");
        }
    }
}
